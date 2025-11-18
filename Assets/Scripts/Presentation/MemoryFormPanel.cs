// Assets/Scripts/Presentation/MemoryFormPanel.cs

using ARMemoryGarden.Infrastructure.AR;
using ARMemoryGarden.Presentation;
using Domain.Models;
using Infrastructure.Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation
{
    public class MemoryFormPanel : MonoBehaviour
    {
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private TMP_InputField _noteInput;

        [Header("Buttons")]
        [SerializeField] private Button _photoButton;
        [SerializeField] private Button _audioButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _cancelButton;

        [Header("Indicators")]
        [SerializeField] private GameObject _photoAddedIndicator;
        [SerializeField] private GameObject _audioAddedIndicator;
        [SerializeField] private TextMeshProUGUI _validationErrorText;

        private Pose _placementPose;
        private byte[] _selectedPhotoBytes;
        private byte[] _recordedAudioBytes;

        private void Awake()
        {
            _saveButton?.onClick.AddListener(OnSaveButtonClicked);
            _cancelButton?.onClick.AddListener(OnCancelButtonClicked);
            _photoButton?.onClick.AddListener(OnPhotoButtonClicked);
            _audioButton?.onClick.AddListener(OnAudioButtonClicked);

            gameObject.SetActive(false);
        }

        public void Show(Pose placementPose)
        {
            _placementPose = placementPose;
            gameObject.SetActive(true);
            ClearForm();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ClearForm()
        {
            if (_titleInput != null) _titleInput.text = "";
            if (_noteInput != null) _noteInput.text = "";
            
            _selectedPhotoBytes = null;
            _recordedAudioBytes = null;

            if (_photoAddedIndicator != null) _photoAddedIndicator.SetActive(false);
            if (_audioAddedIndicator != null) _audioAddedIndicator.SetActive(false);
            if (_validationErrorText != null) _validationErrorText.text = "";
        }

        private async void OnSaveButtonClicked()
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(_titleInput?.text))
            {
                ShowValidationError("Başlık zorunludur");
                return;
            }

            // Memory oluştur
            var memory = new Memory
            {
                title = _titleInput.text.Trim(),
                note = _noteInput?.text?.Trim() ?? "",
                anchorId = System.Guid.NewGuid().ToString(),
                localPosition = new SerializableVector3(_placementPose.position),
                localRotation = new SerializableQuaternion(_placementPose.rotation)
            };

            // Service locator'dan use case al
            var addMemoryUseCase = ServiceLocator.Instance.AddMemoryUseCase;
            var result = await addMemoryUseCase.ExecuteAsync(memory, _selectedPhotoBytes, _recordedAudioBytes);

            if (result != null)
            {
                Debug.Log($"[MemoryFormPanel] Memory saved successfully: {result.id}");

                // AR sahnesinde prefab yerleştir
                var raycastController = FindObjectOfType<ARRaycastController>();
                if (raycastController != null)
                {
                    raycastController.PlaceMemoryWithAnchor(_placementPose, result.id);
                }

                // HUD'a bildir
                var hud = FindObjectOfType<HUDController>();
                hud?.ShowSuccess("Anı kaydedildi!");
                hud?.ShowScanningIndicator(false);

                Hide();
            }
            else
            {
                ShowValidationError("Kaydetme başarısız");
            }
        }

        private void OnCancelButtonClicked()
        {
            Hide();
            
            var hud = FindObjectOfType<HUDController>();
            hud?.SetStatusText("Yüzeyleri tara...");
        }

        private void OnPhotoButtonClicked()
        {
            Debug.Log("[MemoryFormPanel] Photo button clicked");
            
            // TODO: Gerçek fotoğraf seçimi
            // Şimdilik simülasyon
            _selectedPhotoBytes = new byte[1024];
            
            if (_photoAddedIndicator != null)
            {
                _photoAddedIndicator.SetActive(true);
            }

            ShowValidationError(""); // Hata mesajını temizle
        }

        private void OnAudioButtonClicked()
        {
            Debug.Log("[MemoryFormPanel] Audio button clicked");
            
            // TODO: Gerçek ses kaydı
            _recordedAudioBytes = new byte[512];
            
            if (_audioAddedIndicator != null)
            {
                _audioAddedIndicator.SetActive(true);
            }

            ShowValidationError("");
        }

        private void ShowValidationError(string message)
        {
            if (_validationErrorText != null)
            {
                _validationErrorText.text = message;
                _validationErrorText.color = Color.red;
            }

            if (!string.IsNullOrEmpty(message))
            {
                Debug.LogWarning($"[MemoryFormPanel] Validation error: {message}");
            }
        }
    }
}
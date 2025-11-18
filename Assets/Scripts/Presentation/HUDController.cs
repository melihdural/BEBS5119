// Assets/Scripts/Presentation/HUDController.cs

using Presentation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARMemoryGarden.Presentation
{
    /// <summary>
    /// Ana HUD kontrolcüsü. Tarama durumu, butonlar, notifikasyonlar.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _addMemoryButton;
        [SerializeField] private Button _listMemoriesButton;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private GameObject _scanningIndicator;

        [Header("Panels")]
        [SerializeField] private MemoryFormPanel _memoryFormPanel;
        [SerializeField] private MemoryListPanel _memoryListPanel;

        private Infrastructure.AR.ARRaycastController _raycastController;
        private bool _isPlacementMode = false;

        private void Awake()
        {
            // Referansları otomatik bul
            if (_raycastController == null)
            {
                _raycastController = FindObjectOfType<Infrastructure.AR.ARRaycastController>();
            }

            if (_memoryFormPanel == null)
            {
                _memoryFormPanel = FindObjectOfType<MemoryFormPanel>();
            }

            if (_memoryListPanel == null)
            {
                _memoryListPanel = FindObjectOfType<MemoryListPanel>();
            }

            // Buton event'lerini bağla
            if (_addMemoryButton != null)
            {
                _addMemoryButton.onClick.AddListener(OnAddMemoryButtonClicked);
            }

            if (_listMemoriesButton != null)
            {
                _listMemoriesButton.onClick.AddListener(OnListMemoriesButtonClicked);
            }
        }

        private void Start()
        {
            SetStatusText("Yüzeyleri tara...");
            ShowScanningIndicator(true);

            // Raycast event'lerini dinle
            if (_raycastController != null)
            {
                _raycastController.OnPlacementConfirmed += OnPlacementConfirmed;
            }
            else
            {
                Debug.LogError("[HUDController] ARRaycastController not found!");
            }
        }

        private void OnDestroy()
        {
            if (_raycastController != null)
            {
                _raycastController.OnPlacementConfirmed -= OnPlacementConfirmed;
            }
        }

        private void OnAddMemoryButtonClicked()
        {
            Debug.Log("[HUDController] Add Memory button clicked");
            
            // Placement mode'a geç
            _isPlacementMode = true;
            _raycastController?.SetPlacementMode(true);
            SetStatusText("Çiçeği yerleştirmek için ekrana dokun");
        }

        private void OnListMemoriesButtonClicked()
        {
            Debug.Log("[HUDController] List Memories button clicked");
            
            if (_memoryListPanel != null)
            {
                _memoryListPanel.Show();
            }
        }

        private void OnPlacementConfirmed(Pose pose)
        {
            if (!_isPlacementMode) return;

            Debug.Log("[HUDController] Placement confirmed, showing form");

            // Placement mode'dan çık
            _isPlacementMode = false;
            _raycastController?.SetPlacementMode(false);

            // Form panelini aç
            if (_memoryFormPanel != null)
            {
                _memoryFormPanel.Show(pose);
            }
            else
            {
                Debug.LogError("[HUDController] MemoryFormPanel not found!");
            }

            SetStatusText("Anı formu dolduruluyor...");
        }

        public void SetStatusText(string text)
        {
            if (_statusText != null)
            {
                _statusText.text = text;
            }
        }

        public void ShowScanningIndicator(bool show)
        {
            if (_scanningIndicator != null)
            {
                _scanningIndicator.SetActive(show);
            }
        }

        public void ShowError(string message)
        {
            Debug.LogError($"[HUDController] Error: {message}");
            SetStatusText($"Hata: {message}");
        }

        public void ShowSuccess(string message)
        {
            Debug.Log($"[HUDController] Success: {message}");
            SetStatusText(message);
        }
    }
}
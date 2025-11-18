// Assets/Scripts/Presentation/MemoryCard.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Domain.Models;
using Infrastructure.Persistence;

namespace ARMemoryGarden.Presentation
{
    public class MemoryCard : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _dateText;
        [SerializeField] private TextMeshProUGUI _notePreviewText;
        [SerializeField] private Image _thumbnailImage;
        [SerializeField] private GameObject _hasPhotoIndicator;
        [SerializeField] private GameObject _hasAudioIndicator;
        [SerializeField] private Button _cardButton;
        [SerializeField] private Button _deleteButton;

        private Memory _memory;

        private void Awake()
        {
            if (_cardButton != null)
            {
                _cardButton.onClick.AddListener(OnCardClicked);
            }

            if (_deleteButton != null)
            {
                _deleteButton.onClick.AddListener(OnDeleteClicked);
            }
        }

        public void Initialize(Memory memory)
        {
            _memory = memory;

            if (_titleText != null)
            {
                _titleText.text = memory.title;
            }

            if (_dateText != null)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(memory.createdTimestamp).LocalDateTime;
                _dateText.text = date.ToString("dd.MM.yyyy HH:mm");
            }

            if (_notePreviewText != null)
            {
                if (!string.IsNullOrEmpty(memory.note))
                {
                    string preview = memory.note.Length > 50 
                        ? memory.note.Substring(0, 50) + "..." 
                        : memory.note;
                    _notePreviewText.text = preview;
                }
                else
                {
                    _notePreviewText.text = "";
                }
            }

            if (_hasPhotoIndicator != null)
            {
                _hasPhotoIndicator.SetActive(!string.IsNullOrEmpty(memory.photoPath));
            }

            if (_hasAudioIndicator != null)
            {
                _hasAudioIndicator.SetActive(!string.IsNullOrEmpty(memory.audioPath));
            }
        }

        private void OnCardClicked()
        {
            Debug.Log($"[MemoryCard] Card clicked: {_memory?.title}");
            // TODO: Detay paneli
        }

        private async void OnDeleteClicked()
        {
            if (_memory == null) return;

            Debug.Log($"[MemoryCard] Delete clicked: {_memory.id}");

            var deleteUseCase = ServiceLocator.Instance.DeleteMemoryUseCase;
            bool result = await deleteUseCase.ExecuteAsync(_memory.id);

            if (result)
            {
                Debug.Log("[MemoryCard] Memory deleted successfully");
                Destroy(gameObject);

                var hud = FindObjectOfType<HUDController>();
                hud?.ShowSuccess("Anı silindi");
            }
            else
            {
                Debug.LogError("[MemoryCard] Failed to delete memory");
            }
        }
    }
}
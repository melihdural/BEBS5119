// Assets/Scripts/Presentation/MemoryListPanel.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Domain.Models;
using Infrastructure.Persistence;

namespace ARMemoryGarden.Presentation
{
    public class MemoryListPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform _listContainer;
        [SerializeField] private GameObject _memoryCardPrefab;
        [SerializeField] private Button _closeButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _emptyListMessage;

        private List<GameObject> _instantiatedCards = new List<GameObject>();

        private void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(Hide);
            }

            gameObject.SetActive(false);
        }

        public async void Show()
        {
            gameObject.SetActive(true);

            // Mevcut kartları temizle
            ClearCards();

            // Service locator'dan use case al
            var listMemoriesUseCase = ServiceLocator.Instance.ListMemoriesUseCase;
            var memories = await listMemoriesUseCase.ExecuteAsync();

            if (memories == null || memories.Count == 0)
            {
                Debug.Log("[MemoryListPanel] No memories to display");
                if (_emptyListMessage != null)
                {
                    _emptyListMessage.SetActive(true);
                }
                return;
            }

            if (_emptyListMessage != null)
            {
                _emptyListMessage.SetActive(false);
            }

            // Her anı için kart oluştur
            foreach (var memory in memories)
            {
                CreateMemoryCard(memory);
            }

            Debug.Log($"[MemoryListPanel] Displayed {memories.Count} memories");
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void CreateMemoryCard(Memory memory)
        {
            if (_memoryCardPrefab == null || _listContainer == null)
            {
                Debug.LogError("[MemoryListPanel] Card prefab or container not assigned");
                return;
            }

            GameObject cardObject = Instantiate(_memoryCardPrefab, _listContainer);
            _instantiatedCards.Add(cardObject);

            var memoryCard = cardObject.GetComponent<MemoryCard>();
            if (memoryCard != null)
            {
                memoryCard.Initialize(memory);
            }
        }

        private void ClearCards()
        {
            foreach (var card in _instantiatedCards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }

            _instantiatedCards.Clear();
        }
    }
}
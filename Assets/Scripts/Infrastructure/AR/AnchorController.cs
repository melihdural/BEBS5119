// Assets/Scripts/Infrastructure/AR/AnchorController.cs

using System;
using System.Collections.Generic;
using Domain.Models;
using Domain.Repositories;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Infrastructure.AR
{
    /// <summary>
    /// AR anchor oluşturma ve yönetimi
    /// </summary>
    [RequireComponent(typeof(ARAnchorManager))]
    public class AnchorController : MonoBehaviour
    {
        private ARAnchorManager _anchorManager;
        private Dictionary<string, ARAnchor> _activeAnchors = new Dictionary<string, ARAnchor>();
        private IAnchorRepository _anchorRepository;

        private void Awake()
        {
            _anchorManager = GetComponent<ARAnchorManager>();
            _anchorRepository = new Infrastructure.Persistence.JsonAnchorRepository();
        }

        /// <summary>
        /// Verilen pozisyonda anchor oluştur
        /// </summary>
        public ARAnchor CreateAnchor(Pose pose, string anchorId = null)
        {
            if (anchorId == null)
            {
                anchorId = Guid.NewGuid().ToString();
            }

            var anchor = _anchorManager.AddAnchor(pose);
            
            if (anchor != null)
            {
                _activeAnchors[anchorId] = anchor;
                
                // Repository'ye kaydet
                var anchorRef = new AnchorRef(anchorId, pose.position, pose.rotation);
                _ = _anchorRepository.AddAsync(anchorRef); // Async çağrı, await etmiyoruz

                Debug.Log($"[AnchorController] Anchor created: {anchorId}");
                return anchor;
            }

            Debug.LogError("[AnchorController] Failed to create anchor");
            return null;
        }

        /// <summary>
        /// Var olan anchor'ı getir
        /// </summary>
        public ARAnchor GetAnchor(string anchorId)
        {
            if (_activeAnchors.TryGetValue(anchorId, out var anchor))
            {
                return anchor;
            }

            Debug.LogWarning($"[AnchorController] Anchor not found: {anchorId}");
            return null;
        }

        /// <summary>
        /// Anchor sil
        /// </summary>
        public void RemoveAnchor(string anchorId)
        {
            if (_activeAnchors.TryGetValue(anchorId, out var anchor))
            {
                _anchorManager.RemoveAnchor(anchor);
                _activeAnchors.Remove(anchorId);
                _ = _anchorRepository.DeleteAsync(anchorId);
                
                Debug.Log($"[AnchorController] Anchor removed: {anchorId}");
            }
        }

        /// <summary>
        /// Tüm anchor'ları temizle
        /// </summary>
        public void ClearAllAnchors()
        {
            foreach (var kvp in _activeAnchors)
            {
                if (kvp.Value != null)
                {
                    _anchorManager.RemoveAnchor(kvp.Value);
                }
            }

            _activeAnchors.Clear();
            Debug.Log("[AnchorController] All anchors cleared");
        }
    }
}
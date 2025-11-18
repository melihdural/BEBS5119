// Assets/Scripts/Infrastructure/AR/PlaneDetectionController.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Infrastructure.AR
{
    /// <summary>
    /// AR düzlem tespiti yöneticisi
    /// </summary>
    [RequireComponent(typeof(ARPlaneManager))]
    public class PlaneDetectionController : MonoBehaviour
    {
        [SerializeField] private bool _visualizePlanes = true;

        private ARPlaneManager _planeManager;
        private Dictionary<TrackableId, ARPlane> _planes = new Dictionary<TrackableId, ARPlane>();

        private void Awake()
        {
            _planeManager = GetComponent<ARPlaneManager>();
        }

        private void OnEnable()
        {
            _planeManager.planesChanged += OnPlanesChanged;
        }

        private void OnDisable()
        {
            _planeManager.planesChanged -= OnPlanesChanged;
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            // Yeni düzlemler eklendi
            foreach (var plane in args.added)
            {
                _planes[plane.trackableId] = plane;
                Debug.Log($"[PlaneDetectionController] Plane added: {plane.trackableId}");
            }

            // Düzlemler güncellendi
            foreach (var plane in args.updated)
            {
                _planes[plane.trackableId] = plane;
            }

            // Düzlemler kaldırıldı
            foreach (var plane in args.removed)
            {
                _planes.Remove(plane.trackableId);
                Debug.Log($"[PlaneDetectionController] Plane removed: {plane.trackableId}");
            }
        }

        /// <summary>
        /// Düzlem tespitini aç/kapat
        /// </summary>
        public void SetPlaneDetectionEnabled(bool enabled)
        {
            _planeManager.enabled = enabled;
            Debug.Log($"[PlaneDetectionController] Plane detection: {enabled}");
        }

        /// <summary>
        /// Düzlem görselleştirmesini aç/kapat
        /// </summary>
        public void SetPlaneVisualization(bool visible)
        {
            _visualizePlanes = visible;

            foreach (var plane in _planes.Values)
            {
                if (plane != null)
                {
                    plane.gameObject.SetActive(visible);
                }
            }
        }

        /// <summary>
        /// En büyük düzlemi getir (anchor yerleştirme için)
        /// </summary>
        public ARPlane GetLargestPlane()
        {
            ARPlane largest = null;
            float maxArea = 0f;

            foreach (var plane in _planes.Values)
            {
                if (plane != null && plane.size.x * plane.size.y > maxArea)
                {
                    maxArea = plane.size.x * plane.size.y;
                    largest = plane;
                }
            }

            return largest;
        }
    }
}
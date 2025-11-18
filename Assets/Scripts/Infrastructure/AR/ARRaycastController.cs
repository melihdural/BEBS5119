// Assets/Scripts/Infrastructure/AR/ARRaycastController.cs

using System.Collections.Generic;
using Infrastructure.AR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace ARMemoryGarden.Infrastructure.AR
{
    /// <summary>
    /// AR raycast ile dokunma tespiti ve obje yerleştirme
    /// Yeni Input System uyumlu versiyon
    /// </summary>
    public class ARRaycastController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ARRaycastManager _raycastManager;
        [SerializeField] private AnchorController _anchorController;
        [SerializeField] private GameObject _memoryPrefab;

        [Header("Placement")]
        [SerializeField] private GameObject _placementIndicator;

        private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
        private Pose _placementPose;
        private bool _placementPoseValid = false;

        public delegate void OnPlacementDelegate(Pose pose);
        public event OnPlacementDelegate OnPlacementConfirmed;

        private void Awake()
        {
            if (_raycastManager == null)
            {
                _raycastManager = FindObjectOfType<ARRaycastManager>();
            }

            if (_anchorController == null)
            {
                _anchorController = FindObjectOfType<AnchorController>();
            }

            if (_placementIndicator != null)
            {
                _placementIndicator.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Enhanced Touch'ı etkinleştir
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            // Enhanced Touch'ı devre dışı bırak
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();

            // Yeni Input System ile dokunma tespiti
            if (Touch.activeTouches.Count > 0)
            {
                Touch touch = Touch.activeTouches[0];
                
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    if (!IsPointerOverUIObject(touch.screenPosition))
                    {
                        HandleTouch(touch.screenPosition);
                    }
                }
            }

            // Mouse ile test (Editor'de) - Yeni Input System
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (!IsPointerOverUIObject(mousePos))
                {
                    HandleTouch(mousePos);
                }
            }
        }

        private void UpdatePlacementPose()
        {
            Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

            _raycastHits.Clear();
            _placementPoseValid = _raycastManager.Raycast(screenCenter, _raycastHits, TrackableType.Planes);

            if (_placementPoseValid && _raycastHits.Count > 0)
            {
                _placementPose = _raycastHits[0].pose;
            }
        }

        private void UpdatePlacementIndicator()
        {
            if (_placementIndicator != null)
            {
                _placementIndicator.SetActive(_placementPoseValid);

                if (_placementPoseValid)
                {
                    _placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
                }
            }
        }

        private void HandleTouch(Vector2 touchPosition)
        {
            _raycastHits.Clear();

            if (_raycastManager.Raycast(touchPosition, _raycastHits, TrackableType.Planes))
            {
                Pose hitPose = _raycastHits[0].pose;
                
                // Event tetikle
                OnPlacementConfirmed?.Invoke(hitPose);
                
                Debug.Log($"[ARRaycastController] Placement confirmed at {hitPose.position}");
            }
        }

        /// <summary>
        /// Verilen pose'da memory prefab yerleştir ve anchor oluştur
        /// </summary>
        public GameObject PlaceMemoryWithAnchor(Pose pose, string memoryId)
        {
            if (_memoryPrefab == null)
            {
                Debug.LogError("[ARRaycastController] Memory prefab not assigned");
                return null;
            }

            // Anchor oluştur
            var anchor = _anchorController.CreateAnchor(pose, memoryId);
            
            if (anchor != null)
            {
                // Prefab'ı anchor'ın child'ı olarak instantiate et
                GameObject memoryObject = Instantiate(_memoryPrefab, anchor.transform);
                memoryObject.transform.localPosition = Vector3.zero;
                memoryObject.transform.localRotation = Quaternion.identity;

                Debug.Log($"[ARRaycastController] Memory placed with anchor: {memoryId}");
                return memoryObject;
            }

            return null;
        }

        /// <summary>
        /// Placement indicator'ı aç/kapat
        /// </summary>
        public void SetPlacementMode(bool enabled)
        {
            if (_placementIndicator != null)
            {
                _placementIndicator.SetActive(enabled && _placementPoseValid);
            }
        }

        private bool IsPointerOverUIObject(Vector2 screenPosition)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
    }
}
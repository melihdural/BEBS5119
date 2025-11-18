// Assets/Scripts/Presentation/PlacementIndicator.cs

using UnityEngine;

namespace Presentation
{
    /// <summary>
    /// AR placement reticle/gösterge. ARRaycastController tarafından güncellenir.
    /// </summary>
    public class PlacementIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject _visualIndicator;
        [SerializeField] private float _rotationSpeed = 30f;

        private void Update()
        {
            // Yavaşça döndür
            if (_visualIndicator != null)
            {
                _visualIndicator.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
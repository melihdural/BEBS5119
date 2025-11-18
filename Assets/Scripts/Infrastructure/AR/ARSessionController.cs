// Assets/Scripts/Infrastructure/AR/ARSessionController.cs

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Infrastructure.AR
{
    /// <summary>
    /// AR session yaşam döngüsünü yönetir
    /// </summary>
    [RequireComponent(typeof(ARSession))]
    public class ARSessionController : MonoBehaviour
    {
        private ARSession _arSession;

        private void Awake()
        {
            _arSession = GetComponent<ARSession>();
        }

        private void Start()
        {
            StartCoroutine(CheckARSupport());
        }

        private System.Collections.IEnumerator CheckARSupport()
        {
            if ((ARSession.state == ARSessionState.None) ||
                (ARSession.state == ARSessionState.CheckingAvailability))
            {
                yield return ARSession.CheckAvailability();
            }

            if (ARSession.state == ARSessionState.Unsupported)
            {
                Debug.LogError("[ARSessionController] AR not supported on this device");
                // TODO: Kullanıcıya hata mesajı göster
            }
            else
            {
                Debug.Log("[ARSessionController] AR is supported");
            }
        }

        /// <summary>
        /// AR session'ı sıfırla
        /// </summary>
        public void ResetSession()
        {
            Debug.Log("[ARSessionController] Resetting AR session");
            _arSession.Reset();
        }

        /// <summary>
        /// Session durumunu kontrol et
        /// </summary>
        public bool IsSessionReady()
        {
            return ARSession.state == ARSessionState.SessionTracking;
        }
    }
}
// Assets/Scripts/Utils/PermissionHelper.cs

using System.Threading.Tasks;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Utils
{
    /// <summary>
    /// Platform-agnostik izin yönetimi
    /// </summary>
    public static class PermissionHelper
    {
        /// <summary>
        /// Kamera izni kontrol et
        /// </summary>
        public static bool HasCameraPermission()
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
            return Application.HasUserAuthorization(UserAuthorization.WebCam);
#else
            return true;
#endif
        }

        /// <summary>
        /// Kamera izni iste
        /// </summary>
        public static async Task<bool> RequestCameraPermissionAsync()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                
                // İzin dialogunun kapanmasını bekle
                await Task.Delay(500);
                
                return Permission.HasUserAuthorizedPermission(Permission.Camera);
            }
            return true;
#elif UNITY_IOS
            var authorization = Application.RequestUserAuthorization(UserAuthorization.WebCam);
            while (!authorization.isDone)
            {
                await Task.Delay(100);
            }
            return Application.HasUserAuthorization(UserAuthorization.WebCam);
#else
            return true;
#endif
        }

        /// <summary>
        /// Mikrofon izni kontrol et
        /// </summary>
        public static bool HasMicrophonePermission()
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#elif UNITY_IOS
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
#else
            return true;
#endif
        }

        /// <summary>
        /// Mikrofon izni iste
        /// </summary>
        public static async Task<bool> RequestMicrophonePermissionAsync()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
                await Task.Delay(500);
                return Permission.HasUserAuthorizedPermission(Permission.Microphone);
            }
            return true;
#elif UNITY_IOS
            var authorization = Application.RequestUserAuthorization(UserAuthorization.Microphone);
            while (!authorization.isDone)
            {
                await Task.Delay(100);
            }
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
#else
            return true;
#endif
        }

        /// <summary>
        /// Tüm gerekli izinleri kontrol et
        /// </summary>
        public static async Task<bool> RequestAllPermissionsAsync()
        {
            bool camera = await RequestCameraPermissionAsync();
            bool microphone = await RequestMicrophonePermissionAsync();

            if (!camera)
            {
                Debug.LogError("[PermissionHelper] Camera permission denied");
            }

            if (!microphone)
            {
                Debug.LogWarning("[PermissionHelper] Microphone permission denied");
            }

            return camera; // Kamera zorunlu, mikrofon opsiyonel
        }
    }
}
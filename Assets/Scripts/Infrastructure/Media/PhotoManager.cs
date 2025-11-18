// Assets/Scripts/Infrastructure/Media/PhotoManager.cs

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Media
{
    /// <summary>
    /// Platform-agnostik fotoğraf seçme/çekme yöneticisi.
    /// Unity'nin NativeGallery veya benzeri bir plugin kullanılabilir.
    /// Bu stub implementasyonu temel arayüzü gösterir.
    /// </summary>
    public class PhotoManager : MonoBehaviour
    {
        /// <summary>
        /// Galeriden fotoğraf seç
        /// </summary>
        public async Task<byte[]> PickPhotoAsync()
        {
            try
            {
                // TODO: NativeGallery.GetImageFromGallery kullan
                // Şimdilik placeholder

#if UNITY_ANDROID || UNITY_IOS
                Debug.Log("[PhotoManager] Opening native gallery...");
                
                // Simülasyon - gerçek implementasyonda plugin kullan
                await Task.Delay(500);
                
                // Dummy data döndür
                return new byte[1024 * 100]; // 100KB placeholder
#else
                Debug.LogWarning("[PhotoManager] Gallery not supported in editor");
                return null;
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PhotoManager] PickPhotoAsync failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kamera ile fotoğraf çek
        /// </summary>
        public async Task<byte[]> TakePhotoAsync()
        {
            try
            {
                // TODO: NativeCamera.TakePicture kullan

#if UNITY_ANDROID || UNITY_IOS
                Debug.Log("[PhotoManager] Opening native camera...");
                
                await Task.Delay(500);
                
                return new byte[1024 * 150]; // 150KB placeholder
#else
                Debug.LogWarning("[PhotoManager] Camera not supported in editor");
                return null;
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PhotoManager] TakePhotoAsync failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Fotoğrafı 2MB altına compress et
        /// </summary>
        public byte[] CompressPhoto(byte[] photoBytes, int maxSizeBytes = 2 * 1024 * 1024)
        {
            if (photoBytes.Length <= maxSizeBytes)
            {
                return photoBytes;
            }

            // TODO: Image compression (Texture2D + EncodeToPNG/JPG)
            Debug.LogWarning($"[PhotoManager] Photo size {photoBytes.Length} exceeds {maxSizeBytes}, compression needed");
            
            // Placeholder
            return photoBytes;
        }
    }
}
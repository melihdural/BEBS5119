// Assets/Scripts/Infrastructure/Persistence/FileSystemHelper.cs

using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Dosya sistemi yardımcı sınıfı. Medya dosyalarını yönetir.
    /// </summary>
    public static class FileSystemHelper
    {
        // Unity'nin persistent data path'i için tam nitelikli erişim (olası Application adı çakışmalarını önler)
        private static string persistentDataPath => UnityEngine.Application.persistentDataPath;

        /// <summary>
        /// Fotoğraf dosyasını persistentDataPath/photos/ altına kopyala
        /// </summary>
        public static async Task<string> SavePhotoAsync(byte[] photoBytes, string memoryId)
        {
            string photoDir = Path.Combine(persistentDataPath, "photos");
            if (!Directory.Exists(photoDir))
            {
                Directory.CreateDirectory(photoDir);
            }

            string fileName = $"{memoryId}_{System.DateTime.UtcNow.Ticks}.jpg";
            string fullPath = Path.Combine(photoDir, fileName);

            await File.WriteAllBytesAsync(fullPath, photoBytes);
            Debug.Log($"[FileSystemHelper] Photo saved: {fullPath}");

            // Relative path döndür
            return Path.Combine("photos", fileName);
        }

        /// <summary>
        /// Ses dosyasını persistentDataPath/audio/ altına kaydet
        /// </summary>
        public static async Task<string> SaveAudioAsync(byte[] audioBytes, string memoryId)
        {
            string audioDir = Path.Combine(persistentDataPath, "audio");
            if (!Directory.Exists(audioDir))
            {
                Directory.CreateDirectory(audioDir);
            }

            string fileName = $"{memoryId}_{System.DateTime.UtcNow.Ticks}.m4a";
            string fullPath = Path.Combine(audioDir, fileName);

            await File.WriteAllBytesAsync(fullPath, audioBytes);
            Debug.Log($"[FileSystemHelper] Audio saved: {fullPath}");

            return Path.Combine("audio", fileName);
        }

        /// <summary>
        /// Relative path'i full path'e çevir
        /// </summary>
        public static string GetFullPath(string relativePath)
        {
            return Path.Combine(persistentDataPath, relativePath);
        }
    }
}
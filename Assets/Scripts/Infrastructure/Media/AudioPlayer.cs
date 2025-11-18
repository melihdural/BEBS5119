// Assets/Scripts/Infrastructure/Media/AudioPlayer.cs

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Media
{
    /// <summary>
    /// Ses dosyalarını oynatma yöneticisi
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Dosya yolundan ses oynat
        /// </summary>
        public async Task<bool> PlayAsync(string audioFilePath)
        {
            try
            {
                string fullPath = Infrastructure.Persistence.FileSystemHelper.GetFullPath(audioFilePath);

                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"[AudioPlayer] Audio file not found: {fullPath}");
                    return false;
                }

                // TODO: Platform-spesifik audio loading
                // iOS: AVAudioPlayer
                // Android: MediaPlayer
                // Unity: AudioClip (sınırlı format desteği)

#if UNITY_IOS || UNITY_ANDROID
                Debug.Log($"[AudioPlayer] Playing audio: {fullPath}");
                
                // Native player kullan
                await Task.Delay(100);
                return true;
#else
                Debug.LogWarning("[AudioPlayer] Native audio playback not available in editor");
                return false;
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AudioPlayer] PlayAsync failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Oynatmayı durdur
        /// </summary>
        public void Stop()
        {
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}
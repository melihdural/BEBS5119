// Assets/Scripts/Infrastructure/Media/AudioRecorderM4A.cs

using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Media
{
    /// <summary>
    /// Platform-spesifik ses kayıt implementasyonu (M4A/AAC formatı).
    /// iOS: AVAudioRecorder wrapper
    /// Android: MediaRecorder wrapper
    /// Editor: Simülasyon
    /// </summary>
    public class AudioRecorderM4A : MonoBehaviour, IAudioRecorder
    {
        public bool IsRecording { get; private set; }
        public float RecordingDuration { get; private set; }
        public float MaxDuration { get; set; } = 30f; // 30 saniye limit

        private float _recordingStartTime;

        public void StartRecording()
        {
            if (IsRecording)
            {
                Debug.LogWarning("[AudioRecorderM4A] Already recording");
                return;
            }

#if UNITY_IOS
            Debug.Log("[AudioRecorderM4A] Starting iOS audio recording");
            // TODO: iOS AVAudioRecorder native plugin çağrısı
#elif UNITY_ANDROID
            Debug.Log("[AudioRecorderM4A] Starting Android audio recording");
            // TODO: Android MediaRecorder native plugin çağrısı
#else
            Debug.Log("[AudioRecorderM4A] Starting simulated recording");
#endif

            IsRecording = true;
            _recordingStartTime = Time.time;
            RecordingDuration = 0f;
        }

        public async Task<byte[]> StopRecordingAsync()
        {
            if (!IsRecording)
            {
                Debug.LogWarning("[AudioRecorderM4A] Not recording");
                return null;
            }

            IsRecording = false;
            RecordingDuration = Time.time - _recordingStartTime;

            // İş kuralı: 30 saniye kontrolü
            if (RecordingDuration > MaxDuration)
            {
                Debug.LogWarning($"[AudioRecorderM4A] Recording exceeded max duration: {RecordingDuration}s");
            }

#if UNITY_IOS || UNITY_ANDROID
            Debug.Log($"[AudioRecorderM4A] Stopping recording, duration: {RecordingDuration}s");
            
            // TODO: Native plugin'den audio data al
            await Task.Delay(200);
            
            // Dummy data
            return new byte[1024 * 50]; // 50KB placeholder
#else
            Debug.Log("[AudioRecorderM4A] Simulated recording stopped");
            await Task.Delay(100);
            return new byte[1024 * 50];
#endif
        }

        private void Update()
        {
            if (IsRecording)
            {
                RecordingDuration = Time.time - _recordingStartTime;

                // Otomatik dur (30 saniye)
                if (RecordingDuration >= MaxDuration)
                {
                    Debug.Log("[AudioRecorderM4A] Max duration reached, auto-stopping");
                    _ = StopRecordingAsync();
                }
            }
        }

        /// <summary>
        /// Mikrofon izni kontrol et
        /// </summary>
        public bool HasMicrophonePermission()
        {
#if UNITY_ANDROID || UNITY_IOS
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
#else
            return true;
#endif
        }

        /// <summary>
        /// Mikrofon izni iste
        /// </summary>
        public async Task<bool> RequestMicrophonePermissionAsync()
        {
#if UNITY_ANDROID || UNITY_IOS
            var result = Application.RequestUserAuthorization(UserAuthorization.Microphone);
            
            // Unity'nin eski API'si - modern Android/iOS için native plugin gerekebilir
            while (!result.isDone)
            {
                await Task.Delay(100);
            }

            return HasMicrophonePermission();
#else
            return true;
#endif
        }
    }
}
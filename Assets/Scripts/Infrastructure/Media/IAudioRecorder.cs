// Assets/Scripts/Infrastructure/Media/IAudioRecorder.cs

using System.Threading.Tasks;

namespace Infrastructure.Media
{
    /// <summary>
    /// Ses kayıt arayüzü
    /// </summary>
    public interface IAudioRecorder
    {
        /// <summary>
        /// Ses kaydını başlat
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Kaydı durdur ve veriyi döndür
        /// </summary>
        Task<byte[]> StopRecordingAsync();

        /// <summary>
        /// Şu an kayıt yapılıyor mu
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Kayıt süresi (saniye)
        /// </summary>
        float RecordingDuration { get; }

        /// <summary>
        /// Maksimum kayıt süresi (saniye)
        /// </summary>
        float MaxDuration { get; set; }
    }
}
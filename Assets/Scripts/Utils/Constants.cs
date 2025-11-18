// Assets/Scripts/Utils/Constants.cs

namespace Utils
{
    /// <summary>
    /// Uygulama sabitleri
    /// </summary>
    public static class Constants
    {
        // Medya limitleri
        public const int MAX_PHOTO_SIZE_BYTES = 2 * 1024 * 1024;  // 2MB
        public const int MAX_AUDIO_SIZE_BYTES = 1536 * 1024;      // 1.5MB
        public const float MAX_AUDIO_DURATION_SECONDS = 30f;

        // Dosya yolları
        public const string MEMORIES_FILE = "memories.json";
        public const string ANCHORS_FILE = "anchors.json";
        public const string WORLDMAP_FILE = "ar_worldmap.dat";
        public const string PHOTOS_DIR = "photos";
        public const string AUDIO_DIR = "audio";

        // AR ayarları
        public const float PLACEMENT_INDICATOR_HEIGHT = 0.01f;
        public const float MEMORY_PREFAB_SCALE = 0.3f;

        // UI sabitleri
        public const float CARD_HEIGHT = 120f;
        public const int MAX_NOTE_PREVIEW_LENGTH = 50;
    }
}
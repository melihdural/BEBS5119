// Assets/Scripts/Utils/Logger.cs

using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Merkezi log yöneticisi. Debug/Release build'lerde farklı davranabilir.
    /// </summary>
    public static class Logger
    {
        private static bool _isDebugBuild = Debug.isDebugBuild;

        public static void Log(string message, Object context = null)
        {
            if (_isDebugBuild)
            {
                Debug.Log($"[ARMemoryGarden] {message}", context);
            }
        }

        public static void LogWarning(string message, Object context = null)
        {
            Debug.LogWarning($"[ARMemoryGarden] {message}", context);
        }

        public static void LogError(string message, Object context = null)
        {
            Debug.LogError($"[ARMemoryGarden] {message}", context);
        }

        public static void LogException(System.Exception exception, Object context = null)
        {
            Debug.LogException(exception, context);
        }
    }
}
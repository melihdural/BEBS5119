// Assets/Scripts/Infrastructure/AR/ARWorldMapManager.cs

using UnityEngine;
#if UNITY_IOS
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARFoundation;
#else
using System.Threading.Tasks;
#endif

namespace Infrastructure.AR
{
    /// <summary>
    /// iOS ARKit WorldMap kaydetme ve yükleme yöneticisi.
    /// Android için stub implementasyon (relocalization desteği yok).
    /// </summary>
    public class ARWorldMapManager : MonoBehaviour
    {
#if UNITY_IOS
        private string WorldMapFilePath => Path.Combine(Application.persistentDataPath, "ar_worldmap.dat");
        private ARSession _arSession;

        private void Awake()
        {
            _arSession = FindObjectOfType<ARSession>();
        }

        /// <summary>
        /// Mevcut AR session'ın world map'ini kaydet
        /// </summary>
        public async Task<bool> SaveWorldMapAsync()
        {
            if (_arSession == null)
            {
                Debug.LogError("[ARWorldMapManager] AR Session not found");
                return false;
            }

            var sessionSubsystem = _arSession.subsystem as ARKitSessionSubsystem;
            if (sessionSubsystem == null)
            {
                Debug.LogError("[ARWorldMapManager] ARKit session subsystem not available");
                return false;
            }

            try
            {
                var worldMapRequest = sessionSubsystem.GetARWorldMapAsync();

                // ARAsyncCancellableRequest tamamlanana kadar bekle
                while (!worldMapRequest.status.IsComplete())
                {
                    await Task.Yield();
                }

                if (worldMapRequest.status.IsError())
                {
                    Debug.LogError($"[ARWorldMapManager] Failed to get world map: {worldMapRequest.status}");
                    worldMapRequest.Dispose();
                    return false;
                }

                var worldMap = worldMapRequest.GetWorldMap();
                worldMapRequest.Dispose();
                
                if (worldMap.valid)
                {
                    // Serialize ve kaydet
                    byte[] serializedData = worldMap.Serialize();
                    await File.WriteAllBytesAsync(WorldMapFilePath, serializedData);
                    worldMap.Dispose();

                    Debug.Log($"[ARWorldMapManager] World map saved: {WorldMapFilePath}");
                    return true;
                }
                else
                {
                    Debug.LogError("[ARWorldMapManager] World map is not valid");
                    worldMap.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ARWorldMapManager] SaveWorldMapAsync failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kaydedilmiş world map'i yükle ve AR session'a uygula
        /// </summary>
        public async Task<bool> LoadWorldMapAsync()
        {
            if (!File.Exists(WorldMapFilePath))
            {
                Debug.LogWarning("[ARWorldMapManager] No saved world map found");
                return false;
            }

            var sessionSubsystem = _arSession.subsystem as ARKitSessionSubsystem;
            if (sessionSubsystem == null)
            {
                Debug.LogError("[ARWorldMapManager] ARKit session subsystem not available");
                return false;
            }

            try
            {
                byte[] worldMapData = await File.ReadAllBytesAsync(WorldMapFilePath);
                
                // Deserialize
                if (!ARWorldMap.TryDeserialize(worldMapData, out ARWorldMap worldMap))
                {
                    Debug.LogError("[ARWorldMapManager] Failed to deserialize world map");
                    return false;
                }
                
                if (worldMap.valid)
                {
                    // World map'i session'a uygula
                    var config = new ARKitSessionConfiguration
                    {
                        worldMap = worldMap
                    };
                    
                    sessionSubsystem.RunWithConfiguration(config, ARKitSessionRunOptions.ResetTracking);
                    worldMap.Dispose();

                    Debug.Log("[ARWorldMapManager] World map loaded and applied");
                    return true;
                }
                else
                {
                    Debug.LogError("[ARWorldMapManager] Deserialized world map is not valid");
                    worldMap.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ARWorldMapManager] LoadWorldMapAsync failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kaydedilmiş world map var mı kontrol et
        /// </summary>
        public bool HasSavedWorldMap()
        {
            return File.Exists(WorldMapFilePath);
        }

#else
        // Android için stub implementasyonlar
        public Task<bool> SaveWorldMapAsync()
        {
            Debug.LogWarning("[ARWorldMapManager] World map saving not supported on this platform");
            return Task.FromResult(false);
        }

        public Task<bool> LoadWorldMapAsync()
        {
            Debug.LogWarning("[ARWorldMapManager] World map loading not supported on this platform");
            return Task.FromResult(false);
        }

        public bool HasSavedWorldMap()
        {
            return false;
        }
#endif
    }
}
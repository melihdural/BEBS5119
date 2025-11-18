// Assets/Scripts/Infrastructure/AppBootstrap.cs

using System.Collections;
using Infrastructure.AR;
using Infrastructure.Persistence;
using UnityEngine;
using Utils;

/// <summary>
/// Uygulama başlatıcı. İzinleri kontrol eder, service'leri başlatır.
/// MainAR sahnesinde bir GameObject'e eklenmeli.
/// </summary>
public class AppBootstrap : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _permissionDeniedPanel;

    private void Start()
    {
        StartCoroutine(InitializeApp());
    }

    private IEnumerator InitializeApp()
    {
        // Loading panel'i göster
        if (_loadingPanel != null)
            _loadingPanel.SetActive(true);

        if (_permissionDeniedPanel != null)
            _permissionDeniedPanel.SetActive(false);

        Debug.Log("[AppBootstrap] Starting app initialization...");

        // Service locator'ı başlat
        var services = ServiceLocator.Instance;
        Debug.Log("[AppBootstrap] Services initialized");

        yield return new WaitForSeconds(0.5f); // Kısa bekleme

        // İzinleri kontrol et
        var permissionTask = PermissionHelper.RequestAllPermissionsAsync();
        yield return new WaitUntil(() => permissionTask.IsCompleted);

        bool hasPermissions = permissionTask.Result;

        if (!hasPermissions)
        {
            Debug.LogError("[AppBootstrap] Required permissions denied");
            if (_loadingPanel != null)
                _loadingPanel.SetActive(false);
            if (_permissionDeniedPanel != null)
                _permissionDeniedPanel.SetActive(true);
            yield break;
        }

        Debug.Log("[AppBootstrap] Permissions granted");

        // Relocalization dene (iOS)
        var worldMapManager = FindObjectOfType<ARWorldMapManager>();
        if (worldMapManager != null && worldMapManager.HasSavedWorldMap())
        {
            Debug.Log("[AppBootstrap] Loading saved world map...");
            var loadTask = worldMapManager.LoadWorldMapAsync();
            yield return new WaitUntil(() => loadTask.IsCompleted);

            if (loadTask.Result)
            {
                Debug.Log("[AppBootstrap] World map loaded successfully");
            }
            else
            {
                Debug.LogWarning("[AppBootstrap] Failed to load world map");
            }
        }
        else
        {
            Debug.Log("[AppBootstrap] No saved world map found or not supported");
        }

        // Loading panel'i kapat
        if (_loadingPanel != null)
            _loadingPanel.SetActive(false);

        Debug.Log("[AppBootstrap] App initialization complete");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // Uygulama arka plana alınıyor - world map kaydet (iOS)
            Debug.Log("[AppBootstrap] App pausing, saving world map...");
            var worldMapManager = FindObjectOfType<ARWorldMapManager>();
            if (worldMapManager != null)
            {
                _ = worldMapManager.SaveWorldMapAsync();
            }
        }
        else
        {
            Debug.Log("[AppBootstrap] App resumed");
        }
    }

    private void OnApplicationQuit()
    {
        // Uygulama kapanıyor - world map kaydet (iOS)
        Debug.Log("[AppBootstrap] App quitting, saving world map...");
        var worldMapManager = FindObjectOfType<ARWorldMapManager>();
        if (worldMapManager != null)
        {
            // Synchronous olarak kaydet (quit sırasında async çalışmayabilir)
            _ = worldMapManager.SaveWorldMapAsync();
        }
    }
}
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// AR görüntü takibi ile prefab oluşturma işlemlerini yöneten sınıf
/// </summary>
public class PrefabCreator : MonoBehaviour
{
    // Inspector'dan atanacak instantiate edilecek prefab referansı
    [SerializeField] private GameObject prefabToInstantiate;
    
    // Prefab'in görüntüye göre konumlandırma offset değeri
    [SerializeField] private Vector3 prefabOffset;

    // Oluşturulan prefab'in referansı
    private GameObject prefab;
    
    // AR görüntü takip yöneticisi
    private ARTrackedImageManager arTrackedImageManager;

    /// <summary>
    /// Component aktif olduğunda çağrılır
    /// </summary>
    private void OnEnable()
    {
        // ARTrackedImageManager bileşenini al
        arTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();

        if (arTrackedImageManager == null)
        {
            Debug.LogError("PrefabCreator: ARTrackedImageManager component not found!");
            return;
        }

        // Görüntü değişikliklerini dinlemeye başla
        arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        Debug.Log("PrefabCreator: Listening for tracked images.");
    }

    private void OnDisable()
    {
        if (arTrackedImageManager != null)
        {
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    /// <summary>
    /// Takip edilen görüntülerde değişiklik olduğunda çağrılır
    /// </summary>
    /// <param name="obj">Değişen görüntü bilgileri</param>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // Yeni tespit edilen her görüntü için döngü
        foreach (ARTrackedImage image in obj.added)
        {
            Debug.Log($"PrefabCreator: Image added: {image.referenceImage.name}");
            UpdatePrefab(image);
        }

        foreach (ARTrackedImage image in obj.updated)
        {
            UpdatePrefab(image);
        }

        foreach (ARTrackedImage image in obj.removed)
        {
            Debug.Log($"PrefabCreator: Image removed: {image.referenceImage.name}");
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }
    }

    private void UpdatePrefab(ARTrackedImage image)
    {
        if (prefab == null)
        {
             // Prefab'i görüntünün transform'una göre oluştur
            prefab = Instantiate(prefabToInstantiate, image.transform);
            // Prefab'in pozisyonunu offset değeri kadar kaydır
            prefab.transform.position += prefabOffset;
            Debug.Log("PrefabCreator: Prefab instantiated.");
        }
        else
        {
            // Prefab'in image'dan ne kadar uzakta olduğunu kontrol et
            float distance = Vector3.Distance(prefab.transform.position, image.transform.position + prefabOffset);
            
            if (distance > 100.0f)
            {
                Debug.Log($"PrefabCreator: Prefab too far from image ({distance}m), resetting.");
                Destroy(prefab);
                prefab = Instantiate(prefabToInstantiate, image.transform);
                prefab.transform.position += prefabOffset;
            }
            // Hareketi engellediği için sürekli pozisyon güncellemeyi kapattık.
        }

        // Tracking state'e göre görünürlüğü ayarla
        // Limited state'de de gösterelim ki titreme olmasın (özellikle ekrandan taratırken)
        bool isVisible = image.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking ||
                         image.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited;
        prefab.SetActive(isVisible);
    }
}
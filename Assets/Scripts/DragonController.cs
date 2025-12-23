using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Ejderha karakterinin joystick ile kontrolünü sağlar.
/// Kamera yönüne göre hareket eder ve hareket yönüne doğru döner.
/// </summary>
public class DragonController : MonoBehaviour
{
    /// <summary>
    /// Ejderhanın hareket hızı
    /// </summary>
    [SerializeField] private float speed;

    /// <summary>
    /// Joystick referansı (Base Joystick sınıfı kullanılarak tüm türler desteklenir)
    /// </summary>
    private Joystick joystick;

    /// <summary>
    /// Ejderhanın Rigidbody bileşeni
    /// </summary>
    [SerializeField] private Rigidbody rb;
    
    [Header("Abilities")]
    [SerializeField] private ParticleSystem fireParticleSystem;
    [SerializeField] private Transform mouthPoint;

    [SerializeField] private Renderer renderer;
    

    private static DragonController Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        // Rigidbody referansı atanmamışsa otomatik olarak al
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // Android için kritik Rigidbody ayarları
        rb.isKinematic = false; // Fizik motorunun objeyi etkilemesine izin ver
        rb.useGravity = false;  // Yerçekimini devre dışı bırak (havada hareket için)
        
        // Rotasyon ve pozisyon kısıtlamaları (X ve Z rotasyonunu, Y pozisyonunu sabitle)
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Daha yumuşak hareket için interpolasyon
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Hızlı hareket eden objeler için sürekli çarpışma kontrolü

        // UI Butonlarını Bul ve Bağla
        // Kullanıcı "FireButtonObj" tag'ini tercih etti, önce ona bakıyoruz.
        GameObject fireButtonObj = null;
        try {
            fireButtonObj = GameObject.FindGameObjectWithTag("FireButtonObj");
        } catch {}

        // Bulunamazsa isme göre arayalım (Fallback)
        if (fireButtonObj == null) fireButtonObj = GameObject.Find("FireButton");

        if (fireButtonObj != null)
        {
            SetupHoldButton(fireButtonObj, BreatheFire, StoppingFire);
        }

        // --- NEW: Apply Default Material ---
        // Sahne açıldığında materyal atanmamış olabilir, menüden varsayılanı alalım.
        MaterialMenuController menuController = FindObjectOfType<MaterialMenuController>();
        if (menuController != null)
        {
            Material defaultMat = menuController.GetDefaultMaterial();
            if (defaultMat != null)
            {
                SetMaterial(defaultMat);
            }
        }
    }

    /// <summary>
    /// Bir butona basılı tutma (Hold) özelliği ekler.
    /// EventTrigger bileşenini kullanır.
    /// </summary>
    private void SetupHoldButton(GameObject obj, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = obj.AddComponent<EventTrigger>();

        // Pointer Down (Basma)
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => { onDown(); });
        trigger.triggers.Add(entryDown);

        // Pointer Up (Bırakma)
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => { onUp(); });
        trigger.triggers.Add(entryUp);
    }

    /// <summary>
    /// Obje aktif olduğunda joystick referansını alır
    /// </summary>
    private void OnEnable()
    {
        joystick = FindObjectOfType<Joystick>();
    }

    /// <summary>
    /// Fizik güncellemelerinde ejderhanın hareketini kontrol eder
    /// </summary>
    private void FixedUpdate()
    {
        // Joystick referansının geçerli olup olmadığını kontrol et
        if (joystick == null)
        {
             // Joystick'i sahnede bulmaya çalış
             joystick = FindObjectOfType<Joystick>();
             if (joystick == null)
             {
                return;
             }
        }

        // Joystick'ten yatay ve dikey girdileri al
        float xValue = joystick.Horizontal;
        float yValue = joystick.Vertical;

        // Eğer joystick hareket ettirilmiyorsa
        if (xValue == 0 && yValue == 0)
        {
            // Ejderhayı durdur
            rb.velocity = Vector3.zero;
            return;
        }

        // Ana kamerayı al
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            // Ana kamera bulunamazsa sahnedeki kamerayı bul
            mainCamera = FindObjectOfType<Camera>();
            if (mainCamera == null)
            {
                #if UNITY_ANDROID
                Debug.LogError("[ANDROID] Camera not found!");
                #endif
                return;
            }
        }

        // Kameranın transform bilgilerini al
        Transform cameraTransform = mainCamera.transform;
        Vector3 cameraForward = cameraTransform.forward; // Kameranın ileri yönü
        Vector3 cameraRight = cameraTransform.right;     // Kameranın sağ yönü

        // Y eksenini sıfırla (yatay düzlemde hareket için)
        cameraForward.y = 0;
        cameraRight.y = 0;
        
        // Vektörleri normalize et (birim vektör yap) - uzunluk değil yön önemli
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Kamera yönüne göre hareket vektörünü hesapla 
        Vector3 movement = (cameraForward * yValue + cameraRight * xValue);

        // Yeni pozisyonu hesapla (mevcut pozisyon + hareket * hız * delta time)
        Vector3 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;

        // Rigidbody'yi yeni pozisyona taşı
        rb.MovePosition(newPosition);

        // Eğer hareket varsa, ejderhayı hareket yönüne döndür
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }
    
    private Coroutine stopFireCoroutine;

    /// <summary>
    /// Ateş püskürtmeyi başlatır (Basılı tutunca)
    /// </summary>
    public void BreatheFire()
    {
        if (fireParticleSystem != null)
        {
            // Eğer durdurma işlemi devam ediyorsa iptal et
            if (stopFireCoroutine != null) 
            {
                StopCoroutine(stopFireCoroutine);
                stopFireCoroutine = null;
            }

            fireParticleSystem.gameObject.SetActive(true);
            
            // Zaten oynuyorsa tekrar Play demeye gerek yok, akışı bozmamak için
            if (!fireParticleSystem.isPlaying)
                fireParticleSystem.Play();
        }
    }

    /// <summary>
    /// Ateş püskürtmeyi durdurur (Bırakınca).
    /// Particle'ların doğal olarak sönmesini bekler, sonra objeyi kapatır.
    /// </summary>
    public void StoppingFire()
    {
        if (fireParticleSystem != null)
        {
            // Yeni partikül oluşumunu durdur (Mevcutlar yaşamaya devam eder)
            fireParticleSystem.Stop();
            
            // Partiküller tamamen kaybolunca objeyi kapatmak için bekle
            if (stopFireCoroutine != null) StopCoroutine(stopFireCoroutine);
            stopFireCoroutine = StartCoroutine(DisableFireRoutine());
        }
    }

    private System.Collections.IEnumerator DisableFireRoutine()
    {
        // Partiküllerin hepsi sönene kadar bekle
        while (fireParticleSystem.IsAlive(true))
        {
            yield return new WaitForSeconds(0.2f);
        }

        fireParticleSystem.gameObject.SetActive(false);
        stopFireCoroutine = null;
    }

    public void SetMaterial(Material mat)
    {
        // Sadece ana obje değil, alt objelerdeki Renderer'ları da etkilemeli
        renderer.material = mat;
    }
}

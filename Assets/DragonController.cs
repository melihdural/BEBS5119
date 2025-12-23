using UnityEngine;

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
    [SerializeField] private Renderer dragonRenderer;
    [SerializeField] private Transform mouthPoint;

    public static DragonController Instance { get; private set; }

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

        // Renderer referansı atanmamışsa
        if (dragonRenderer == null)
            dragonRenderer = GetComponentInChildren<Renderer>();

        // Ateş efekti yoksa oluştur
        if (fireParticleSystem == null)
        {
            // Çocuk objelerde ara
            fireParticleSystem = GetComponentInChildren<ParticleSystem>();
            
            // Hala yoksa basit bir tane oluştur
            if (fireParticleSystem == null)
            {
                CreateFallbackFireEffect();
            }
        }

        // Android için kritik Rigidbody ayarları
        rb.isKinematic = false; // Fizik motorunun objeyi etkilemesine izin ver
        rb.useGravity = false;  // Yerçekimini devre dışı bırak (havada hareket için)
        
        // Rotasyon ve pozisyon kısıtlamaları (X ve Z rotasyonunu, Y pozisyonunu sabitle)
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Daha yumuşak hareket için interpolasyon
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Hızlı hareket eden objeler için sürekli çarpışma kontrolü
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
    /// <summary>
    /// Ejderha ateş püskürtür
    /// </summary>
    public void BreatheFire()
    {
        if (fireParticleSystem != null)
        {
            if (fireParticleSystem.isPlaying)
                fireParticleSystem.Stop();
                
            fireParticleSystem.Play();
        }
    }

    /// <summary>
    /// Ejderhanın rengini değiştirir
    /// </summary>
    /// <param name="color">Yeni renk</param>
    public void SetColor(Color color)
    {
        if (dragonRenderer != null)
        {
            dragonRenderer.material.color = color;
        }
    }

    /// <summary>
    /// Basit bir ateş efekti oluşturur (Asset atanmamışsa)
    /// </summary>
    private void CreateFallbackFireEffect()
    {
        GameObject fireObj = new GameObject("ProceduralFireEffect");
        fireObj.transform.SetParent(this.transform);
        
        // Ağız pozisyonu varsa oraya, yoksa biraz ileriye ve yukarıya koy
        if (mouthPoint != null)
        {
            fireObj.transform.position = mouthPoint.position;
            fireObj.transform.rotation = mouthPoint.rotation;
        }
        else
        {
            fireObj.transform.localPosition = new Vector3(0, 1.5f, 1.0f); // Tahmini ağız konumu
            fireObj.transform.localRotation = Quaternion.identity;
        }

        fireParticleSystem = fireObj.AddComponent<ParticleSystem>();
        
        // Ateş benzeri ayarlar
        var main = fireParticleSystem.main;
        main.startLifetime = 1.5f;
        main.startSpeed = 5f;
        main.startSize = 0.5f;
        main.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.yellow);
        main.loop = false;
        main.playOnAwake = false;
        
        var emission = fireParticleSystem.emission;
        emission.rateOverTime = 20;

        var shape = fireParticleSystem.shape;
        shape.angle = 25f;
        
        // Renderer ayarları
        var renderer = fireObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
    }
}

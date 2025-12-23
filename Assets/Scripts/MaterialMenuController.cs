using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro kullanıyorsa diye, standard Text için UnityEngine.UI yeterli.

public class MaterialMenuController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Material> materials;
    [SerializeField] private List<Color> colors;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button toggleButton;
    
    [Header("List Generation")]
    [SerializeField] private Transform listContentContainer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Vector2 gridCellSize = new Vector2(200, 200);
    [SerializeField] private Vector2 gridSpacing = new Vector2(100, 100);

    private bool isMenuOpen = false;

    private void Start()
    {
        // Panel başlangıçta kapalı olsun
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Toggle butonu bağla
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleMenu);
        }

        SetupLayout();
        GenerateList();
    }

    private void SetupLayout()
    {
        if (listContentContainer == null) return;

        // Ensure GridLayoutGroup
        GridLayoutGroup grid = listContentContainer.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = listContentContainer.gameObject.AddComponent<GridLayoutGroup>();
        }

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.cellSize = gridCellSize;
        grid.spacing = gridSpacing;
        grid.childAlignment = TextAnchor.UpperCenter; // Buttonları ortala

        // Scroll rect için Pivot ve Anchor ayarlarını düzelt (Yukarıdan başlasın)
        RectTransform rect = listContentContainer.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.pivot = new Vector2(0.5f, 1f); // Pivot üst orta
            // Anchor'ları da üstten uzayacak şekilde ayarlayabiliriz veya parent'a bırakabiliriz
            // Genellikle scroll content için pivot (0.5, 1) yeterlidir.
            
            // Pozisyonu resetle (özellikle y 0 olmalı ki en tepeden başlasın)
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
        }

        // Ensure ContentSizeFitter for scrolling
        ContentSizeFitter fitter = listContentContainer.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = listContentContainer.gameObject.AddComponent<ContentSizeFitter>();
        }
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        // Horizontal fit usually Unconstrained for vertical lists or MinSize/Preferred depending on need.
        // Unconstrained is default which is fine if width is controlled by parent width.

        // --- NEW: Ensure Parent has ScrollRect ---
        if (listContentContainer.parent != null)
        {
            GameObject parentObj = listContentContainer.parent.gameObject;
            ScrollRect scrollRect = parentObj.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                scrollRect = parentObj.AddComponent<ScrollRect>();
                scrollRect.movementType = ScrollRect.MovementType.Elastic;
                scrollRect.scrollSensitivity = 20f;
                scrollRect.horizontal = false; // Vertical list, usually
                scrollRect.vertical = true;
                
                // Needs an Image for raycast if it doesn't have one
                if (parentObj.GetComponent<Image>() == null)
                {
                    Image img = parentObj.AddComponent<Image>();
                    img.color = new Color(0, 0, 0, 0.5f); // Semi-transparent or Color.clear
                }
            }

            // Assign Content
            if (scrollRect.content == null)
            {
                scrollRect.content = listContentContainer.GetComponent<RectTransform>();
            }

            // Ensure Clipping (Mask)
            if (parentObj.GetComponent<Mask>() == null && parentObj.GetComponent<RectMask2D>() == null)
            {
                parentObj.AddComponent<RectMask2D>();
            }
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);
    }

    private void GenerateList()
    {
        if (listContentContainer == null || buttonPrefab == null)
        {
            Debug.LogError($"MaterialMenuController: Missing references! Container: {listContentContainer}, Prefab: {buttonPrefab}");
            return;
        }

        if (materials == null || materials.Count == 0)
        {
            Debug.LogWarning("MaterialMenuController: Materials list is empty or null!");
            return;
        }

        foreach (var mat in materials)
        {
            if (mat == null) 
            {
                Debug.LogWarning("MaterialMenuController: Found null material in list.");
                continue;
            }

            GameObject btnObj = Instantiate(buttonPrefab, listContentContainer);
            if (btnObj == null)
            {
                Debug.LogError("MaterialMenuController: Failed to instantiate button!");
                continue;
            }
            
            btnObj.SetActive(true);
            
            // Buton üzerindeki Text'i ayarla (Varsa)
            var textComp = btnObj.GetComponentInChildren<Text>();
            if (textComp != null) textComp.text = mat.name;
            
            var tmpText = btnObj.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) tmpText.text = mat.name;

            // Tıklama olayını bağla
            Button btn = btnObj.GetComponent<Button>();
            var indexOfMat = materials.IndexOf(mat);
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnMaterialSelected(mat));
                var btnImg = btnObj.GetComponent<Image>();
                if (btnImg != null && colors != null && indexOfMat < colors.Count)
                {
                    btnImg.color = colors[indexOfMat];
                }
            }
        }
    }

    private void OnMaterialSelected(Material mat)
    {
        // DragonController instance'ına ulaş
        // Singleton pattern kullanıyorsa Instance property'den, yoksa FindObjectOfType
        DragonController dragon = FindObjectOfType<DragonController>(); 
        
        if (dragon != null)
        {
            dragon.SetMaterial(mat);
        }
        else
        {
            Debug.LogError("DragonController not found in scene!");
        }

        // Seçimden sonra menüyü kapatmak isterseniz:
        ToggleMenu();
    }

    public Material GetDefaultMaterial()
    {
        if (materials != null && materials.Count > 0)
        {
            return materials[0];
        }
        return null;
    }
}

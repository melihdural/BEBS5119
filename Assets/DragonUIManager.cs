using UnityEngine;
using UnityEngine.UI;

public class DragonUIManager : MonoBehaviour
{
    [SerializeField] private Font uiFont;

    private GameObject canvasObj;
    private GameObject colorPanel;

    private void Start()
    {
        // Try to find a font if not assigned
        if (uiFont == null)
        {
            uiFont = Font.CreateDynamicFontFromOSFont("Arial", 24);
        }

        SetupUI();
    }

    private void SetupUI()
    {
        // 1. Check/Create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            canvasObj = new GameObject("DragonCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        else
        {
            canvasObj = canvas.gameObject;
        }

        // 2. Create Fire Button (Bottom Right)
        // Position: Anchored to Bottom Right (1,0)
        CreateButton("FireButton", "FIRE!", new Vector2(160, 80), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-100, 100), FireAction);

        // 3. Create Color Button (Top Right)
        // Position: Anchored to Top Right (1,1)
        CreateButton("ColorButton", "COLOR", new Vector2(160, 80), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-100, -100), ToggleColorPanel);

        // 4. Create Color Panel (Center, initially hidden)
        CreateColorPanel();
    }

    private void CreateButton(string name, string text, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        Button btn = buttonObj.AddComponent<Button>();
        btn.onClick.AddListener(action);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text txt = textObj.AddComponent<Text>();
        txt.text = text;
        txt.font = uiFont;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = 24;
    }

    private void CreateColorPanel()
    {
        colorPanel = new GameObject("ColorPickerPanel");
        colorPanel.transform.SetParent(canvasObj.transform, false);
        
        RectTransform rect = colorPanel.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 400);
        rect.anchoredPosition = Vector2.zero; // Center

        Image img = colorPanel.AddComponent<Image>();
        img.color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
        
        // Layout Group
        VerticalLayoutGroup layout = colorPanel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 20;
        layout.padding = new RectOffset(40, 40, 40, 40);

        // Add Header
        GameObject headerObj = new GameObject("Header");
        headerObj.transform.SetParent(colorPanel.transform, false);
        LayoutElement leHeader = headerObj.AddComponent<LayoutElement>();
        leHeader.minHeight = 40;
        Text headerTxt = headerObj.AddComponent<Text>();
        headerTxt.text = "SELECT COLOR";
        headerTxt.font = uiFont;
        headerTxt.alignment = TextAnchor.MiddleCenter;
        headerTxt.color = Color.white;
        headerTxt.fontSize = 20;

        // Add Color Option Buttons
        CreateColorOption("Red", Color.red);
        CreateColorOption("Green", Color.green);
        CreateColorOption("Blue", Color.blue);
        CreateColorOption("Yellow", Color.yellow);
        CreateColorOption("Magenta", Color.magenta);
        
        // Close Button
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(colorPanel.transform, false);
        LayoutElement le = closeBtn.AddComponent<LayoutElement>();
        le.minHeight = 50;
        
        Image closeImg = closeBtn.AddComponent<Image>();
        closeImg.color = Color.red; // Red close button
        Button btn = closeBtn.AddComponent<Button>();
        btn.onClick.AddListener(() => colorPanel.SetActive(false));
         
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(closeBtn.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text txt = textObj.AddComponent<Text>();
        txt.text = "CLOSE";
        txt.font = uiFont;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white; 
        txt.fontSize = 20;

        colorPanel.SetActive(false); // Hide initially
    }

    private void CreateColorOption(string name, Color color)
    {
        GameObject btnObj = new GameObject(name + "Button");
        btnObj.transform.SetParent(colorPanel.transform, false);
        
        LayoutElement le = btnObj.AddComponent<LayoutElement>();
        le.minHeight = 50;
        le.preferredWidth = 300;

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() => SetDragonColor(color));
    }

    // Actions
    private void FireAction()
    {
        if (DragonController.Instance != null)
        {
            DragonController.Instance.BreatheFire();
        }
        else
        {
            // Fallback find
            DragonController controller = FindObjectOfType<DragonController>();
            if (controller != null) controller.BreatheFire();
        }
    }

    private void ToggleColorPanel()
    {
        if (colorPanel != null)
        {
            colorPanel.SetActive(!colorPanel.activeSelf);
        }
    }

    private void SetDragonColor(Color color)
    {
        if (DragonController.Instance != null)
        {
            DragonController.Instance.SetColor(color);
        }
        else
        {
             DragonController controller = FindObjectOfType<DragonController>();
             if (controller != null) controller.SetColor(color);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates the shop popup UI at runtime. Add this to an empty GameObject (optionally with ShopPopup for custom settings).
/// It will add a Canvas, background (UI panel or 3D Plane), title, and four buttons and wire them to ShopPopup.
/// The popup starts disabled; show it by calling ShopPopup.ShowPopup() when the user clicks the medieval shop.
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class ShopPopupUIBuilder : MonoBehaviour
{
    [Header("Optional: add ShopPopup first to customize costs/item names; otherwise defaults are used.")]
    [Tooltip("Leave unchecked to start with popup hidden (recommended).")]
    public bool startVisible;

    [Header("Background")]
    [Tooltip("Use a 3D Plane/Quad as background instead of a UI Image. Uses World Space canvas so the plane appears in the scene.")]
    public bool usePlaneForBackground = true;
    [Tooltip("World-space size of the canvas and plane when using Plane background (width, height).")]
    public Vector2 planeSize = new Vector2(2f, 2.2f);
    [Tooltip("Color of the plane material when using Plane background.")]
    public Color planeColor = new Color(0.15f, 0.12f, 0.2f, 0.95f);

    void Awake()
    {
        if (GetComponentInChildren<Button>() != null)
            return; // already built

        EnsureCanvas();
        BuildUI();
        gameObject.SetActive(startVisible);
    }

    void EnsureCanvas()
    {
        var canvas = GetComponent<Canvas>();
        var scaler = GetComponent<CanvasScaler>();

        if (usePlaneForBackground)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = 100;
            var rect = GetComponent<RectTransform>();
            rect.sizeDelta = planeSize;
            rect.localScale = Vector3.one;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.dynamicPixelsPerUnit = 100f;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
    }

    void BuildUI()
    {
        var shopPopup = GetComponent<ShopPopup>();
        if (shopPopup == null)
            shopPopup = gameObject.AddComponent<ShopPopup>();

        Transform contentParent;

        if (usePlaneForBackground)
        {
            // 3D Quad as background (behind the UI)
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "BackgroundPlane";
            quad.transform.SetParent(transform, false);
            quad.transform.localPosition = new Vector3(0, 0, -0.05f);
            quad.transform.localRotation = Quaternion.identity;
            quad.transform.localScale = new Vector3(planeSize.x, planeSize.y, 1f);
            var renderer = quad.GetComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Unlit/Color"));
            renderer.material.color = planeColor;
            Object.Destroy(quad.GetComponent<Collider>()); // no need for raycast on background

            contentParent = transform;
        }
        else
        {
            // Full-screen semi-transparent UI panel
            var panel = CreatePanel("Panel");
            panel.transform.SetParent(transform, false);
            SetFullStretch(panel.GetComponent<RectTransform>());
            var panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.92f);
            contentParent = panel.transform;
        }

        // Centered content area
        var content = new GameObject("Content");
        content.transform.SetParent(contentParent, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(400, 420);
        contentRect.anchoredPosition = Vector2.zero;

        var contentImage = content.AddComponent<Image>();
        contentImage.color = new Color(0.2f, 0.2f, 0.25f, 0.98f);

        var vertical = content.AddComponent<VerticalLayoutGroup>();
        vertical.spacing = 12f;
        vertical.padding = new RectOffset(24, 24, 24, 24);
        vertical.childAlignment = TextAnchor.UpperCenter;
        vertical.childControlWidth = true;
        vertical.childControlHeight = false;
        vertical.childForceExpandWidth = true;

        // Title
        var title = CreateText("Shop", 28, content.transform);
        var titleLayout = title.GetComponent<LayoutElement>();
        titleLayout.minHeight = 44;
        titleLayout.preferredHeight = 44;

        // Spacer
        var spacer = new GameObject("Spacer");
        spacer.transform.SetParent(content.transform, false);
        spacer.AddComponent<LayoutElement>().minHeight = 8;

        // Buttons
        CreateButton("Watering Can (5 Sunlight)", () => shopPopup.BuyWateringCan(), content.transform);
        CreateButton("Fertilizer (5 Coins)", () => shopPopup.BuyFertilizer(), content.transform);
        CreateButton("Powerup (15 Sunlight + 15 Coins)", () => shopPopup.BuyPowerup(), content.transform);

        var closeSpacer = new GameObject("Spacer2");
        closeSpacer.transform.SetParent(content.transform, false);
        closeSpacer.AddComponent<LayoutElement>().minHeight = 16;

        var closeBtn = CreateButton("Close", () => shopPopup.HidePopup(), content.transform);
        var closeColors = closeBtn.colors;
        closeColors.normalColor = new Color(0.5f, 0.25f, 0.25f);
        closeBtn.colors = closeColors;
    }

    static GameObject CreatePanel(string name)
    {
        var go = new GameObject(name);
        go.AddComponent<CanvasRenderer>();
        var image = go.AddComponent<Image>();
        image.color = Color.white;
        return go;
    }

    static GameObject CreateText(string text, int fontSize, Transform parent)
    {
        var go = new GameObject("Text");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 40);

        var t = go.AddComponent<Text>();
        t.text = text;
        t.fontSize = fontSize;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;

        return go;
    }

    static Button CreateButton(string label, UnityEngine.Events.UnityAction onClick, Transform parent)
    {
        var go = new GameObject("Button_" + label);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(320, 44);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.25f, 0.4f, 0.25f);

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = image;
        btn.onClick.AddListener(onClick);

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var textRect = textGo.AddComponent<RectTransform>();
        SetFullStretch(textRect);
        var text = textGo.AddComponent<Text>();
        text.text = label;
        text.fontSize = 18;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return btn;
    }

    static void SetFullStretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}

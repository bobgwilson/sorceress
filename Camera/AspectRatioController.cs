using UnityEngine;

/// <summary>
/// This camera script maintains a fixed aspect ratio by adding black bars (letterboxing or pillarboxing).
/// </summary>
[ExecuteInEditMode]
public class AspectRatioController : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16f / 9f;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private Camera cam;
    
    /// <summary>
    /// Initializes the camera reference and sets the background to black.
    /// </summary>
    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = Color.black;  // ensures black behind viewport
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    /// <summary>
    /// Sets the initial aspect ratio and stores the screen size.
    /// </summary>
    void Start()
    {
        UpdateAspect();
    }

    /// <summary>
    /// Updates the aspect ratio if the screen size changes.
    /// </summary>
    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) UpdateAspect();
    }

    /// <summary>
    /// Clears the entire screen to black before rendering, fixing pillarbox color issues.
    /// </summary>
    void OnPreCull()
    {
        GL.Clear(true, true, Color.black);
    }
    
    /// <summary>
    /// Adjusts the camera viewport to maintain the target aspect ratio by adding letterboxing or pillarboxing.
    /// </summary>
    void UpdateAspect()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            // Letterbox (add black bars on top and bottom)
            cam.rect = new Rect(0, (1f - scaleHeight) / 2f, 1f, scaleHeight);
        }
        else
        {
            // Pillarbox (add black bars on the sides)
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) / 2f, 0, scaleWidth, 1f);
        }
        
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}

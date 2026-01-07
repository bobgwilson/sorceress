using UnityEngine;

/// <summary>
/// Handles tinting the color of an enemy's sprite.
/// Be sure to set the material on the SpriteRenderer to SpriteColorizeHSL-Material.
/// </summary>
public class SpriteTinter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Sets the blend value for the sprite's material, controlling the tint intensity.
    /// </summary>
    private float _blendValue;
    public float BlendValue
    {
        set
        {
            _blendValue = value;
            spriteRenderer.material.SetFloat("_Blend", _blendValue);
        }
    }
    
    /// <summary>
    /// Initializes the SpriteRenderer and assigns a unique material instance for this enemy.
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material = new Material(spriteRenderer.material); // unique material per enemy
    }
}

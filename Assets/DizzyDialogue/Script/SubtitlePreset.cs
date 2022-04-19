using UnityEngine;

/// <summary>
/// Declaration of the SubtitlePreset ScriptableObject. These give you the power to change the appearance of each subtitle or to give characters individual subtitle looks.
/// </summary>
[CreateAssetMenu(fileName = "NewSubPreset", menuName = "Dizzy Crow/Subtitle Preset")]
public class SubtitlePreset : ScriptableObject
{
    [Tooltip("The font.")]
    public Font font;
    [Tooltip("The font style, e.g. normal, bold, italic.")]
    public FontStyle fontStyle;
    [Tooltip("The font size.")]
    public int fontSize = 18;
    [Tooltip("The font color.")]
    public Color color = Color.white;
}

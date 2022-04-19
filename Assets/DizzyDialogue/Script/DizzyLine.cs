using UnityEngine;

/// <summary>
/// Declaration of the DizzyLine ScriptableObject. These are single lines of dialogue with metainfo to be used in the DizzyDialogue system.
/// </summary>
public class DizzyLine : ScriptableObject
{
    [Tooltip("DizzySpeaker where the audio and subtitle originate. Make sure you add the component to your speaker gameObject and set up its identity there.")]
    public string speakerIdentity = "YOURSPEAKER";
    [Tooltip("The text that is seen in the subtitle.")]
    public string line;
    [Tooltip("The audio file that is played. Can be null.")]
    public AudioClip clip;
    [Tooltip("How long the line takes to finish.")]
    public float length = 3f;
    [Tooltip("The maximum distance the line reaches.")]
    public float distance = 12f;
    [Tooltip("How much precedence it has over other lines to stay in the subtitles.")]
    public float power = 10f;
    [Tooltip("Whether the subtitle should display the speaker's name.")]
    public bool subtitleShowsSpeaker = true;
    [Tooltip("The preset to use for the subtitle. Create your own in the project hierarchy.")]
    public SubtitlePreset subtitleStyle;
    [Tooltip("The exit condition. When unassigned, next line plays automatically")]
    public DizzyCondition condition;
    [Tooltip("False: condLine0 plays after condition is met. True: If condition is met conLine0 plays, else condLine1 plays.")]
    public bool conditionIsBranch;
    [Tooltip("How many lines to jump forwards after the condition check.")]
    public int condJump0 = 1, condJump1 = 2;

    [HideInInspector]
    public GameObject speaker; // Accessed by other scripts, but should not be changed manually. To set this line's speaker, change speakerIdentity.
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays your lines' subtitles on referenced UI elements.
/// </summary>
public class DizzySubtitles : MonoBehaviour {

    [SerializeField, Tooltip("UI text elements that display subtitles. The amount controls how many subtitles can be shown simultaneously - make sure they are all assigned!")]
    private List<Text> textFields = new List<Text>();
    [SerializeField, Tooltip("UI text elements that display speaker names in subtitles. Make sure they are all assigned, and that there are as many of these as of the other type!")]
    private List<Text> nameFields = new List<Text>();

    [SerializeField, Tooltip("The UI box containing your text elements. Gets toggled off when no subtitles are in range.")]
    private GameObject subtitleUIArea;
    [SerializeField, Tooltip("Where the \"listener\" is positioned - should be the camera.")]
    private GameObject listenerTarget;
    
    // The list holding all lines that could be displayed
    private List<DizzyLine> linesInRange = new List<DizzyLine>();
    private int numberInRange;
    private int maxNumberSubtitles;

    private void Start()
    {
        if (!listenerTarget)
        {
            Debug.LogError("DizzyDialogue: No subtitle listener has been assigned. Using \'Main Camera\' if possible, but please assign this yourself!");
            listenerTarget = GameObject.Find("Main Camera");
        }
    }

    void LateUpdate ()
    {
        if (!listenerTarget)
            return;

        linesInRange.Clear();
        numberInRange = 0;
        maxNumberSubtitles = textFields.Count;

        // Add all lines in range to a list
        foreach (DizzyLine line in DizzyDialogue.activeLines)
        {
            Vector3 distance = line.speaker.transform.position - listenerTarget.transform.position;
            if (distance.magnitude <= line.distance)
                linesInRange.Add(line);
        }
        numberInRange = linesInRange.Count; // Number of lines in range

        if (numberInRange > 0)
        {
            // Sort subtitles based on their power
            linesInRange.Sort((x, y) => y.power.CompareTo(x.power));

            // Remove "weak" subtitles if there are too many lines in range
            if (numberInRange > maxNumberSubtitles)
            {
                linesInRange.RemoveRange(maxNumberSubtitles, numberInRange - maxNumberSubtitles);
            }

            // Assign line text & style to UI elements, disable excess elements
            for (int i = 0; i < maxNumberSubtitles; i++)
            {
                if (i < numberInRange)
                {
                    // Actives the subtitle UI object and sets its text to the line
                    textFields[i].enabled = true;
                    textFields[i].text = linesInRange[i].line;
                    if (linesInRange[i].subtitleStyle)
                    {
                        textFields[i].font = linesInRange[i].subtitleStyle.font;
                        textFields[i].fontStyle = linesInRange[i].subtitleStyle.fontStyle;
                        textFields[i].fontSize = linesInRange[i].subtitleStyle.fontSize;
                        textFields[i].color = linesInRange[i].subtitleStyle.color;
                    }

                    // Actives the subtitle speaker name UI object and sets its text to the speaker's name
                    if (linesInRange[i].subtitleShowsSpeaker)
                    {
                        nameFields[i].enabled = true;
                        nameFields[i].text = linesInRange[i].speakerIdentity + ": ";
                    }
                    else
                        nameFields[i].enabled = false;
                }
                else
                {
                    textFields[i].enabled = false;
                    nameFields[i].enabled = false;
                }
            }
            
            subtitleUIArea.SetActive(true);
        }
        else
        {
            subtitleUIArea.SetActive(false);
        }
	}
}

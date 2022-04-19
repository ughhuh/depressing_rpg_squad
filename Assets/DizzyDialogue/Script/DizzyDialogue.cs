using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Necessary in every scene using the DizzyDialogue system.
/// Manages the scene's active lines inside of a list and calls follow-up functions when they are finished.
/// Derived from Unity's Event Manager: https://unity3d.com/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
/// </summary>
public class DizzyDialogue : MonoBehaviour
{
    public static List<DizzyLine> activeLines;
    private Dictionary<string, UnityEvent> lineEventDictionary;
    private Dictionary<string, GameObject> speakerDictionary;

    private static DizzyDialogue dDialogue;

    // Makes sure that you have a Dialogue Manager in your scene (lots of errors if you don't).
    public static DizzyDialogue instance
    {
        get
        {
            if(!dDialogue)
            {
                dDialogue = FindObjectOfType(typeof(DizzyDialogue)) as DizzyDialogue;

                if (!dDialogue)
                {
                    Debug.LogError("There needs to be an active DizzyDialogue script on a GameObject in your scene to manage your dialogue. Add it to a GameObject under Add Component -> DizzyDialogue -> DizzyDialogue Manager");
                }
                else
                {
                    dDialogue.Init();
                }
            }

            return dDialogue;
        }
    }

    void Init()
    {
        if(activeLines == null)
        {
            activeLines = new List<DizzyLine>();
        }
        if (lineEventDictionary == null)
        {
            lineEventDictionary = new Dictionary<string, UnityEvent>();
        }
        if (speakerDictionary == null)
        {
            speakerDictionary = new Dictionary<string, GameObject>();
        }
    }

    private void OnEnable()
    {
        instance.Init();
    }

    // Add speaker (object got activated)
    public static void AddSpeaker(string speakerIdentity, GameObject speakerObj)
    {
        instance.speakerDictionary.Add(speakerIdentity, speakerObj);
    }

    // Remove speaker (object got deactivated)
    public static void RemoveSpeaker (string speakerIdentity, GameObject speakerObj)
    {
        if (!dDialogue)
            return;

        GameObject tempObj;
        if (instance.speakerDictionary.TryGetValue(speakerIdentity, out tempObj))
        {
            instance.speakerDictionary.Remove(speakerIdentity);
        }
    }

    // Get the speaker for this line
    private static GameObject FindSpeaker(string speakerIdentity)
    {
        GameObject possibleSpeaker;
        if(!instance.speakerDictionary.TryGetValue(speakerIdentity, out possibleSpeaker))
        {
            Debug.LogError("The speaker for this line was not found in the scene. Make sure you attach a DizzySpeaker component to a gameObject and give it the identity defined in your line. Missing speakerIdentity: " + speakerIdentity);
        }
        return possibleSpeaker;
    }

    // Start line
    public static void SayLine(DizzyLine line)
    {
        if (line.speakerIdentity != "")
        {
            // print(line.speakerIdentity +  " saying: " + line.line);
            GameObject speaker = FindSpeaker(line.speakerIdentity); // Looks up the speaker object in the speaker dictionary (returns null if the identity does not exist or gameObject is not active)
            if (speaker)
            {
                line.speaker = speaker;
                instance.TriggerEvent("StartedLine_" + line.GetInstanceID());
                activeLines.Add(line);

                // Play the line's audio clip from the speaker object if a clip is assigned and the object has an AudioSource component.
                AudioSource aud = speaker.GetComponent<AudioSource>();
                if (aud && line.clip)
                {
                    aud.clip = line.clip;
                    aud.Play();
                }

                instance.StartCoroutine(instance.WaitForEndOfLine(line));
            }
        }
        else
        {
            if (line.line != "")
                Debug.LogWarning("Line has no speaker, handling it as pause. Line: " + line.line);
            instance.StartCoroutine(instance.WaitForEndOfLine(line));
        }
    }

    // Line is playing
    private IEnumerator WaitForEndOfLine(DizzyLine line)
    {
        yield return new WaitForSeconds(line.length);
        EndLine(line);
    }

    // Line has finished (can also be called from other scripts to cancel a line prematurely)
    public static void EndLine(DizzyLine line)
    {
        if (!dDialogue)
            return;

        // print("Finished saying: " + line.line);
        activeLines.Remove(line);
        instance.TriggerEvent("EndedLine_" + line.GetInstanceID());
    }

    // UnityEvent based listening functions
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.lineEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.lineEventDictionary.Add(eventName, thisEvent);
        }
    }

    // UnityEvent based listening functions
    public static void StopListening(string eventName, UnityAction listener)
    {
        if (!dDialogue) return;
        UnityEvent thisEvent = null;
        if (instance.lineEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
            instance.lineEventDictionary.Remove(eventName);
        }
    }

    // UnityEvent based listening functions
    private void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.lineEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}

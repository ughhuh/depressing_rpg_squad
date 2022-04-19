using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Necessary in every speaker using the DizzyDialogue system. Acts as the source for lines you have set to play from here.
/// Make sure you give it a unique identity; this is used to identify the gameObject and it can show up in subtitles.
/// </summary>
[AddComponentMenu("Dizzy Crow/Dialogue Speaker")]
public class DizzySpeaker : MonoBehaviour {
    [SerializeField, Tooltip("Change this to whatever name your character should have. Should be unique and can be used in subtitles.")]
    public string identity = "SampleIdentity";

    // Adds this speaker to the speaker dictionary
    private void OnEnable()
    {
        DizzyDialogue.AddSpeaker(identity, gameObject);
    }

    // Removes this speaker from the speaker dictionary
    private void OnDisable ()
    {
        DizzyDialogue.RemoveSpeaker(identity, gameObject);
    }
}

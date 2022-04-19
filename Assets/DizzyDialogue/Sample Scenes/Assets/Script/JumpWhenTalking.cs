using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpWhenTalking : MonoBehaviour {

    private Rigidbody rb;
    private DizzyConvo convo;
    private string identity;
    private List<string> listeners;

    [SerializeField, Tooltip("The GameObject hosting the DizzyConvo that this actor is participating in - can leave blank if that's this object")]
    private GameObject convoHost;
    [SerializeField]
    private float jumpForce = 100f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        if (!convoHost)
            convoHost = gameObject;
        convo = convoHost.GetComponent<DizzyConvo>();
        identity = GetComponent<DizzySpeaker>().identity;

        foreach (DizzyLine line in convo.lines)
        {
            if (line.speakerIdentity == identity)
                DizzyDialogue.StartListening("StartedLine_" + line.GetInstanceID(), SampleAnimation);
        }
	}

    public void SampleAnimation()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }
}

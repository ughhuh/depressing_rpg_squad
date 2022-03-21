using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueNPC : MonoBehaviour
{
    // ui components
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] string nameNPC;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject namePanel;

    [SerializeField] string[] lines; //an array to store lines

    private float typingSpeed = 0.1f;
    private int lineCounter;
    private bool hasPlayer; // checks whether player is in trigger zone
    private bool isPlaying = false; // used to reassign E key functionalities

    
    void Update()
    {
        // start the dialogue by pressing e
        if (hasPlayer == true && Input.GetKeyDown(KeyCode.E) && isPlaying == false)
        {
            StartDialogue();
        }


        // isPlaying bool prevents user from restarting the dialogue
        if (Input.GetKeyDown(KeyCode.E) && isPlaying == true)
        {
            if (textComponent.text == lines[lineCounter]) // if a line is typed fully, type the next line
            {
                NextLine();
            }
            else // quick fill the line
            {
                StopAllCoroutines();
                textComponent.text = lines[lineCounter];
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player") // if player enters slime zone, record it
        {
            hasPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player") // if player enters slime zone, record it
        {
            hasPlayer = false;
        }
    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        namePanel.SetActive(true);
        textComponent.text = string.Empty;
        nameComponent.text = nameNPC;

        lineCounter = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {

        foreach (char k in lines[lineCounter].ToCharArray()) // type line letter by letter
        {
            textComponent.text += k;
            yield return new WaitForSeconds(typingSpeed);
            isPlaying = true;
        }
    }

    void NextLine()
    {
        if (lineCounter < lines.Length - 1) // if there're still lines to type, then type the next line
        {
            lineCounter++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else // fininsh the interraction
        {
            dialoguePanel.SetActive(false);
            namePanel.SetActive(false);
            isPlaying = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple way to handle a conversation (monologue, dialogue, or group).
/// Just add lines in chronological order and they will play one after the other.
/// </summary>
[AddComponentMenu("Dizzy Crow/Dialogue Convo")]
public class DizzyConvo : MonoBehaviour {

    [Tooltip("Set this to true if you want the conversation to start immediately.")]
    public bool startOnAwake = true;
    [Tooltip("Set this to true if you want the conversation to loop.")]
    public bool loop;
    [Tooltip("The lines in this conversation go here, playing chronologically.")]
    public List<DizzyLine> lines = new List<DizzyLine>();

    private int lineNum = 0; // How far into the conversation we are.
    private int prevLineNum = -1; // The last line spoken in this conversation.

    private void OnEnable()
    {
        if (startOnAwake)
        {
            StartCoroutine(ConverseDelay(0.2f)); // Starting with a delay makes sure that every component has initiated.
        }
    }

    /// <summary>
    /// Gets called to start a convo and to continue it after a line has finished.
    /// </summary>
    /// <param name="me">A check to make sure that the event triggering this convo is meant for this gameObject.</param>
    void Converse(GameObject me)
    {
        // Only go on if the event is for this convo object
        if (gameObject != me || lineNum < 0 || lineNum > lines.Count - 1)
            return;


        if (prevLineNum > -1)
        {
            // Stop listening to previous line
            DizzyDialogue.StopListening("EndedLine_" + lines[prevLineNum].GetInstanceID(), delegate { Converse(gameObject); });

            if (lines[prevLineNum].condition == null)
            {
                if (!(lineNum == 0 && prevLineNum == lines.Count - 1))
                    lineNum++;
            }
            else
            {
                lines[prevLineNum].condition.ResetObjects();
                if (lines[prevLineNum].condition.CheckCondition()) // If condition is met
                {
                    lineNum += lines[prevLineNum].condJump0; // Jump in conversation based on condJump0
                }
                else if (lines[prevLineNum].conditionIsBranch) // If condition is not met but conversation should branch
                {
                    lineNum += lines[prevLineNum].condJump1; // Jump in conversation based on condJump1
                } else // Conversation should not branch; wait for condition
                {
                    StartCoroutine(WaitForCondition(lines[prevLineNum], 0.5f));
                    return;
                }

                if (lineNum < 0 || lineNum > lines.Count - 1)
                {
                    Debug.LogWarning("Jumped outside of conversation through branching condition " + lines[prevLineNum].condition + ". Stopping convo in gameObject " + gameObject + " (line " + prevLineNum + ")");
                    return;
                }
            }
        }

        if (lineNum > lines.Count - 1)
        {
            if (loop)
                lineNum = 0;
            else
                return;
        }
        
        // Say the line
        DizzyDialogue.SayLine(lines[lineNum]);
        // print(gameObject + " saying line " + lineNum + ": " + lines[lineNum].line);
        prevLineNum = lineNum;

        if (lineNum + 1 < lines.Count) // if not at the final item on the list
        {
            WaitForLineEnd();
        }
        else if (loop)
        {
            WaitForLineEnd();
            lineNum = 0; // Reset the convo progression
        }
    }

    private void WaitForLineEnd()
    {
        // Start listening for the end of the current line to continue the convo
        DizzyDialogue.StartListening("EndedLine_" + lines[prevLineNum].GetInstanceID(), delegate { Converse(gameObject); });
    }

    /// <summary>
    /// Keeps checking the condition of the specified line in the specified interval.
    /// </summary>
    /// <param name="line">The line that holds the condition to check.</param>
    /// <param name="repeatDelay">How much time to leave between condition checks.</param>
    private IEnumerator WaitForCondition(DizzyLine line, float repeatDelay)
    {
        yield return new WaitForSeconds(repeatDelay);
        if (line.condition.CheckCondition())
            Converse(gameObject);
        else
            StartCoroutine(WaitForCondition(line, repeatDelay));
    }

    /// <summary>
    /// A delay before calling Converse() again.
    /// </summary>
    /// <param name="delay">Time in seconds to delay calling the Converse function.</param>
    /// <returns></returns>
    IEnumerator ConverseDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Converse(gameObject);
    }
}

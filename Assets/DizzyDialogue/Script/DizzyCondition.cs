using UnityEngine;

/*          HOW TO CREATE YOUR OWN CONDITIONS
 * 1. Add a new case for condType, starting with "3" (line 72)
 *    Here is where you check the condition, so that code goes there
 * 2. Go to DizzyConditionEditor.cs
 * 3. Add a new item to condTypes, that being the name of the condition
 * 4. Add a new case for condType, starting with "3"
 * 5. See if it works! If it does not, feel free to contact me!
 */

/// <summary>
/// Conditions to use for branching or halting dialogues.
/// </summary>
[CreateAssetMenu(fileName = "NewCondition", menuName = "Dizzy Crow/Condition")]
public class DizzyCondition : ScriptableObject
{
    [Tooltip("Denotes the type of condition. \n0 = proximity \n1 = position \n2 = time")]
    public int condType = 0;
    [Tooltip("For proximity & position: distance to check. \nFor time: \'Time.time\' to check.")]
    public float condFloat = 5f;
    [Tooltip("GameObjects to compare.")]
    public string condObj0, condObj1;
    [Tooltip("For position comparison.")]
    public Vector3 condPosition;

    private GameObject obj0, obj1; // Assigned automatically; set condObj0 and condObj1 manually

    /// <summary>
    /// GameObjects will have to be found again after running this function.
    /// </summary>
    public void ResetObjects()
    {
        obj0 = null;
        obj1 = null;
    }

    /// <summary>
    /// Checks if condition is met. Returns true if its is and false if it isn't (or if it's improperly set up).
    /// </summary>
    public bool CheckCondition()
    {
        if (obj0 == null && !(condObj0 == null || condObj0 == "")) // Assigns GameObject 0
        {
            obj0 = GameObject.Find(condObj0);
            if (!obj0)
            {
                Debug.LogError("Condition check could not find GameObject " + condObj0 + " - condition: " + this);
                return false;
            }
        }
        if (condType == 0 && obj1 == null) // Assigns GameObject 1 (if proximity condition)
        {
            obj1 = GameObject.Find(condObj1);
            if (!obj1)
            {
                Debug.LogError("Condition check could not find GameObject " + condObj1 + " - condition: " + this);
                return false;
            }
        }

        switch (condType)
        {
            case 0: // Proximity
                Vector3 prox = obj0.transform.position - obj1.transform.position;
                return (prox.magnitude <= condFloat);
            case 1: // Position
                Vector3 dist = obj0.transform.position - condPosition;
                return (dist.magnitude <= condFloat);
            case 2: // Time
                return (Time.time > condFloat);
            default:
                Debug.LogError("Condition type set incorrectly: " + this);
                return false;
        }
        
    }
}
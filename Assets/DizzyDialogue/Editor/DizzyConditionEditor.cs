using UnityEditor;

/*          HOW TO CREATE YOUR OWN CONDITIONS
 * 1. Start in DizzyCondition.cs, not this script
 *    Add a new case for condType, starting with "3"
 *    Here is where you check the condition, so that code goes there
 * 2. Go to this script (DizzyConditionEditor.cs)
 * 3. Add a new item to condTypes, that being the name of the condition (line 16)
 * 4. Add a new case for condType, starting with "3" (line 41)
 * 5. See if it works! If it does not, feel free to contact me!
 */

[CustomEditor(typeof(DizzyCondition))]
public class DizzyConditionEditor : Editor
{
    // Condition names displayed in the dropdown menu
    string[] condTypes = new[] { "Proximity", "Position", "Time" };
    
    public override void OnInspectorGUI()
    {
        DizzyCondition condition = (DizzyCondition)target;
        EditorStyles.label.wordWrap = true;
        condition.condType = EditorGUILayout.Popup("Condition type", condition.condType, condTypes);
        switch (condition.condType)
        {
            case 0: // Proximity
                EditorGUILayout.LabelField("Returns TRUE if object distance is LESS THAN OR EQUAL TO target distance.");
                condition.condObj0 = EditorGUILayout.TextField("Object 1", condition.condObj0);
                condition.condObj1 = EditorGUILayout.TextField("Object 2", condition.condObj1);
                condition.condFloat = EditorGUILayout.FloatField("Distance", condition.condFloat);
                break;
            case 1: // Position
                EditorGUILayout.LabelField("Returns TRUE if object distance from position is LESS THAN OR EQUAL TO target distance.");
                condition.condObj0 = EditorGUILayout.TextField("Object", condition.condObj0);
                condition.condPosition = EditorGUILayout.Vector3Field("Position", condition.condPosition);
                condition.condFloat = EditorGUILayout.FloatField("Distance", condition.condFloat);
                break;
            case 2: // Time
                EditorGUILayout.LabelField("Returns TRUE if \'Time.time\' is GREATER THAN target time.");
                condition.condFloat = EditorGUILayout.FloatField("Time", condition.condFloat);
                break;
            default:
                EditorGUILayout.LabelField("The condition is set to an incorrect type, please correct.");
                break;
        }

        EditorUtility.SetDirty(target);
    }
}
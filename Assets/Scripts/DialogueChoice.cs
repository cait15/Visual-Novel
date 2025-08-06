using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Choice", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class DialogueChoice : ScriptableObject
{
    public string choiceText;
    
    public int damageToPlayer;
    public int damageToEnemy;

    public DialogueNode nextNode;
}

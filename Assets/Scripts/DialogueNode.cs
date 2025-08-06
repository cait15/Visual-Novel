using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Scriptable Objects/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    public string dialogueText;
    
    public string name;

    public DialogueChoice[] choices;

    public DialogueNode nextDefaultNode; 
    
    public List<Sprite> animatedSprites; // List of sprites for animation
    public bool isThisJudgeDialogue;
    public bool isReactionDialogue;

    public float animationSpeed = 0.75f;
}

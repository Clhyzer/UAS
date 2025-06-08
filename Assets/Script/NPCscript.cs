using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPCscript : ScriptableObject
{
    public string npcName;
    public string[] dialogueBeforeQuest;
    public string[] dialogueAfterQuest;
    public bool givesQuest;
    public string questID;
}

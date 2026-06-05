using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // The dialogue to trigger
    public TextBubble textBubble; // Reference to the NPC's TextBubble
    public Transform questionBoxContainer; // Reference to the NPC's question box container

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, textBubble, questionBoxContainer); // Start the dialogue
    }

    private void OnMouseDown()
    {
        if (DialogueManager.Instance.isDialogueActive == false) // Only trigger if no dialogue is active
        {
            TriggerDialogue();
        }
    }
}
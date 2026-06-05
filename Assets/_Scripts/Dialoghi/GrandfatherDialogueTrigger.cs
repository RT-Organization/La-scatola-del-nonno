using UnityEngine;

public class GrandfatherDialogueTrigger : DialogueTrigger
{
    // Additional fields for the grandfather's dialogues
    public Dialogue glassesDialogue; // Dialogue for the glasses
    public Dialogue pictureDialogue; // Dialogue for the picture
    public Dialogue binocularsDialogue; // Dialogue for the binoculars
    public Dialogue gunDialogue; // Dialogue for the gun

    private Dialogue currentDialogue; // Current dialogue to use

    private void Start()
    {
        // Set the default dialogue initially
        currentDialogue = dialogue; // Use the base dialogue as the default
    }

    public void SetDialogueForItem(string itemName)
    {
        // Swap the dialogue based on the item brought
        switch (itemName)
        {
            case "Glasses":
                currentDialogue = glassesDialogue;
                break;
            case "Picture":
                currentDialogue = pictureDialogue;
                break;
            case "Binoculars":
                currentDialogue = binocularsDialogue;
                break;
            case "Gun":
                currentDialogue = gunDialogue;
                break;
            default:
                currentDialogue = dialogue; // Fallback to the default dialogue
                break;
        }
    }

    public new void TriggerDialogue()
    {
        // Use the current dialogue (grandfather-specific) instead of the default one
        DialogueManager.Instance.StartDialogue(currentDialogue, textBubble, questionBoxContainer);
    }
}
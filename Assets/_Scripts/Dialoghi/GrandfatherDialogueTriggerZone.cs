using UnityEngine;

public class GrandfatherDialogueTriggerZone : MonoBehaviour
{
    public GrandfatherDialogueTrigger grandfatherDialogueTrigger; // Reference to the grandfather's dialogue trigger

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is one of the items
        string itemName = other.gameObject.name;
        grandfatherDialogueTrigger.SetDialogueForItem(itemName);

        // Trigger the updated dialogue
        grandfatherDialogueTrigger.TriggerDialogue();

        // Optionally, you can destroy the item or disable it after it's been used
        Destroy(other.gameObject);
    }
}
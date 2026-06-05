using System.Collections;
using UnityEngine;

public class KeyItem : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string keyID = "CabinKey"; // Unique identifier for this key
    public float pickupDelay = 0.2f; // Small delay before disappearing

    public void Interact()
    {
        // Add key to inventory
        InventorySystem.Instance.AddKey(keyID);

        // Play sound if needed (optional)
        // AudioManager.Instance.Play("KeyPickup");

        // Disable with small delay for feedback
        StartCoroutine(DisableKey());
    }

    private IEnumerator DisableKey()
    {
        yield return new WaitForSeconds(pickupDelay);
        gameObject.SetActive(false); // Makes the key disappear
    }
}
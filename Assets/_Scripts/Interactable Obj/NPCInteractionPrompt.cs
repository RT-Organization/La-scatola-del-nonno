using UnityEngine;

public class NPCInteractionPrompt : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt; // UI element for the interaction prompt
    [SerializeField] private float interactionRange = 5f; // Range for detecting NPCs

    private Camera playerCamera; // Reference to the player's camera

    private void Start()
    {
        // Find the player's camera
        playerCamera = Camera.main;

        // Ensure the prompt is hidden at the start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Perform a raycast from the camera to detect NPCs
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            // Check if the raycast hits an NPC
            if (hit.transform.CompareTag("NPC"))
            {
                // Show the interaction prompt
                ShowPrompt();
            }
            else
            {
                // Hide the prompt if not looking at an NPC
                HidePrompt();
            }
        }
        else
        {
            // Hide the prompt if nothing is hit
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
}
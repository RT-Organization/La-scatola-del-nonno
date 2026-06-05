using TMPro;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    // UI Components
    [SerializeField] private TextMeshPro questionText; // The displayed question text
    [SerializeField] private DialogueLine response; // Dialogue response when selected

    // References
    private BoxCollider boxCollider; // Collider for mouse interaction
    private Color defaultColor; // Original text color
    private Transform cam; // Main camera reference
    private SpriteRenderer background; // Background sprite

    private void Awake()
    {
        // Initialize references
        cam = Camera.main.transform;
        boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();
        defaultColor = questionText.color; // Cache default color
        background = transform.Find("Background").GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Keep UI element facing camera
        transform.LookAt(transform.position + cam.forward);
    }

    /// <summary>
    /// Initializes question box with content
    /// </summary>
    public void Setup(string question, DialogueLine response)
    {
        questionText.text = question;
        this.response = response;
        AdjustColliderAndBackground();
    }

    /// <summary>
    /// Adjusts collider and background size to match text dimensions
    /// </summary>
    private void AdjustColliderAndBackground()
    {
        if (!questionText || !boxCollider || !background) return;

        // Update text mesh and get rendered size
        questionText.ForceMeshUpdate();
        Vector2 textSize = questionText.GetRenderedValues(false);

        // Apply padding and update visuals
        Vector2 backgroundSize = textSize + new Vector2(0.5f, 0.25f);
        background.size = backgroundSize;
        boxCollider.size = new Vector3(backgroundSize.x, backgroundSize.y, 0f);
        boxCollider.center = new Vector3(0f, 0f, -.1f);
    }

    // --- Mouse Interaction Handlers ---
    private void OnMouseDown()
    {
        DialogueManager.Instance.OnQuestionSelected(response);
    }

    private void OnMouseEnter()
    {
        if (questionText) questionText.color = Color.cyan;
    }

    private void OnMouseExit()
    {
        if (questionText) questionText.color = defaultColor;
    }
}
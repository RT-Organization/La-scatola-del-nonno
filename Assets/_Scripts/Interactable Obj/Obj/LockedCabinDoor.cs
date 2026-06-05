using UnityEngine;

public class LockedCabinDoor : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string requiredKeyID = "CabinKey";
    public float openAngle = 90f;
    public float smoothTime = 0.3f;

    private bool _isOpen;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(0, openAngle, 0) * _closedRotation;
    }

    public void Interact()
    {
        if (InventorySystem.Instance.HasKey(requiredKeyID))
        {
            _isOpen = !_isOpen;
            // Play door sound if needed
        }
        else
        {
            Debug.Log("You need the cabin key to open this!");
            // Play locked sound if needed
        }
    }

    private void Update()
    {
        Quaternion targetRotation = _isOpen ? _openRotation : _closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime * Time.deltaTime);
    }
}
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;

    public float throwForce = 500f; //force at which the object is thrown at
    public float pickUpRange = 5f; //how far the player can pickup the object from
    private float rotationSensitivity = 2.5f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; //layer index
    private float originalSesitivityValue = 2.5f;
    public CameraManager mouseLookScript;
    [SerializeField] private GameObject interactionText;
    [SerializeField] private GameObject heldText;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer"); //if your holdLayer is named differently make sure to change this ""
    }



    void Update()
    {
        RaycastHit hit;
        bool isLookingAtObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange);

        if (heldObj == null) // Not holding anything
        {
            if (isLookingAtObject && hit.transform.CompareTag("canPickUp"))
            {
                // Show pickup prompt with a custom message
                ShowPickUpPrompt();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    print("DEVI PRENDERLO NON INTERAGIRE...");
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickUpObject(hit.transform.gameObject);
                }
            }
            else
            {
                // Hide prompt if not looking at an object
                HidePickUpPrompt();
            }
        }
        else // Holding an object
        {
            // Hide pickup prompt and show held object prompt instead
            HidePickUpPrompt();
            ShowHeldObjectPrompt();

            MoveObject();
            RotateObject();

            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop)
            {
                StopClipping();
                ThrowObject();
            }
            else if (Input.GetKeyDown(KeyCode.F) && canDrop)
            {
                StopClipping();
                DropObject();
            }
        }
    }



    private void ShowPickUpPrompt()
    {
        interactionText.SetActive(true);
    }

    private void ShowHeldObjectPrompt()
    {
        heldText.SetActive(true);
    }

    private void HidePickUpPrompt()
    {
        interactionText.SetActive(false);
    }


    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }

    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))//hold R key to rotate, change this to whatever key you want
        {
            canDrop = false; //make sure throwing can't occur during rotating

            //disable player being able to look around
            mouseLookScript.mouseSensitivity = 0f;

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            //re-enable player being able to look around
            mouseLookScript.mouseSensitivity = originalSesitivityValue;
            canDrop = true;
        }
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        heldText.SetActive(false);
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Character Movement Stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f; //runSpeed not implemented yet

    public float CurrMoveSpeed => wishRun ? runSpeed : walkSpeed; //implement "movement speed type" Status (MoveSpeed-Nerf/Buff-Flat/%) currently applied
    public float MaxMoveSpeed => runSpeed; //implement "movement speed type" Status (MoveSpeed-Nerf/Buff-Flat/%) currently applied

    [Space]

    public float jumpSpeed = 5f;
    public float MaxJumpSpeed => jumpSpeed; //implement with "jump speed type" Status (JumpSpeed-Nerf/Buff-Flat/%) currently applied

    [Header("Movement Control")]
    [Tooltip("Acceleration constant that describes how fast the characer can change movement direction in the air or when its current speed exceeds currently allowed movement speed for it")]
    [SerializeField] private float steeringAcceleration = 20f;

    [Header("Movement Reduction")]

    [Header("Horizontal")]
    [Tooltip("Reduction (deceleration constant) of horizontal velocity when grounded")]
    [SerializeField] private float groundHorizontalDrag = 10f;
    [Tooltip("Reduction (deceleration constant) of horizontal velocity in mid-air. Highly suggest 0")]
    [SerializeField] private float airHorizontalDrag = 0f;

    [Header("Vertical")]
    [Tooltip("Effect of gravity at rest (grounded). Should leave at 2")]
    [SerializeField] private float groundGravity = 2f;
    [Tooltip("Gravitational acceleration constant applied in the air")]
    [SerializeField] private float gravity = 9.81f;

    [Header("Dependencies")]
    [SerializeField] private UnityEngine.CharacterController controller;

    public bool IsGrounded => controller.isGrounded;



    private KeyCode
        forwardBind = KeyCode.W,
        leftBind = KeyCode.A,
        backwardBind = KeyCode.S,
        rightBind = KeyCode.D,
        runBind = KeyCode.LeftShift,
        jumpBind = KeyCode.Space;



    private bool
        wishForward = false,
        wishLeft = false,
        wishBackward = false,
        wishRight = false,
        wishRun = false,
        wishUp = false;
    private Vector3 wishDirection = Vector3.zero;
    private float wishY = 0f;

    public Vector3 WishDirection => wishDirection;
    public bool WishUp => wishUp;



    private Vector3
        moveVelocity = Vector3.zero,
        jumpVelocity = Vector3.zero,
        bufferVelocity = Vector3.zero; //for additional, external velocities (from other scripts)

    public Vector3 MoveVelocity => moveVelocity;
    public Vector3 JumpVelocity => jumpVelocity;



    //Current Velocity:
    private Vector3 velocity = Vector3.zero, horizontalVelocity = Vector3.zero, verticalVelocity = Vector3.zero;

    public Vector3 Velocity
    {
        get => velocity;
        set
        {
            velocity = value;
            horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            verticalVelocity = new Vector3(0f, velocity.y, 0f);
        }
    }
    public Vector3 HorizontalVelocity
    {
        get => horizontalVelocity;
        set => Velocity = new Vector3(value.x, velocity.y, value.z);
    }
    public Vector3 VerticalVelocity
    {
        get => verticalVelocity;
        set => Velocity = new Vector3(velocity.x, value.y, velocity.z);
    }
    //With these properties, be careful with editing x, y, z directly



    void Update()
    {
        InputUpdate();
        MoveUpdate();
        JumpUpdate();

        /*ignore, draw moveVelocity*/
        Debug.DrawLine(transform.position, transform.position + moveVelocity, Color.green, Time.deltaTime);
        /*ignore, draw jumpVelocity*/
        if (jumpVelocity.magnitude > 0f) Debug.DrawLine(transform.position, transform.position + jumpVelocity, Color.blue, 1f);
        /*ignore, draw bufferVelocity*/
        if (bufferVelocity.magnitude > 0f) Debug.DrawLine(transform.position, transform.position + bufferVelocity, Color.yellow, 1f);

        ApplyHorizontalDrag(Time.deltaTime);
        ApplyGravity(Time.deltaTime);

        /*ignore, draw gravity (vertical velocity)*/
        Debug.DrawLine(transform.position, transform.position + Vector3.up * velocity.y, Color.red, Time.deltaTime);

        BeatGroundGravityWhenAscending();

        SumVelocities(Time.deltaTime);
        ClearBufferVelocity();

        /*ignore, draw velocity*/
        Debug.DrawLine(transform.position, transform.position + velocity, Color.white, Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);
    }



    private void InputUpdate()
    {
        wishForward = Input.GetKey(forwardBind);
        wishLeft = Input.GetKey(leftBind);
        wishBackward = Input.GetKey(backwardBind);
        wishRight = Input.GetKey(rightBind);

        wishRun = Input.GetKey(runBind);

        wishDirection = (
            ((wishForward ? 1f : 0f) + (wishBackward ? -1f : 0f)) * transform.forward
            + ((wishLeft ? -1f : 0f) + (wishRight ? 1f : 0f)) * transform.right
            ).normalized;

        wishUp = Input.GetKey(jumpBind);
        wishY = wishUp ? 1f : 0f;
    }

    private void MoveUpdate() => moveVelocity = CurrMoveSpeed * wishDirection;

    private Vector3 GroundJump() => MaxJumpSpeed * wishY * Vector3.up;
    private Vector3 AirJump() => Vector3.zero;
    private void JumpUpdate() => jumpVelocity = controller.isGrounded ? GroundJump() : AirJump();

    private bool Approximately(float a, float b, float delta) => Mathf.Abs(a - b) <= delta; //can be moved to Math utility class

    private Vector3 Drag(Vector3 velocity, float drag, float deltaTime) //returns velocity dragged by drag given deltaTime, can be moved to Vector3 utility class
    {
        Vector3 dragVector = (-drag * velocity.normalized) * deltaTime;
        return velocity.sqrMagnitude > dragVector.sqrMagnitude ? velocity + dragVector : Vector3.zero;
    }
    private Vector3 GroundHorizontalDrag(float deltaTime) => Drag(horizontalVelocity, groundHorizontalDrag, deltaTime);
    private Vector3 AirHorizontalDrag(float deltaTime) => Drag(horizontalVelocity, airHorizontalDrag, deltaTime);
    private void ApplyHorizontalDrag(float deltaTime) => HorizontalVelocity = controller.isGrounded ? GroundHorizontalDrag(deltaTime) : AirHorizontalDrag(deltaTime);

    private Vector3 GroundGravitate() => verticalVelocity.y < 0 ? groundGravity * Vector3.down : verticalVelocity;
    private Vector3 AirGravitate(float deltaTime) => (gravity * Vector3.down) * deltaTime + verticalVelocity;
    private void ApplyGravity(float deltaTime) => VerticalVelocity = controller.isGrounded ? GroundGravitate() : AirGravitate(deltaTime);

    private void BeatGroundGravityWhenAscending() => VerticalVelocity = verticalVelocity + (controller.isGrounded && jumpVelocity.y + bufferVelocity.y > 0f ? groundGravity * Vector3.up : Vector3.zero);

    private Vector3 Sum(Vector3[] vectors, Action<Vector3> action) //can be moved to Vector3 utility class
    {
        Vector3 sum = Vector3.zero;

        foreach (var vector in vectors)
        {
            sum += vector;
            action?.Invoke(vector);
        }

        return sum;
    }
    private Vector3 Sum(params Vector3[] vectors) => Sum(vectors, null); //can be moved to Vector3 utility class
    private Vector3 SumAndClampToLongest(params Vector3[] vectors) //can be moved to Vector3 utility class
    {
        float longest = 0f;

        void action(Vector3 vector) => longest = Mathf.Max(longest, vector.magnitude);
        Vector3 sum = Sum(vectors, action);

        return Vector3.ClampMagnitude(sum, longest);
    }
    private void SumVelocities(float deltaTime) => Velocity = Sum(
        controller.isGrounded && MaxMoveSpeed >= horizontalVelocity.magnitude
        ? moveVelocity
        : Vector3.ClampMagnitude(
            steeringAcceleration * moveVelocity.normalized * deltaTime + horizontalVelocity,
            CurrMoveSpeed >= horizontalVelocity.magnitude
            ? CurrMoveSpeed
            : horizontalVelocity.magnitude
            ),
        jumpVelocity,
        verticalVelocity,
        bufferVelocity
        );

    private void ClearBufferVelocity() => bufferVelocity = Vector3.zero;


    public void AddVelocity(Vector3 velocity) => bufferVelocity += velocity;
    public void SetVelocity(Vector3 velocity) => Velocity = velocity;
}
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float runMultiplier = 2;

    [SerializeField]
    private float gravity = 5;

    [SerializeField]
    private float jumpForce = 6;

    [Tooltip("Determines how fast the player will change directions in the air.")]
    [SerializeField]
    private float airAccelerationSpeed = 5;

    private Vector3 currentVelocity;

    private InputManager input;
    private CharacterController controller;

    private void Awake()
    {
        input = GetComponent<InputManager>();
        controller = GetComponent<CharacterController>();
    }

    private Vector3 GetMoveVector()
    {
        var rightVector = input.Horizontal * transform.right;
        var forwardVector = input.Vertical * transform.forward;

        //Movement.
        return rightVector + forwardVector;
    }

    private void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        //If we're grounded.
        if(controller.isGrounded)
        {
            //Movement.
            currentVelocity = GetMoveVector() * GetSpeed();

            //Jumping.
            if (input.Jump)
                currentVelocity += Vector3.up * jumpForce;
        }
        else
        {
            //Apply gravity.
            var gravityVector = Vector3.down * gravity * deltaTime;
            currentVelocity += gravityVector;

            //Movement.
            var inAirMoveVector = GetMoveVector() * GetSpeed();
            inAirMoveVector -= currentVelocity;

            var velocityProjectedDiff = Vector3.ProjectOnPlane(inAirMoveVector, gravityVector);
            velocityProjectedDiff *= deltaTime;

            currentVelocity += velocityProjectedDiff * airAccelerationSpeed;
        }

        //Move the controller.
        controller.Move(currentVelocity * Time.fixedDeltaTime);
    }

    private float GetSpeed()
    {
        var speed = this.speed;
        speed *= input.IsRunning ? runMultiplier : 1f;

        return speed;
    }
}
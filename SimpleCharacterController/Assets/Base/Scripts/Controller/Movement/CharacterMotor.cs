using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;

    [Range(0, 4)]
    [SerializeField]
    private float runMultiplier = 2;

    [SerializeField]
    private float crouchSpeedMultiplier = 1;

    [Header("Crouching")]
    [Space(5f)]

    [SerializeField]
    private Lerper crouchLerp;

    [SerializeField]
    private float crouchHeightMultiplier = 0.5f;

    [Header("Uncrouching detection")]
    [Space(5f)]

    [SerializeField]
    private float uncrouchDetectionSphereHeightMultiplier = 1.8f;

    [SerializeField]
    private LayerMask uncrouchDetectionMask;

    [Header("In air settings")]
    [Space(5f)]

    [Tooltip("Determines how hard the gravity pushes down on the player")]
    [SerializeField]
    private float gravity = 10;

    [SerializeField]
    private float stickToGroundForce = 20;

    [Tooltip("Determines how far we can jump")]
    [SerializeField]
    private float jumpForce = 6;

    [Tooltip("Determines how fast we change directions while in the air")]
    [SerializeField]
    private float airAccelerationSpeed = 5;

    [Header("Debugs")]
    [Space(5f)]

    [SerializeField]
    private bool debugCrouchDetectionSphere;

    private Vector3 moveVector;
    private Vector3 currentVelocity;

    private PlayerHandler pHandler;
    private CharacterController controller;

    private bool inTransition;
    private Vector2 normalControllerValues;

    private Vector3 cameraNormalPosition;
    private Transform mainCamera;

    private void Start()
    {
        //Get a reference to the player handler.
        pHandler = Manager.Instance.PlayerHandler;
        //Get a reference to the controller component.
        controller = GetComponent<CharacterController>();

        //Get the camera object's transform.
        mainCamera = GetComponentInChildren<Camera>().transform;
        //Save the camera's standing position.
        cameraNormalPosition = mainCamera.localPosition;

        //Store the default standing values.
        normalControllerValues.x = controller.height;
        normalControllerValues.y = controller.center.y;
    }

    private void UpdateMoveVector()
    {
        //Vector that moves the character along its x axis.
        var rightVector = pHandler.Horizontal * transform.GetChild(0).right;
        //Vector that moves the character along its z axis.
        var forwardVector = pHandler.Vertical * transform.GetChild(0).forward;

        //Vector that moves the character.
        moveVector = rightVector + forwardVector;
        moveVector.Normalize();
    }

    private void OnDrawGizmosSelected()
    {
        if (debugCrouchDetectionSphere)
            Gizmos.DrawWireSphere(transform.position + Vector3.up * uncrouchDetectionSphereHeightMultiplier, controller.radius);
    }

    public bool CanUncrouch()
    {
        //Check if there are colliders over the controller.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up * uncrouchDetectionSphereHeightMultiplier,
            controller.radius, uncrouchDetectionMask);

        //If there's nothing on top of the player, we can uncrouch.
        if (hitColliders.Length == 0)
            return true;

        return false;
    }

    public void OnToggleCrouch()
    {
        //We start the transition.
        inTransition = true;

        //Reset the lerp.
        crouchLerp.Reset();
    }

    private void Update()
    {
        //If we need to transition.
        if (inTransition)
        {
            //Update the easing.
            crouchLerp.Update(Time.deltaTime);

            //Stop the transition if it's done.
            if (crouchLerp.IsDone(true))
                inTransition = false;

            //Get the correct values to transition to.
            var gHeight = pHandler.IsCrouched ? normalControllerValues.x * crouchHeightMultiplier : normalControllerValues.x;
            var gCenter = pHandler.IsCrouched ? normalControllerValues.y * crouchHeightMultiplier : normalControllerValues.y;

            //Lerp to those values.
            controller.height = Mathf.Lerp(controller.height, gHeight, crouchLerp.InterpolatedValue);
            controller.center = Vector3.Lerp(controller.center, gCenter * Vector3.up, crouchLerp.InterpolatedValue);
        }
    }

    private void FixedUpdate()
    {
        UpdateMoveVector();

        //Get the current fixed delta time.
        var deltaTime = Time.fixedDeltaTime;

        //If we're grounded.
        if (controller.isGrounded)
        {
            //Get the new movement direction.
            currentVelocity = moveVector * GetSpeed();
            //Stick the character to the ground.
            currentVelocity.y -= stickToGroundForce;

            //If we jump, apply some force.
            if (pHandler.Jump)
                currentVelocity = Vector3.up * jumpForce;
        }
        //If we're in the air.
        else
        {
            //Calculate the gravity vector.
            var gravityVector = Vector3.down * gravity * deltaTime;
            //Add it to the current velocity.
            currentVelocity += gravityVector;

            //In air movement velocity.
            var inAirMoveVector = moveVector * GetSpeed();
            //This subtraction allows for the nice curvature of the jump by factoring in our gravity.
            inAirMoveVector -= currentVelocity;

            //Project the velocity so it's perpendicular to the controller.
            var velocityDiff = Vector3.ProjectOnPlane(inAirMoveVector, gravityVector);

            //Add it to the current velocity.
            currentVelocity += velocityDiff * airAccelerationSpeed * deltaTime;
        }

        //Set the current velocity value.
        pHandler.CurrentVelocity = currentVelocity;

        //Move the controller.
        controller.Move(currentVelocity * deltaTime);
    }

    private float GetSpeed()
    {
        var speed = this.speed;
        speed *= pHandler.IsRunning ? runMultiplier : 1f;
        speed *= pHandler.IsCrouched ? crouchSpeedMultiplier : 1f;

        return speed;
    }
}
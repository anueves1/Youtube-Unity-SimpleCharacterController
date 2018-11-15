using UnityEngine;

namespace Finished
{
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5;

        [Range(0, 4)]
        [SerializeField]
        private float runMultiplier = 2;

        [Header("Crouching")]
        [Space(5f)]

        [SerializeField]
        private float crouchSpeedMultiplier = 1f;

        [SerializeField]
        private Lerper crouchLerp;

        [SerializeField]
        private float crouchHeightMultiplier = 0.5f;

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

        private Vector3 moveVector;
        private Vector3 currentVelocity;

        private InputManager input;
        private CharacterController controller;

        private bool wasCrouching;
        private bool inTransition;

        private Vector2 normalControllerValues;

        private void Awake()
        {
            //Get a reference to the input component.
            input = GetComponent<InputManager>();
            //Get a reference to the controller component.
            controller = GetComponent<CharacterController>();

            //Store the default standing values.
            normalControllerValues.x = controller.height;
            normalControllerValues.y = controller.center.y;
        }

        private void UpdateMoveVector()
        {
            //Vector that moves the character along its x axis.
            var rightVector = input.Horizontal * transform.right;
            //Vector that moves the character along its z axis.
            var forwardVector = input.Vertical * transform.forward;

            //Vector that moves the character.
            moveVector = rightVector + forwardVector;
            moveVector.Normalize();
        }

        private void Update()
        {
            //If we just toggled the crouch.
            if (input.IsCrouched != wasCrouching)
            {
                //We start the transition.
                inTransition = true;

                //Reset the lerp.
                crouchLerp.Reset();
            }

            //Assign the crouching value for this frame.
            wasCrouching = input.IsCrouched;

            //If we need to transition.
            if(inTransition)
            {
                //Update the easing.
                crouchLerp.Update(Time.deltaTime);

                //Stop the transition if it's done.
                if (crouchLerp.IsDone(true))
                    inTransition = false;

                //Get the correct values to transition to.
                var gHeight = wasCrouching ? normalControllerValues.x * crouchHeightMultiplier : normalControllerValues.x;
                var gCenter = wasCrouching ? normalControllerValues.y * crouchHeightMultiplier : normalControllerValues.y;

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
                if (input.Jump)
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

            //Move the controller.
            controller.Move(currentVelocity * deltaTime);
        }

        private float GetSpeed()
        {
            var speed = this.speed;
            speed *= input.IsRunning ? runMultiplier : 1f;
            speed *= input.IsCrouched ? crouchSpeedMultiplier : 1f;

            return speed;
        }
    }
}
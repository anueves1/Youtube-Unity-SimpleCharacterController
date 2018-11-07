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

        [Header("In air settings")]
        [Space(5f)]

        [Tooltip("Determines how hard the gravity pushes down on the player")]
        [SerializeField]
        private float gravity = 10;

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

        private void Awake()
        {
            //Get a reference to the input component.
            input = GetComponent<InputManager>();
            //Get a reference to the controller component.
            controller = GetComponent<CharacterController>();
        }

        private void UpdateMoveVector()
        {
            //Vector that moves the character along its x axis.
            var rightVector = input.Horizontal * transform.right;
            //Vector that moves the character along its z axis.
            var forwardVector = input.Vertical * transform.forward;

            //Vector that moves the character.
            moveVector = rightVector + forwardVector;
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
                //Smooth it.
                velocityDiff *= deltaTime;

                //Add it to the current velocity.
                currentVelocity += velocityDiff * airAccelerationSpeed;
            }

            //Move the controller.
            controller.Move(currentVelocity * deltaTime);
        }

        private float GetSpeed()
        {
            var speed = this.speed;
            speed *= input.IsRunning ? runMultiplier : 1f;

            return speed;
        }
    }
}
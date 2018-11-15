using UnityEngine;

namespace Finished
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField]
        private float sensitivity = 1f;

        [SerializeField]
        private Vector2 mouseYClamp = new Vector2(-80, 80);

        [Header("Crouching")]
        [Space(5f)]

        [SerializeField]
        private float crouchOffset = 0.5f;

        [SerializeField]
        private Lerper crouchLerp;

        [Header("Third Person")]
        [Space(5f)]

        [SerializeField]
        private Vector3 thirdPersonOffset;

        [SerializeField]
        private Lerper thirdPersonLerp;

        private float mouseX;
        private float mouseY;

        private InputManager input;
        private Vector3 normalPosition;

        private bool wasCrouching;
        private bool crouchTransition;

        private void Awake()
        {
            //Make the cursor invisible.
            Cursor.visible = false;
            //Lock it.
            Cursor.lockState = CursorLockMode.Locked;

            //Get a reference to the input manager.
            input = GetComponentInParent<InputManager>();

            //Save the position the camera has while standing.
            normalPosition = transform.localPosition;
        }

        private void HandleCrouching()
        {
            //If we just toggled the crouch.
            if (input.IsCrouched != wasCrouching)
            {
                //We start the transition.
                crouchTransition = true;

                //Reset the lerp.
                crouchLerp.Reset();
            }

            //Assign the crouching value for this frame.
            wasCrouching = input.IsCrouched;

            //If we need to transition.
            if (crouchTransition)
            {
                //Update the easing.
                crouchLerp.Update(Time.deltaTime);

                //Stop the transition if it's done.
                if (crouchLerp.IsDone(true))
                    crouchTransition = false;

                //Get the correct value to transition to.
                var gPosition = wasCrouching ? normalPosition - (Vector3.up * crouchOffset) : normalPosition;

                //Lerp to that value.
                transform.localPosition = Vector3.Lerp(transform.localPosition, gPosition, crouchLerp.InterpolatedValue);
            }
        }

        private void Update()
        {
            HandleCrouching();

            //If we need to change our view. 
            if(input.ThirdPerson)
            {
                //Update the lerp.
                thirdPersonLerp.Update(Time.deltaTime);

                //Lerp the position.
                transform.localPosition = Vector3.Lerp(transform.localPosition, thirdPersonOffset, thirdPersonLerp.InterpolatedValue);
            }
        }

        private void LateUpdate()
        {
            //Add the horizontal mouse input for this frame.
            mouseX += input.MouseX * sensitivity;
            //Add the vertical mouse input for this frame.
            mouseY -= input.MouseY * sensitivity;

            //Clamp the rotation on the y axis.
            mouseY = ClampRotation(mouseY, mouseYClamp.x, mouseYClamp.y);

            //Camera rotation quaternion.
            var cameraRotation = Quaternion.Euler(mouseY, 0f, 0f);
            //Player rotation quaternion.
            var playerRotation = Quaternion.Euler(0f, mouseX, 0f);

            //Assign the camera's rotation.
            transform.localRotation = cameraRotation;
            //Assign the player's rotation.
            transform.parent.localRotation = playerRotation;
        }

        private float ClampRotation(float angle, float min, float max)
        {
            if (angle > 360)
                angle -= 360;
            else if (angle < -360)
                angle += 360;

            return Mathf.Clamp(angle, min, max);
        }
    }
}
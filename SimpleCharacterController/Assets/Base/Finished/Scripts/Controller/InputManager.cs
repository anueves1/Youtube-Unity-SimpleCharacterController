using UnityEngine;

namespace Finished
{
    public class InputManager : MonoBehaviour
    {
        public bool IsRunning { get; private set; }
        public bool Jump { get; private set; }
        public bool IsCrouched { get; private set; }

        public bool ThirdPerson { get; private set; }

        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }

        public float MouseX { get; private set; }
        public float MouseY { get; private set; }

        private CharacterMotor motor;
        private CameraHandler cameraHandler;

        private void Awake()
        {
            //Get a reference to the character motor component.
            motor = GetComponent<CharacterMotor>();

            //Get a reference to the camera handler component.
            cameraHandler = GetComponentInChildren<CameraHandler>();
        }

        private void Update()
        {
            Horizontal = Input.GetAxisRaw("Horizontal");
            Vertical = Input.GetAxisRaw("Vertical");

            //Toggle third person mode when pressing the key.
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //Toggle the value.
                ThirdPerson = !ThirdPerson;

                //Event for view change.
                cameraHandler.OnViewChange();
            }

            //Toggle crouching when pressing the key.
            if (Input.GetKeyDown(KeyCode.C))
            {
                //Check if we can uncrouch, or just crouch.
                if (IsCrouched && motor.CanUncrouch() || IsCrouched == false)
                {
                    //Toggle the value.
                    IsCrouched = !IsCrouched;

                    //Let the character motor know we changed the state.
                    motor.OnToggleCrouch();

                    //Change the camera view.
                    cameraHandler.OnViewChange();
                }
            }

            //If the player is standing.
            if(IsCrouched == false)
            {
                //Jump & Run Inputs.
                Jump = Input.GetKeyDown(KeyCode.Space);
                IsRunning = Input.GetKey(KeyCode.LeftShift);
            }

            MouseX = Input.GetAxisRaw("Mouse X");
            MouseY = Input.GetAxisRaw("Mouse Y");
        }
    }
}
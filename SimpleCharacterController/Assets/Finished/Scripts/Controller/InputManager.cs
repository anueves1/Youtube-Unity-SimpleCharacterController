using UnityEngine;

namespace Finished
{
    public class InputManager : MonoBehaviour
    {
        public bool IsRunning { get; private set; }
        public bool Jump { get; private set; }

        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }

        public float MouseX { get; private set; }
        public float MouseY { get; private set; }

        private void Update()
        {
            Horizontal = Input.GetAxisRaw("Horizontal");
            Vertical = Input.GetAxisRaw("Vertical");

            Jump = Input.GetKeyDown(KeyCode.Space);
            IsRunning = Input.GetKey(KeyCode.LeftShift);

            MouseX = Input.GetAxisRaw("Mouse X");
            MouseY = Input.GetAxisRaw("Mouse Y");
        }
    }
}
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 1f;

    private float mouseX;
    private float mouseY;

    private InputManager input;

    private void Awake()
    {
        //Make the cursor invisible.
        Cursor.visible = false;
        //Lock it.
        Cursor.lockState = CursorLockMode.Locked;

        //Get a reference to the input manager.
        input = GetComponentInParent<InputManager>();
    }

    private void LateUpdate()
    {
        //Add the horizontal mouse input for this frame.
        mouseX += input.MouseX * sensitivity;
        //Add the vertical mouse input for this frame.
        mouseY -= input.MouseY * sensitivity;

        //Camera rotation quaternion.
        var cameraRotation = Quaternion.Euler(mouseY, 0f, 0f);
        //Player rotation quaternion.
        var playerRotation = Quaternion.Euler(0f, mouseX, 0f);

        //Assign the camera's rotation.
        transform.localRotation = cameraRotation;
        //Assign the player's rotation.
        transform.parent.localRotation = playerRotation;
    }
}
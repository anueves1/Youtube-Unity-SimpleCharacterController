using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_Sensivity;

    [SerializeField]
    private Vector2 m_YClamp;

    private float m_MouseX;
    private float m_MouseY;

    private InputHandler m_Input;

    private void Start()
    {
        //Get a reference to the input handler.
        m_Input = Manager.Instance.InputHandler;

        //Make the cursor invisible.
        Cursor.visible = false;

        //Lock the cursor.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        //Get the value for the mouse x.
        m_MouseX += m_Input.MouseX * m_Sensivity.x * Time.timeScale;

        //Get the value for the mouse y movement.
        m_MouseY -= m_Input.MouseY * m_Sensivity.y * Time.timeScale;

        //Clamp the y rotation.
        m_MouseY = ClampAngle(m_MouseY, m_YClamp.x, m_YClamp.y);

        //Make a quaternion with the rotation the camera should be using.
        var cameraRotation = Quaternion.Euler(m_MouseY, 0f, 0f);

        //Make a quaternion with the rotation the character should be using.
        var characterRotation = Quaternion.Euler(0f, m_MouseX, 0f);

        //Assign the rotation for the camera.
        transform.localRotation = cameraRotation;

        //Assign the rotation for the character.
        transform.parent.localRotation = characterRotation;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 360f)
            angle -= 360f;
        else if (angle < -360f)
            angle += 360f;

        return Mathf.Clamp(angle, min, max);
    }
}
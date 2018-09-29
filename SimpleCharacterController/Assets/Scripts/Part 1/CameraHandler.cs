using UnityEngine;

namespace Part1
{
    public class CameraHandler : MonoBehaviour
    {
        [Header("First Person")]
        [Space(5f)]

        [SerializeField]
        private Vector2 m_Sensivity = Vector2.one;

        [SerializeField]
        private Vector2 m_YClamp = new Vector2(-80, 80);

        [Header("Third Person")]
        [Space(5f)]

        [SerializeField]
        private Vector3 m_TPOffset;

        [SerializeField]
        private float m_TransitionSpeed = 2;

        [SerializeField]
        private bool m_CanTransitionToTP = true;

        private float m_MouseX;
        private float m_MouseY;

        private Vector3 m_FPOffset;
        private InputHandler m_Input;

        private bool m_IsFirstPerson = true;

        private void Start()
        {
            //Get a reference to the input handler.
            m_Input = Manager.Instance.InputHandler;

            //Make the cursor invisible.
            Cursor.visible = false;

            //Lock the cursor.
            Cursor.lockState = CursorLockMode.Locked;

            //Position where the camera is when we are in first person.
            m_FPOffset = transform.localPosition;
        }

        private void Update()
        {
            //If we want to change our view and we can, toggle it.
            if (m_Input.ToggleView && m_CanTransitionToTP)
                m_IsFirstPerson = !m_IsFirstPerson;

            //Get the position where the camera should be at.
            var nPosition = m_IsFirstPerson ? m_FPOffset : m_TPOffset;

            //Lerp the camera's position to the needed one.
            transform.localPosition = Vector3.Lerp(transform.localPosition, nPosition, Time.deltaTime * m_TransitionSpeed);
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
}
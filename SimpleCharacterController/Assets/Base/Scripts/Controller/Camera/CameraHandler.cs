using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 1f;

    [SerializeField]
    private float smoothValue = 1f;

    [SerializeField]
    private Vector2 mouseYClamp = new Vector2(-80, 80);

    [Header("Crouching")]
    [Space(5f)]

    [SerializeField]
    private float crouchOffset = 0.8f;

    [Header("Third Person")]
    [Space(5f)]

    [SerializeField]
    private Vector3 tpPosition = new Vector3(0, 0, -4);

    [SerializeField]
    private Lerper viewLerp;

    [SerializeField]
    private Transform body;

    private float mouseX;
    private float mouseY;

    private PlayerHandler pHandler;
    private Vector3 normalPosition;

    private bool inViewTransition;

    private void Start()
    {
        //Make the cursor invisible.
        Cursor.visible = false;
        //Lock it.
        Cursor.lockState = CursorLockMode.Locked;

        //Get a reference to the player handler.
        pHandler = Manager.Instance.PlayerHandler;

        //Save the position the camera has while standing.
        normalPosition = transform.localPosition;
    }

    public void OnViewChange()
    {
        //Reset the lerper.
        viewLerp.Reset();

        //We start the view transition.
        inViewTransition = true;
    }

    private void Update()
    {
        //Go back if we're not transitioning the view.
        if (inViewTransition == false)
            return;

        //Update the lerp.
        viewLerp.Update(Time.deltaTime);

        //If we're done with the lerp.
        if (viewLerp.IsDone(true))
            inViewTransition = false;

        //Calculate the position the camera should be at when in first person. (Based on crouch state)
        Vector3 nPosition = pHandler.IsCrouched ? normalPosition - (Vector3.up * crouchOffset) : normalPosition;

        //Goal position for the camera.
        Vector3 goalPosition = pHandler.ThirdPerson ? tpPosition : nPosition;

        //Lerp the position.
        transform.localPosition = Vector3.Lerp(transform.localPosition, goalPosition, viewLerp.InterpolatedValue);
    }

    private void LateUpdate()
    {
        //Add the horizontal mouse input for this frame.
        mouseX += pHandler.MouseX * sensitivity;
        //Add the vertical mouse input for this frame.
        mouseY -= pHandler.MouseY * sensitivity;

        //Clamp the rotation on the y axis.
        mouseY = ClampRotation(mouseY, mouseYClamp.x, mouseYClamp.y);

        Quaternion pivotRotation = Quaternion.identity;
        Quaternion cameraRotation = transform.localRotation;

        //If we're in third person.
        if (pHandler.ThirdPerson)
        {
            //Camera rotation quaternion for third person.
            pivotRotation = Quaternion.Euler(mouseY, mouseX, 0f);

            //Look at the pivot.
            transform.LookAt(transform.parent.position);
        }
        else
        {
            //Pivot rotation quaternion for first person.
            pivotRotation = Quaternion.Euler(0f, mouseX, 0f);
            //Camera rotation quaternion for first person.
            cameraRotation = Quaternion.Euler(mouseY, 0f, 0f);
        }

        //Assign the camera's rotation.
        transform.localRotation = Quaternion.Slerp(transform.localRotation, cameraRotation, Time.deltaTime * smoothValue);
        //Assign the pivot's rotation.
        transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, pivotRotation, Time.deltaTime * smoothValue);
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
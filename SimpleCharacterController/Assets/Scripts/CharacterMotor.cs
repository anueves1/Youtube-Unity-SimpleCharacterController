using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 2;

    private InputHandler m_Input;
    private CharacterController m_CharacterController;

    private void Start()
    {
        //Get a reference to the character controller.
        m_CharacterController = GetComponent<CharacterController>();

        //Get a reference to the input handler.
        m_Input = Manager.Instance.InputHandler;
    }

    private void FixedUpdate()
    {
        //Get the vector that would move the character horizontally.
        var horizontal = m_Input.Horizontal * transform.right;

        //Get the vector that would move the character vertically.
        var vertical = m_Input.Vertical * transform.forward;
        
        //Get the movement vector.
        var moveDir = horizontal + vertical;
        //Normalize the movement vector.
        moveDir.Normalize();

        //Add the speed to the movement vector.
        moveDir *= m_Speed;

        //Move in that direction.
        m_CharacterController.Move(moveDir);
    }
}
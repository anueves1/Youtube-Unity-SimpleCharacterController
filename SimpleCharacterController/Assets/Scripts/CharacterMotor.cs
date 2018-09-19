using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 0.1f;

    [SerializeField]
    private float m_RunMultiplier = 2;

    [Header("Jump")]
    [Space(5f)]

    [SerializeField]
    private float m_JumpForce = 2;

    [SerializeField]
    private float m_Gravity = 2;

    private Vector3 m_MoveDir;

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

        //If we're on the ground.
        if (m_CharacterController.isGrounded)
        {
            //Get the movement vector.
            m_MoveDir = horizontal + vertical;
            //Normalize the movement vector.
            m_MoveDir.Normalize();

            //Add the speed to the movement vector.
            m_MoveDir *= GetSpeed();

            //If we are trying to jump, call the jump function.
            if (m_Input.Jump)
            {
                //Add the jump force.
                m_MoveDir.y = m_JumpForce;
            }
        }
        //If we're not grounded, apply gravity.
        else
            m_MoveDir.y += Vector3.down.y * m_Gravity;

        //Move in that direction.
        m_CharacterController.Move(m_MoveDir);
    }

    private float GetSpeed()
    {
        var speed = m_Speed;

        //Add the running speed if needed.
        speed *= m_Input.Running ? 2f : 1f;

        return speed;
    }
}
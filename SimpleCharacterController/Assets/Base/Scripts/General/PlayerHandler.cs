using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public bool IsRunning { get; set; }
    public bool Jump { get; set; }
    public bool IsCrouched { get; set; }

    public bool ThirdPerson { get; set; }

    public float Horizontal { get; set; }
    public float Vertical { get; set; }

    public float MouseX { get; set; }
    public float MouseY { get; set; }

    public Vector3 CurrentVelocity { get; set; }
}
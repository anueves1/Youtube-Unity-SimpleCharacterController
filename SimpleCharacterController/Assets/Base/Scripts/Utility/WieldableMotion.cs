using UnityEngine;

public class WieldableMotion : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;

    [SerializeField]
    private float motionSmoothness = 2f;

    [Header("Offset settings")]
    [Space(5f)]
    
    [SerializeField]
    private Vector3 positionOffset;

    [Header("Bob Settings")]
    [Space(5f)]

    [SerializeField]
    private Vector2 bobAmplitude = new Vector2(0.7f, 1.4f);

    [SerializeField]
    private float amplitudeMultiplier = 1;

    [SerializeField]
    private Vector2 bobRate = new Vector2(0.9f, 1.8f);

    [SerializeField]
    private float rateMultiplier = 5;

    private PlayerHandler pHandler;

    private void Start()
    {
        //Get a reference to the handler.
        pHandler = Manager.Instance.PlayerHandler;
    }

    private void Update()
    {
        Vector3 positionForce = default;

        //Get the current velocity.
        Vector3 velocity = pHandler.CurrentVelocity;
        velocity.y = 0f;

        //Add the bob force.
        positionForce += GetBob(velocity);

        //Add the offset.
        positionForce += positionOffset;

        //Assign the new position.
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, positionForce, Time.deltaTime * motionSmoothness);
    }

    private Vector3 GetBob(Vector3 velocity)
    {
        var bobForce = Vector3.zero;

        //Current amplitude for the bob is the movement magnitude multiplied by the bob amplitude.
        var cAmplitude = velocity.magnitude * bobAmplitude * -0.01f * amplitudeMultiplier;

        //Calculate the bob position for this frame.
        bobForce.x = Mathf.Cos(bobRate.x * rateMultiplier * Time.time) * cAmplitude.x;
        bobForce.y = Mathf.Cos(bobRate.y * rateMultiplier * Time.time) * cAmplitude.y;

        return bobForce;
    }
}
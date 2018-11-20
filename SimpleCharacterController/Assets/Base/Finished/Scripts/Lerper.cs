using UnityEngine;

[System.Serializable]
public class Lerper
{
    public float InterpolatedValue { get; private set; }

    [SerializeField]
    private float duration = 1f;

    public void Reset()
    {
        InterpolatedValue = 0f;
    }

    public void Update(float deltaTime)
    {
        //Clamp the time.
        InterpolatedValue = Mathf.Clamp01(InterpolatedValue + (1f / duration) * deltaTime);
    }

    public bool IsDone(bool reset = false)
    {
        //Check if the easing is over.
        if (InterpolatedValue >= 1)
        {
            //Reset if needed.
            if (reset)
                Reset();

            return true;
        }

        return false;
    }
}

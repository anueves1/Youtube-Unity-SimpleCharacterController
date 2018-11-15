using UnityEngine;

[System.Serializable]
public class Lerper
{
    public float InterpolatedValue { get; private set; }

    [SerializeField]
    private float duration = 1f;

    private float time;

    public void Reset()
    {
        InterpolatedValue = 0f;

        time = 0f;
    }

    public float Update(float deltaTime)
    {
        //Clamp the time.
        time = Mathf.Clamp01(time + (1f / duration) * deltaTime);
        InterpolatedValue = time;

        return InterpolatedValue;
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

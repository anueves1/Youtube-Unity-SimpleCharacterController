using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance
    {
        get
        {
            //If there's no instance of the manager class in the scene.
            if (instance == null)
            {
                //Try to find a manager object in the scene.
                instance = FindObjectOfType<Manager>();

                //If there's none.
                if (instance == null)
                {
                    //Create a manager gameobject, and add this component to it.
                    instance = new GameObject("Manager").AddComponent<Manager>();
                }
            }

            return instance;
        }
    }

    private static Manager instance;

    public PlayerHandler PlayerHandler { get; private set; }

    private void Awake()
    {
        //Get a reference to the player's handler.
        PlayerHandler = FindObjectOfType<PlayerHandler>();
    }
}
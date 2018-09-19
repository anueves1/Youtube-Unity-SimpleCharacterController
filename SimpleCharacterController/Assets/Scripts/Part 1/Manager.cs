using UnityEngine;

namespace Part1
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance {
            get
            {
                //Try to find a manager object in the scene.
                var manager = FindObjectOfType<Manager>();

                //If there's no manager object.
                if (manager == null)
                {
                    //Create a new manager object.
                    var nObject = new GameObject("Manager");
                    //Add the manager component.
                    var nManager = nObject.AddComponent<Manager>();

                    //Use the new object as a manager.
                    manager = nManager;
                }

                return manager;
            }
        }

        public InputHandler InputHandler { get; private set; }

        private void Awake()
        {
            //Find the input handler.
            InputHandler = FindObjectOfType<InputHandler>();
        }
    }
}
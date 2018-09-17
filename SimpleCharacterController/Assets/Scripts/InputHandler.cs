﻿using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    public bool Running { get; private set; }

    private void Update()
    {
        //Get the horizontal axis value.
        Horizontal = Input.GetAxisRaw("Horizontal");
        //Get the vertical axis value.
        Vertical = Input.GetAxisRaw("Vertical");

        //Get the mouse horizontal value.
        MouseX = Input.GetAxisRaw("Mouse X");
        //Get the mouse vertical value.
        MouseY = Input.GetAxisRaw("Mouse Y");

        //Get the running state.
        Running = Input.GetKey(KeyCode.LeftShift);
    }
}
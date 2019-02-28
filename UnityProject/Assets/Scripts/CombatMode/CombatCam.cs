﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Simple camera script, slightly modified from management mode camera script
 */
public class CombatCam : MonoBehaviour
{

    int Boundary = 30;

    int theScreenWidth;
    int theScreenHeight;

    public bool rightHeld;
    public bool leftHeld;
    public bool downHeld;
    public bool upHeld;

    float maxFov = 60.0f;
    float minFov = 10.0f;
    public float sensitivity = 5.0f;
    public bool CameraMovementEnabled = true;

    public float fov;


    void Start()
    {
        fov = Camera.main.fieldOfView;
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
    }

    void Update()
    {
        if (CameraMovementEnabled)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                rightHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                rightHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                leftHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                leftHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                upHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                upHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                downHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                downHeld = false;
            }

            if (rightHeld)
            {
                transform.position += new Vector3(0.005f * fov,
                    0.0f, 0.0f);
            }

            if (leftHeld)
            {
                transform.position += new Vector3(-(0.005f * fov),
                    0.0f, 0.0f);
            }

            if (upHeld)
            {
                transform.position += new Vector3(0.0f,
                    0.0f, 0.005f * fov);
            }

            if (downHeld)
            {
                transform.position += new Vector3(0.0f, 0.0f, -(0.005f * fov));
            }


            fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }

    }
}
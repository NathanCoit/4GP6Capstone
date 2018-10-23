﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {

	int Boundary = 30;

	int theScreenWidth;
	int theScreenHeight;

	bool rightHeld;
	bool leftHeld;
	bool downHeld;
	bool upHeld;

	float maxFov = 60.0f;
	float minFov = 10.0f;
	public float sensitivity = 5.0f;

	float fov = Camera.main.fieldOfView;


	void Start() 
	{
		theScreenWidth = Screen.width;
		theScreenHeight = Screen.height;
	}

	void Update() 
	{
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			rightHeld = true;
		}

		if (Input.GetKeyUp(KeyCode.RightArrow)){
			rightHeld = false;
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			leftHeld = true;
		}

		if (Input.GetKeyUp(KeyCode.LeftArrow)){
			leftHeld = false;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			upHeld = true;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)){
			upHeld = false;
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			downHeld = true;
		}

		if (Input.GetKeyUp(KeyCode.DownArrow)){
			downHeld = false;
		}

		if (Input.mousePosition.x > theScreenWidth - Boundary || rightHeld)
		{
			transform.position += new Vector3(Time.deltaTime * fov,
				0.0f, 0.0f);
		}

		if (Input.mousePosition.x < 0 + Boundary || leftHeld)
		{
			transform.position += new Vector3(-(Time.deltaTime * fov),
				0.0f, 0.0f);
		}

		if (Input.mousePosition.y > theScreenHeight - Boundary || upHeld)
		{
			transform.position += new Vector3(0.0f,
				0.0f, Time.deltaTime * fov);
		}

		if (Input.mousePosition.y < 0 + Boundary || downHeld)
		{
			transform.position += new Vector3(0.0f,
				0.0f, -(Time.deltaTime * fov));
		}


		fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
		fov = Mathf.Clamp(fov, minFov, maxFov);
		Camera.main.fieldOfView = fov;

	}

}

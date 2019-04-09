using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {

	private int Boundary = 30;

	private int theScreenWidth;
	private int theScreenHeight;

	public bool rightHeld;
	public bool leftHeld;
	public bool downHeld;
	public bool upHeld;

	private float maxFov = 80.0f;
	private float minFov = 10.0f;
	public float sensitivity = 7.0f;
    public GameObject gameManagerObject;
    private GameManager gameManagerScript;
    public bool CameraMovementEnabled = true;

    public float fov;


	void Start() 
	{
        maxFov = maxFov + maxFov * 0.1f * gameManagerScript.PlayerFaction.GodTier;
        fov = Camera.main.fieldOfView;
        theScreenWidth = Screen.width;
		theScreenHeight = Screen.height;
        // Move starting camera position to viewing player village
        // -30 on the z coordinate points camera at starting village with current camera angle
        transform.position = new Vector3(gameManagerScript.PlayerVillage.ObjectPosition.x, 50, gameManagerScript.PlayerVillage.ObjectPosition.z - 30f);
	}

    private void Awake()
    {
        gameManagerScript = gameManagerObject.GetComponent<GameManager>();
    }

    void Update() 
	{
        Vector3 NewCameraPosition = transform.position;
		if(CameraMovementEnabled)
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

            if (Input.mousePosition.x > theScreenWidth - Boundary || rightHeld)
            {
                NewCameraPosition += new Vector3(Time.deltaTime * fov,
                    0.0f, 0.0f);
            }

            if (Input.mousePosition.x < 0 + Boundary || leftHeld)
            {
                NewCameraPosition += new Vector3(-(Time.deltaTime * fov),
                    0.0f, 0.0f);
            }

            if (Input.mousePosition.y > theScreenHeight - Boundary || upHeld)
            {
                NewCameraPosition += new Vector3(0.0f,
                    0.0f, Time.deltaTime * fov);
            }

            if (Input.mousePosition.y < 0 + Boundary || downHeld)
            {
                NewCameraPosition += new Vector3(0.0f,
                    0.0f, -(Time.deltaTime * fov));
            }

            // Camera movement occurred, validate movement
            if(NewCameraPosition != transform.position)
            {
                // Check if movement will put camera out of bounding area
                if(Vector3.Distance(NewCameraPosition, Vector3.zero - new Vector3(0, 0, 30f)) < (gameManagerScript.MapRadius / 2) / gameManagerScript.MapTierCount * (gameManagerScript.CurrentTier+1))
                {
                    transform.position = NewCameraPosition;
                }
                else if(Vector3.Distance(NewCameraPosition, Vector3.zero - new Vector3(0, 0, 30f)) < Vector3.Distance(transform.position, Vector3.zero - new Vector3(0, 0, 30f)))
                {
                    // Camera is out of bounding area, check if player is trying to move back into bounding area
                    // Prevents camera from being locked if it leaves bounding area
                    transform.position = NewCameraPosition;
                }
            }

            fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }

	}

}

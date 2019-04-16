using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for controlling interactions between the player/game and the camera
/// Allows for centralization of all camera related functionality
/// </summary>
public class Cam : MonoBehaviour
{
    public bool CameraMovementEnabled = true;
    public float Sensitivity = 7.0f;
    public GameObject GameManagerObject;
    public float FieldOfView;
    public bool RightHeld;
    public bool LeftHeld;
    public bool DownHeld;
    public bool UpHeld;

    private readonly int mintBoundary = 30;
    private readonly float mfMinFov = 10.0f;

    private int mintScreenWidth;
    private int mintScreenHeight;
    private float mfMaxFov = 80.0f;
    private GameManager mmusGameManagerScript;

    private void Awake()
    {
        mmusGameManagerScript = GameManagerObject.GetComponent<GameManager>();
    }

    void Start()
    {
        // Scale max fov with tier as map has increased in explorable size
        mfMaxFov = mfMaxFov + mfMaxFov * 0.1f * mmusGameManagerScript.PlayerFaction.GodTier;
        FieldOfView = Camera.main.fieldOfView;
        mintScreenWidth = Screen.width;
        mintScreenHeight = Screen.height;
        // Move starting camera position to viewing player village
        // -30 on the z coordinate points camera at starting village with current camera angle
        transform.position = new Vector3(mmusGameManagerScript.PlayerVillage.ObjectPosition.x, 50, mmusGameManagerScript.PlayerVillage.ObjectPosition.z - 30f);
    }

    void Update()
    {
        Vector3 uniNewCameraPositionVector3 = transform.position;
        if (CameraMovementEnabled)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RightHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                RightHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LeftHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                LeftHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                UpHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                UpHeld = false;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                DownHeld = true;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                DownHeld = false;
            }

            if (Input.mousePosition.x > mintScreenWidth - mintBoundary || RightHeld)
            {
                uniNewCameraPositionVector3 += new Vector3(Time.deltaTime * FieldOfView,
                    0.0f, 0.0f);
            }

            if (Input.mousePosition.x < 0 + mintBoundary || LeftHeld)
            {
                uniNewCameraPositionVector3 += new Vector3(-(Time.deltaTime * FieldOfView),
                    0.0f, 0.0f);
            }

            if (Input.mousePosition.y > mintScreenHeight - mintBoundary || UpHeld)
            {
                uniNewCameraPositionVector3 += new Vector3(0.0f,
                    0.0f, Time.deltaTime * FieldOfView);
            }

            if (Input.mousePosition.y < 0 + mintBoundary || DownHeld)
            {
                uniNewCameraPositionVector3 += new Vector3(0.0f,
                    0.0f, -(Time.deltaTime * FieldOfView));
            }

            // Camera movement occurred, validate movement
            if (uniNewCameraPositionVector3 != transform.position)
            {
                // Check if movement will put camera out of bounding area
                if (Vector3.Distance(uniNewCameraPositionVector3, Vector3.zero - new Vector3(0, 0, 30f)) < (mmusGameManagerScript.MapRadius / 2) / mmusGameManagerScript.MapTierCount * (mmusGameManagerScript.CurrentTier + 1))
                {
                    transform.position = uniNewCameraPositionVector3;
                }
                else if (Vector3.Distance(uniNewCameraPositionVector3, Vector3.zero - new Vector3(0, 0, 30f)) < Vector3.Distance(transform.position, Vector3.zero - new Vector3(0, 0, 30f)))
                {
                    // Camera is out of bounding area, check if player is trying to move back into bounding area
                    // Prevents camera from being locked if it leaves bounding area
                    transform.position = uniNewCameraPositionVector3;
                }
            }

            FieldOfView -= Input.GetAxis("Mouse ScrollWheel") * Sensitivity;
            FieldOfView = Mathf.Clamp(FieldOfView, mfMinFov, mfMaxFov);
            Camera.main.fieldOfView = FieldOfView;
        }

    }

    /// <summary>
    /// Make camera look at object
    /// </summary>
    /// <param name="puniGameObject"></param>
    private void LookAtObject(GameObject puniGameObject)
    {
        if(Vector3.Distance(puniGameObject.transform.position, Vector3.zero - new Vector3(0, 0, 30f)) 
            < (mmusGameManagerScript.MapRadius / 2) / mmusGameManagerScript.MapTierCount * (mmusGameManagerScript.CurrentTier + 1))
        {
            transform.position = new Vector3(puniGameObject.transform.position.x, 50, puniGameObject.transform.position.z - 30f);
        }
    }

    public void CentreOnGod()
    {
        LookAtObject(mmusGameManagerScript.PlayerGod.PlayerGod);
    }

    public void CentreOnVillage()
    {
        LookAtObject(mmusGameManagerScript.PlayerVillage.MapGameObject);
    }

}

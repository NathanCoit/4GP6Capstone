using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Simple camera script, slightly modified from management mode camera script
 */
public class CombatCam : MonoBehaviour
{
    private MapManager MapMan; // used to get size of tile map so we can determine boundary
    private BoardManager BoardMan; //Needed for find the average postion of units
    private bool start; // really awful way of avoiding organising awake/start stuff (for now!!)
    private int xBoundary;
    private int zBoundary;

    int theScreenWidth;
    int theScreenHeight;

    public bool rightHeld;
    public bool leftHeld;
    public bool downHeld;
    public bool upHeld;

    //Stuff for moving camera to units
    private Vector3 camSmoothDampV;
    public float targetTolerance;
    private Vector3 cameraTarget;
    public bool cameraMoving;

    float maxFov = 60.0f;
    float minFov = 10.0f;
    public float sensitivity = 5.0f;
    public bool CameraMovementEnabled = true;

    public float fov;

    // i tried putting this here and the boundaries in start - didn't work :(
    private void Awake()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }

    void Start()
    {
        fov = Camera.main.fieldOfView;
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
        start = true;
    }

    void Update()
    {
        
        if (start)
        {
            xBoundary = MapMan.tiles.GetLength(0);
            zBoundary = MapMan.tiles.GetLength(1) / 2;
            start = false;
        }

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

            if (rightHeld && transform.position.x < xBoundary)
            {
                transform.position += new Vector3(0.005f * fov,
                    0.0f, 0.0f);

                // set position to boundary incase above line caused it to go past boundary
                if (transform.position.x > xBoundary)
                    transform.position = new Vector3(xBoundary * 1.0f,
                        transform.position.y, transform.position.z);
            }

            if (leftHeld && transform.position.x > 0)
            {
                transform.position += new Vector3(-(0.005f * fov),
                    0.0f, 0.0f);

                if (transform.position.x < 0)
                    transform.position = new Vector3(0.0f,
                        transform.position.y, transform.position.z);
            }

            if (upHeld && transform.position.z < zBoundary)
            {
                transform.position += new Vector3(0.0f,
                    0.0f, 0.005f * fov);

                if (transform.position.z > zBoundary)
                    transform.position = new Vector3(transform.position.x,
                        transform.position.y, zBoundary * 1.0f);
            }

            // arbitrarily set to -3 atm but it seems like a good distance
            // from the bottom of the map
            if (downHeld && transform.position.z > -3)
            {
                transform.position += new Vector3(0.0f, 0.0f, -(0.005f * fov));

                if (transform.position.z < -3)
                    transform.position = new Vector3(transform.position.x,
                        transform.position.y, -3.0f);
            }

            fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }

        if (cameraMoving)
        {
            if (!(System.Math.Abs(Camera.main.transform.position.x - cameraTarget.x) < targetTolerance) || !(System.Math.Abs(Camera.main.transform.position.z - cameraTarget.z) < targetTolerance))
            {
                Camera.main.transform.position = Vector3.SmoothDamp(
                    Camera.main.transform.position, cameraTarget, ref camSmoothDampV, 0.1f);
            }
            else
                cameraMoving = false;
        }






    }

    //Reset Camera average postion of units on the grid
    public void resetCamera()
    {
        float xSum = 0;
        float zSum = 0;
        int unitsCounted = 0;
        foreach(Unit u in BoardMan.playerUnits)
        {
            if(u.getPos().x != -1 && u.getPos().y != -1)
            {
                xSum += u.unitGameObject().transform.position.x;
                zSum += u.unitGameObject().transform.position.z;
                unitsCounted++;
            }
        }

        foreach (Unit u in BoardMan.enemyUnits)
        {
            if (u.getPos().x != -1 && u.getPos().y != -1)
            {
                xSum += u.unitGameObject().transform.position.x;
                zSum += u.unitGameObject().transform.position.z;
                unitsCounted++;
            }
        }

        Camera.main.GetComponent<CombatCam>().lookAt(new Vector3(xSum / unitsCounted, 0, zSum / unitsCounted - 2));
    }

    //Function for setting camera target to unit when its selected
    public void lookAt(Vector3 target)
    {
        //Did some trig. This adjacent places the target in the middle of the camera
        float adjacent = 5;
        cameraTarget = new Vector3(target.x, 5, target.z - adjacent);
        cameraMoving = true;
    }

}

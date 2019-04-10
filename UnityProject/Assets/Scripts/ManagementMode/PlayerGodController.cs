using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to control interactions between the player and their god avatar.
/// The player may explore the map using their god avatar.
/// </summary>
public class PlayerGodController : MonoBehaviour
{
    public GameObject PlayerGod;
    public GameManager GameManagerScript;
    public TerrainMap GameMap;
    public FogOfWarScript FogOfWarController;

    private Vector3 muniDestinationVector3 = Vector3.zero;
    private Quaternion muniDestinationRotation = Quaternion.identity;

    public float MovementSpeed;
    private float mfRadius = 0;

    private void FixedUpdate()
    {
        if(muniDestinationVector3 != Vector3.zero)
        {
            PlayerGod.transform.position = Vector3.Lerp(PlayerGod.transform.position, muniDestinationVector3, MovementSpeed);
        }
        if(muniDestinationRotation != Quaternion.identity)
        {
            PlayerGod.transform.localRotation = Quaternion.Lerp(PlayerGod.transform.localRotation, muniDestinationRotation, 0.3f);
        }
    }

    /// <summary>
    /// Public method to set a point for the player god avatar to move to
    /// Called from the raycasting collision detection within game manager
    /// </summary>
    /// <param name="puniDestination"></param>
    public void SetPointToMoveTowards(Vector3 puniDestination)
    {
        // Check if point is within tier radius
        if (Vector3.Distance(Vector3.zero, puniDestination) > mfRadius)
        {
            // Point is out of range, calculate the vector closest that is within range
            // Get angle of point 
            float fAngleOfPoint = Vector3.Angle(new Vector3(100f, 0.5f, 0), puniDestination) * Mathf.PI / 180;
            // In third or fourth quadrant, add Pi as .angle will always return smallest vector
            if (puniDestination.z < 0)
            {
                fAngleOfPoint = 2 * Mathf.PI - fAngleOfPoint;
            }
            muniDestinationVector3 = new Vector3(mfRadius * Mathf.Cos(fAngleOfPoint), 0.5f, mfRadius * Mathf.Sin(fAngleOfPoint));
        }
        else
        {
            muniDestinationVector3 = puniDestination;
        }
        Quaternion uniOriginalRotation = PlayerGod.transform.localRotation;
        PlayerGod.transform.LookAt(muniDestinationVector3);
        muniDestinationRotation = PlayerGod.transform.localRotation;
        PlayerGod.transform.localRotation = uniOriginalRotation;
    }

    /// <summary>
    /// Method called to create and place the god object for the player.
    /// Allows same script to cover all god types
    /// </summary>
    public void CreatePlayerGod()
    {
        // Load player god and place near village
        Object uniGodObject = Resources.Load("Gods/" + GameManagerScript.PlayerFaction.Type);
        if(uniGodObject != null)
        {
            PlayerGod = (GameObject)GameObject.Instantiate(uniGodObject);
            if(PlayerGod.GetComponent<Collider>() == null)
            {
                PlayerGod.AddComponent<CapsuleCollider>().radius = 5;
                PlayerGod.GetComponent<CapsuleCollider>().height = 1;
                PlayerGod.GetComponent<CapsuleCollider>().center = new Vector3(0, 3, 0);
            }
            PlayerGod.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            PlayerGod = GameObject.CreatePrimitive(PrimitiveType.Cube);
            PlayerGod.transform.localScale.Scale(new Vector3(5,5,5));
        }
        PlayerGod.AddComponent<LineRenderer>().positionCount = 0;
        PlayerGod.transform.position = GameManagerScript.PlayerVillage.MapGameObject.transform.position + new Vector3(10, 0, 0);

        // Get the constraint radius
        mfRadius = (GameManagerScript.PlayerFaction.FactionArea[0][1] - GameManagerScript.PlayerFaction.FactionArea[0][0]) * (GameManagerScript.PlayerFaction.GodTier + 1);
        FogOfWarController.m_player = PlayerGod.transform;
    }

    /// <summary>
    /// Draw a circle around the player's god to give feedback when
    /// the player god is selected
    /// </summary>
    /// <param name="pblnTurnOn"></param>
    public void TogglePlayerOutlines(bool pblnTurnOn)
    {
        LineRenderer uniLineRenderer = PlayerGod.GetComponent<LineRenderer>();
        Vector3 uniPositionVector3;
        // Turn on building outlines
        if (pblnTurnOn)
        {
            int vertexCount = 40; // 4 vertices == square
            float lineWidth = 0.2f;
            float radius = 5;

            uniLineRenderer.useWorldSpace = false;
            uniLineRenderer.widthMultiplier = lineWidth;

            float deltaTheta = (2f * Mathf.PI) / vertexCount;
            float theta = 0f;

            uniLineRenderer.positionCount = vertexCount + 2;
            for (int i = 0; i < uniLineRenderer.positionCount; i++)
            {
                uniPositionVector3 = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
                uniLineRenderer.SetPosition(i, uniPositionVector3);
                theta += deltaTheta;
            }
        }

        // Turn off building outlines
        else
        {
            uniLineRenderer.positionCount = 0;
        }
    }
}

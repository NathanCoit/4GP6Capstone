using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGodController : MonoBehaviour
{
    public GameObject PlayerGod;
    public GameManager GameManagerScript;
    public TerrainMap GameMap;

    private Vector3 muniDestinationVector = Vector3.zero;

    public float MovementSpeed;
    private float mfRadius = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if(muniDestinationVector != Vector3.zero)
        {
            PlayerGod.transform.position = Vector3.Lerp(PlayerGod.transform.position, muniDestinationVector, MovementSpeed);
        }
    }

    public void SetPointToMoveTowards(Vector3 puniDestination)
    {
        // Check if point is within tier radius
        if (Vector3.Distance(Vector3.zero, puniDestination) > mfRadius)
        {
            // Point is out of range, calculate the vector closest that is within range
            // Get angle of point 
            float AngleOfPoint = Vector3.Angle(new Vector3(100f, 0.5f, 0), puniDestination) * Mathf.PI / 180;
            // In third or fourth quadrant, add Pi as .angle will always return smallest vector
            if (puniDestination.z < 0)
            {
                AngleOfPoint = 2 * Mathf.PI - AngleOfPoint;
            }
            muniDestinationVector = new Vector3(mfRadius * Mathf.Cos(AngleOfPoint), 0.5f, mfRadius * Mathf.Sin(AngleOfPoint));
        }
        else
        {
            muniDestinationVector = puniDestination;
        }
        
    }

    public void CreatePlayerGod()
    {
        // Load player god and place near village
        PlayerGod = GameObject.CreatePrimitive(PrimitiveType.Cube);
        PlayerGod.AddComponent<LineRenderer>().positionCount = 0;
        PlayerGod.transform.position = GameManagerScript.PlayerVillage.BuildingObject.transform.position + new Vector3(10, 0, 0);

        // Get the constraint radius
        mfRadius = (GameManagerScript.PlayerFaction.FactionArea[0][1] - GameManagerScript.PlayerFaction.FactionArea[0][0]) * (GameManagerScript.PlayerFaction.GodTier + 1);
    }

    public void TogglePlayerOutlines(bool pblnTurnOn)
    {
        LineRenderer lineRenderer = PlayerGod.GetComponent<LineRenderer>();
        Vector3 pos;
        // Turn on building outlines
        if (pblnTurnOn)
        {
            int vertexCount = 40; // 4 vertices == square
            float lineWidth = 0.2f;
            float radius = 1.0f;

            lineRenderer.useWorldSpace = false;
            lineRenderer.widthMultiplier = lineWidth;

            float deltaTheta = (2f * Mathf.PI) / vertexCount;
            float theta = 0f;

            lineRenderer.positionCount = vertexCount + 2;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                pos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
                lineRenderer.SetPosition(i, pos);
                theta += deltaTheta;
            }
        }

        // Turn off building outlines
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGodController : MonoBehaviour
{
    public GameObject PlayerGod;
    public GameManager GameManagerScript;
    public TerrainMap GameMap;

    private Vector3 muniDestinationVector = Vector3.zero;
    private Quaternion muniDestinationRotation = Quaternion.identity;

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
        if(muniDestinationRotation != Quaternion.identity)
        {
            PlayerGod.transform.localRotation = Quaternion.Lerp(PlayerGod.transform.localRotation, muniDestinationRotation, 0.3f);
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
        Quaternion uniOriginalRotation = PlayerGod.transform.localRotation;
        PlayerGod.transform.LookAt(muniDestinationVector);
        muniDestinationRotation = PlayerGod.transform.localRotation;
        PlayerGod.transform.localRotation = uniOriginalRotation;
    }

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
            float radius = 5;

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

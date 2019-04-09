using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarScript : MonoBehaviour
{

    public GameObject m_fogOfWarPlane;
    public Transform m_player;
    public LayerMask m_fogLayer;
    public Faction PlayerFaction;
    public float m_radius = 5f;
    private float m_radiusSqr { get { return m_radius * m_radius; } }

    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private Color[] m_colors;

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(m_player.position + new Vector3(0,50,0), new Vector3(0, -1, 0));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 1000, m_fogLayer, QueryTriggerInteraction.Collide))
        {
            for (int i = 0; i < m_vertices.Length; i++)
            {
                Vector3 v = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
                float dist = Vector3.SqrMagnitude(v - hit.point);
                if (dist < m_radiusSqr)
                {
                    float alpha = Mathf.Min(m_colors[i].a, dist / m_radiusSqr);
                    m_colors[i].a = alpha;
                }
            }
            UpdateColor();
        }
    }

    void Initialize()
    {
        m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        m_vertices = m_mesh.vertices;
        m_colors = new Color[m_vertices.Length];
        for (int i = 0; i < m_colors.Length; i++)
        {
            m_colors[i] = Color.black;
        }
        RevealPlayerAreas();
        UpdateColor();
    }

    void UpdateColor()
    {
        m_mesh.colors = m_colors;
    }

    public void RevealPlayerAreas()
    {
        Vector3 univec3;
        float fDist;
        float fAngle;
        bool blnPointInArea = false;
        for(int i = 0; i < m_vertices.Length; i++)
        {
            univec3 = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);

            fDist = Vector3.SqrMagnitude(univec3 - Vector3.zero);
            fAngle = Vector3.Angle(new Vector3(100f, 0.5f, 0), univec3) * Mathf.PI / 180;
            if (univec3.z < 0)
            {
                fAngle = 2 * Mathf.PI - fAngle;
            }
            blnPointInArea = false;
            foreach(float[] arrFactionArea in PlayerFaction.FactionArea)
            {
                if ((fAngle > arrFactionArea[2] && fAngle < arrFactionArea[3])
                    && fDist > arrFactionArea[0]* arrFactionArea[0] && fDist < arrFactionArea[1] * arrFactionArea[1])
                {
                    blnPointInArea = true;
                    break;
                }
            }
            if(blnPointInArea)
            {
                m_colors[i].a = 0;
            }
        }
    }
}
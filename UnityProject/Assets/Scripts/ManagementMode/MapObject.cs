using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapObject
{
    public enum MapObjectType
    {
        Treasure,
        Building
    }
    public MapObjectType ObjectType;
    public GameObject MapGameObject;
    static public float ObjectRadius = 10f; // Radius around a building that can not be built on, overriden by gamemanager

    public Vector3 ObjectPosition
    {
        get
        {
            return MapGameObject.transform.position;
        }
        set
        {
            MapGameObject.transform.position = value;
        }
    }

    public void ToggleObjectOutlines(bool pblnTurnOn)
    {
        LineRenderer lineRenderer = MapGameObject.GetComponent<LineRenderer>();
        Vector3 pos;
        // Turn on object outlines
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

        // Turn off object outlines
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
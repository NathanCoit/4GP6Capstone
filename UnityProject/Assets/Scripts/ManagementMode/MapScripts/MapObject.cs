using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// The base object inherited by all objects placed on the game map
/// Used to simplify the calculations of where new buildings can be by
/// aggregating all map objects into one type
/// </summary>
public class MapObject
{
    public enum MapObjectType
    {
        Treasure,
        Building
    }
    public MapObjectType ObjectType;
    public GameObject MapGameObject;
    static public float ObjectRadius = 10f; // Radius around an object that can not be built on, overriden by gamemanager

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

    /// <summary>
    /// Toggle the outlines around an object to allow the player
    /// to see the area that an object occupies
    /// </summary>
    /// <param name="pblnTurnOn"></param>
    public void ToggleObjectOutlines(bool pblnTurnOn)
    {
        LineRenderer uniLineRenderer = MapGameObject.GetComponent<LineRenderer>();
        Vector3 uniPositionVector3;
        // Turn on object outlines
        if (pblnTurnOn)
        {
            int intVertexCount = 40; // 4 vertices == square
            float fLineWidth = 0.2f;
            float fRadius = 1.0f;

            uniLineRenderer.useWorldSpace = false;
            uniLineRenderer.widthMultiplier = fLineWidth;

            float fDeltaTheta = (2f * Mathf.PI) / intVertexCount;
            float fTheta = 0f;

            uniLineRenderer.positionCount = intVertexCount + 2;
            for (int i = 0; i < uniLineRenderer.positionCount; i++)
            {
                uniPositionVector3 = new Vector3(fRadius * Mathf.Cos(fTheta), 0f, fRadius * Mathf.Sin(fTheta));
                uniLineRenderer.SetPosition(i, uniPositionVector3);
                fTheta += fDeltaTheta;
            }
        }

        // Turn off object outlines
        else
        {
            uniLineRenderer.positionCount = 0;
        }
    }
}
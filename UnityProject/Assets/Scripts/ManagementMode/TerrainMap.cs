﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The terrain map object holding methods for interacting with the map object
/// </summary>
public class TerrainMap
{
    private GameObject mgobjTerrainMap;
    private List<Building> marrBuildingsOnMap;
    private List<GameObject> LineDrawers;
    private System.Random randomNumGenerator = new System.Random();



    public TerrainMap(float pfMapRadius, Texture mapTexture)
    {
        mgobjTerrainMap = CreateTerrainObject(pfMapRadius, mapTexture);
        marrBuildingsOnMap = new List<Building>();
        LineDrawers = new List<GameObject>();
    }


    private GameObject CreateTerrainObject(float pfMapRadius, Texture mapTexture)
    {
        GameObject gobjMap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gobjMap.transform.localScale = new Vector3(pfMapRadius, 0.1f, pfMapRadius);
        GameObject.Destroy(gobjMap.GetComponent<CapsuleCollider>());
        gobjMap.AddComponent<MeshCollider>();
        gobjMap.AddComponent<LineRenderer>().positionCount = 0;
        gobjMap.GetComponent<Renderer>().material.mainTexture = mapTexture;
        gobjMap.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(40, 40));
        return gobjMap;
    }
    public void PlaceSavedFactionBuildings(GameInfo.SavedBuilding[] BuildingsToPlace, Faction OwningFaction)
    {
        Building loadedBuilding = null;
        foreach(GameInfo.SavedBuilding buildingToPlace in BuildingsToPlace)
        {
            switch (buildingToPlace.BuildingType)
            {
                case Building.BUILDING_TYPE.MATERIAL:
                    loadedBuilding = new MineBuilding(buildingToPlace, OwningFaction);
                    break;
                default:
                    loadedBuilding = new Building(buildingToPlace, OwningFaction);
                    break;
            }
            PlaceBuilding(loadedBuilding, new Vector3(buildingToPlace.x, buildingToPlace.y, buildingToPlace.z), true);
        }
    }
    public bool PlaceBuilding(Building pBuildingToPlace, Vector3 pvec3PointToPlace, bool IgnoreOtherBuildings = false)
    {
        bool blnCanPlace = true;
        //Attempt to place building and return result
        // Check if trying to place too close to another building
        if(!IgnoreOtherBuildings)
        {
            blnCanPlace = CheckForValidPlacementPoint(pvec3PointToPlace, pBuildingToPlace.OwningFaction.FactionArea);
        }
        else
        {
            blnCanPlace = true;
        }

        if (blnCanPlace)
        {
            marrBuildingsOnMap.Add(pBuildingToPlace);
            pBuildingToPlace.BuildingPosition = pvec3PointToPlace;
            pBuildingToPlace.OwningFaction.OwnedBuildings.Add(pBuildingToPlace);
        }
        return blnCanPlace;
    }

    public bool CheckForValidPlacementPoint(Vector3 pvec3PointToPlace, List<float[]> parrFactionAreas)
    {
        bool blnCanPlace = true;
        bool blnInAnArea = false;
        float DistanceBetweenBuildings = 0f;
        float AngleOfPlacement = 0f;
        float RadiusOfPlacement = 0f;

        foreach (Building BuildingOnMap in marrBuildingsOnMap)
        {
            DistanceBetweenBuildings = Vector3.Distance(pvec3PointToPlace, BuildingOnMap.BuildingPosition);
            if (DistanceBetweenBuildings < Building.BuildingRadiusSize * 2)
            {
                blnCanPlace = false;
            }
        }

        RadiusOfPlacement = Vector3.Distance(new Vector3(0, 0.5f, 0), pvec3PointToPlace);

        AngleOfPlacement = Vector3.Angle(new Vector3(100f, 0.5f, 0), pvec3PointToPlace) * Mathf.PI / 180;
        // In third or fourth quadrant, add Pi as .angle will always return smallest vector
        if (pvec3PointToPlace.z < 0)
        {
            AngleOfPlacement = 2 * Mathf.PI - AngleOfPlacement;
        }

        foreach (float[] playerArea in parrFactionAreas)
        {
            // Check if you are placing in your own area.
            if ((AngleOfPlacement > playerArea[2] && AngleOfPlacement < playerArea[3])
                && RadiusOfPlacement > playerArea[0] && RadiusOfPlacement < playerArea[1])
            {
                blnInAnArea = true;
            }
        }

        return blnInAnArea && blnCanPlace;
    }

    public List<Building> GetBuildings()
    {
        return marrBuildingsOnMap;
    }

    public GameObject GetMapObject()
    {
        return mgobjTerrainMap;
    }

    public void RemoveBuilding(Building pmusBuildingToRemove)
    {
        if(pmusBuildingToRemove != null)
        {
            marrBuildingsOnMap.Remove(pmusBuildingToRemove);
        }
    }

    public void DivideMap(List<Faction> parrCurrentFactions, float pfStartingRad, float pfEndingRad)
    {
        //GameObject uniLandObject;
        float fFullCircleRad = 2 * Mathf.PI;
        float fAreaAngle = fFullCircleRad / parrCurrentFactions.Count;
        float fAngle = 0;
        //int intPosx = 1;
        //int intPosy = 1;
        foreach (Faction FactionToPlace in parrCurrentFactions)
        {
            FactionToPlace.FactionArea = new List<float[]>();
            FactionToPlace.FactionArea.Add(new float[] { pfStartingRad, pfEndingRad, fAngle, fAngle + fAreaAngle });

            fAngle += fAreaAngle;
        }
    }

    public void AddGodLandscapes(List<Faction> parrCurrentFactions)
    {
        GameObject uniLandObject;
        int intPosx = 1;
        int intPosy = 1;
        foreach (Faction FactionToPlace in parrCurrentFactions)
        {
            foreach(float[] farrFactionArea in FactionToPlace.FactionArea)
            {
                if (farrFactionArea[0] == 0)
                {
                    intPosx = (farrFactionArea[0] + farrFactionArea[1]) / 2 * Mathf.Cos((farrFactionArea[2] + farrFactionArea[3]) / 2) > 0 ? 1 : -1;
                    intPosy = (farrFactionArea[0] + farrFactionArea[1]) / 2 * Mathf.Sin((farrFactionArea[2] + farrFactionArea[3]) / 2) > 0 ? 1 : -1;
                    // pie shaped area, place pie area
                    uniLandObject = (GameObject)GameObject.Instantiate(Resources.Load("GodLands/" + FactionToPlace.Type.ToString() + "Pie"));
                    uniLandObject.transform.position = new Vector3(0, 0.15f, 0);
                    uniLandObject.transform.Rotate(new Vector3(0, ((farrFactionArea[2] + farrFactionArea[3]) / 2 * -180 / Mathf.PI) - 45, 0));
                    uniLandObject.transform.localScale = new Vector3(82.5f, 82.5f, 82.5f);
                }
                else if (farrFactionArea[0] > 80 && farrFactionArea[0] < 85)
                {
                    // Rounded rectangle
                    uniLandObject = (GameObject)GameObject.Instantiate(Resources.Load("GodLands/" + FactionToPlace.Type.ToString() + "RoundedRect"));
                    uniLandObject.transform.position = new Vector3(0, 0.15f, 0);
                    uniLandObject.transform.Rotate(new Vector3(0, -((farrFactionArea[2] + farrFactionArea[3]) / 2 * 180 / Mathf.PI) - 30, 0));
                    uniLandObject.transform.localScale = new Vector3(82.5f, 82.5f, 82.5f);
                }
                else
                {
                    // Rounded rect3
                    uniLandObject = (GameObject)GameObject.Instantiate(Resources.Load("GodLands/" + FactionToPlace.Type.ToString() + "RoundedRect3"));
                    uniLandObject.transform.position = new Vector3(0, 0.15f, 0);
                    uniLandObject.transform.Rotate(new Vector3(0, -((farrFactionArea[2] + farrFactionArea[3]) / 2 * 180 / Mathf.PI) - 30, 0));
                    uniLandObject.transform.localScale = new Vector3(82.5f, 82.5f, 82.5f);
                }
            }
        }
    }

    public void DrawFactionArea(Faction faction)
    {
        OutlineFaction(faction);
    }

    public void HideMap()
    {
        Renderer uniMapRenderer = mgobjTerrainMap.GetComponent<Renderer>();
        uniMapRenderer.enabled = false;
    }

    public void DrawMultipleFactionAreas(List<Faction> factions)
    {
        foreach (Faction faction in factions)
        {
            OutlineFaction(faction);
        }
    }

    public Vector3 CalculateRandomPosition(Faction pobjFactionToPlace)
    {
        Vector3 vec3StartingPosition = new Vector3(0, 0, 0);
        int areaIndex = randomNumGenerator.Next(pobjFactionToPlace.FactionArea.Count);
        float[] FactionArea = pobjFactionToPlace.FactionArea[areaIndex];

        float fAngle = Random.Range(FactionArea[2] + (Building.BuildingRadiusSize / 100f), FactionArea[3] - (Building.BuildingRadiusSize / 100f));
        float fRad = Random.Range(FactionArea[0] + Building.BuildingRadiusSize, FactionArea[1] - Building.BuildingRadiusSize);

        vec3StartingPosition = new Vector3(fRad * Mathf.Cos(fAngle), 1.5f, fRad * Mathf.Sin(fAngle));
        return vec3StartingPosition;
    }

    private void OutlineFaction(Faction faction)
    {
        LineRenderer PlayerBoundsLineRenderer;
        Vector3 vec3LinePosition;
        List<float[]> PlayerArea;
        LineDrawers = new List<GameObject>();
        GameObject LineDrawer;
        float angleOfPoint;
        float deltaTheta;

        PlayerArea = faction.FactionArea;
        foreach (float[] playerArea in PlayerArea)
        {
            LineDrawer = new GameObject("LineDrawer");
            LineDrawer.transform.parent = mgobjTerrainMap.transform;
            LineDrawers.Add(LineDrawer);
            PlayerBoundsLineRenderer = LineDrawer.AddComponent<LineRenderer>();
            PlayerBoundsLineRenderer.useWorldSpace = true;
            PlayerBoundsLineRenderer.widthMultiplier = 0.2f;

            if (playerArea[0] == 0)
            {
                // In starting tier, pie shape
                PlayerBoundsLineRenderer.positionCount = 33;
                vec3LinePosition = new Vector3(playerArea[1] * Mathf.Cos(playerArea[3]), 0.5f, playerArea[1] * Mathf.Sin(playerArea[3]));
                PlayerBoundsLineRenderer.SetPosition(0, vec3LinePosition);

                vec3LinePosition = new Vector3(0, 0.5f, 0);
                PlayerBoundsLineRenderer.SetPosition(1, vec3LinePosition);

                vec3LinePosition = new Vector3(playerArea[1] * Mathf.Cos(playerArea[2]), 0.5f, playerArea[1] * Mathf.Sin(playerArea[2]));
                PlayerBoundsLineRenderer.SetPosition(2, vec3LinePosition);

                deltaTheta = (playerArea[3] - playerArea[2]) / 30;
                angleOfPoint = playerArea[2] + deltaTheta;
                for (int i = 0; i < 30; i++)
                {
                    vec3LinePosition = new Vector3(playerArea[1] * Mathf.Cos(angleOfPoint), 0.5f, playerArea[1] * Mathf.Sin(angleOfPoint));
                    PlayerBoundsLineRenderer.SetPosition(i + 3, vec3LinePosition);
                    angleOfPoint += deltaTheta;
                }
            }
            else
            {
                // An outer tier, rounded rectangle
                PlayerBoundsLineRenderer.positionCount = 64;

                vec3LinePosition = new Vector3(playerArea[1] * Mathf.Cos(playerArea[3]), 0.5f, playerArea[1] * Mathf.Sin(playerArea[3]));
                PlayerBoundsLineRenderer.SetPosition(0, vec3LinePosition);

                vec3LinePosition = new Vector3(playerArea[0] * Mathf.Cos(playerArea[3]), 0.5f, playerArea[0] * Mathf.Sin(playerArea[3]));
                PlayerBoundsLineRenderer.SetPosition(1, vec3LinePosition);

                deltaTheta = (playerArea[2] - playerArea[3]) / 30;
                angleOfPoint = playerArea[3] + deltaTheta;
                for (int i = 0; i < 30; i++)
                {
                    vec3LinePosition = new Vector3(playerArea[0] * Mathf.Cos(angleOfPoint), 0.5f, playerArea[0] * Mathf.Sin(angleOfPoint));
                    PlayerBoundsLineRenderer.SetPosition(i + 2, vec3LinePosition);
                    angleOfPoint += deltaTheta;
                }

                vec3LinePosition = new Vector3(playerArea[0] * Mathf.Cos(playerArea[2]), 0.5f, playerArea[0] * Mathf.Sin(playerArea[2]));
                PlayerBoundsLineRenderer.SetPosition(32, vec3LinePosition);

                vec3LinePosition = new Vector3(playerArea[1] * Mathf.Cos(playerArea[2]), 0.5f, playerArea[1] * Mathf.Sin(playerArea[2]));
                PlayerBoundsLineRenderer.SetPosition(33, vec3LinePosition);

                deltaTheta = (playerArea[3] - playerArea[2]) / 30;
                angleOfPoint = playerArea[2] + deltaTheta;
                for (int i = 0; i < 30; i++)
                {
                    vec3LinePosition = new Vector3(playerArea[1] * Mathf.Cos(angleOfPoint), 0.5f, playerArea[1] * Mathf.Sin(angleOfPoint));
                    PlayerBoundsLineRenderer.SetPosition(i + 34, vec3LinePosition);
                    angleOfPoint += deltaTheta;
                }
            }
        }
    }
}
 
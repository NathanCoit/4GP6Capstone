using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The terrain map object holding methods for interacting with the map object
/// </summary>
public class TerrainMap
{
    private GameObject mgobjTerrainMap;
    private List<Building> marrBuildingsOnMap;
    private Dictionary<Faction, float[]> marrFactionAreas; //Startingangle, ending angle, starting rad, ending rad 

    public TerrainMap()
    {
        mgobjTerrainMap = CreateTerrainObject();
        marrBuildingsOnMap = new List<Building>();
    }


    private GameObject CreateTerrainObject()
    {
        GameObject gobjMap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gobjMap.transform.localScale = new Vector3(50, 0.1f, 50);
        // Create map
        //TerrainData _TerrainData = new TerrainData();
        //_TerrainData.size = new Vector3(10, 10, 10);
        //_TerrainData.heightmapResolution = 512;
        //_TerrainData.baseMapResolution = 1024;
        //_TerrainData.SetDetailResolution(1024, 16);

        //TerrainCollider _TerrainCollider = gobjMap.AddComponent<TerrainCollider>();
        //Terrain _Terrain2 = gobjMap.AddComponent<Terrain>();

        //_TerrainCollider.terrainData = _TerrainData;
        //_Terrain2.terrainData = _TerrainData;

        return gobjMap;
    }

    public bool PlaceBuilding(Building pBuildingToPlace, Vector3 pvec3PointToPlace)
    {
        bool blnCanPlace = true;
        float DistanceBetweenBuildings = 0f;
        //Attempt to place building and return result
        foreach (Building BuildingOnMap in marrBuildingsOnMap)
        {
            DistanceBetweenBuildings = Vector3.Distance(pvec3PointToPlace, BuildingOnMap.BuildingPosition);
            if (DistanceBetweenBuildings < 2.0f)
            {
                blnCanPlace = false;
            }
        }
        if (blnCanPlace)
        {
            marrBuildingsOnMap.Add(pBuildingToPlace);
            pBuildingToPlace.BuildingPosition = pvec3PointToPlace;
        }
        return blnCanPlace;
    }

    public List<Building> GetBuildings()
    {
        return marrBuildingsOnMap;
    }

    public void DivideMap(List<Faction> parrCurrentFactions, float pfStartingRad, float pfEndingRad)
    {

        marrFactionAreas = new Dictionary<Faction, float[]>();
        float fFullCircleRad = 2 * Mathf.PI;
        float fAreaAngle = fFullCircleRad / parrCurrentFactions.Count;
        float fAngle = 0;
        foreach (Faction FactionToPlace in parrCurrentFactions)
        {
            marrFactionAreas.Add(FactionToPlace, new float[] { pfStartingRad, pfEndingRad, fAngle, fAngle + fAreaAngle });
            fAngle += fAreaAngle;
        }
    }

    public Vector3 CalculateStartingPosition(Faction pobjFactionToPlace)
    {
        Vector3 vec3StartingPosition = new Vector3(0, 0, 0);
        float[] FactionArea = marrFactionAreas[pobjFactionToPlace];

        float fAngle = Random.Range(FactionArea[2], FactionArea[3]);
        float fRad = Random.Range(FactionArea[0] + 2f, FactionArea[1] - 2f);

        vec3StartingPosition = new Vector3(fRad * Mathf.Cos(fAngle), 0.5f, fRad * Mathf.Sin(fAngle));
        return vec3StartingPosition;
    }
}

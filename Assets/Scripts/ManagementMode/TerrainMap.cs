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

    public TerrainMap()
    {
        mgobjTerrainMap = CreateTerrainObject();
        marrBuildingsOnMap = new List<Building>();
    }


    private GameObject CreateTerrainObject()
    {
        GameObject gobjMap = new GameObject("GameMap");
        // Create map
        TerrainData _TerrainData = new TerrainData();
        _TerrainData.size = new Vector3(10, 10, 10);
        _TerrainData.heightmapResolution = 512;
        _TerrainData.baseMapResolution = 1024;
        _TerrainData.SetDetailResolution(1024, 16);

        TerrainCollider _TerrainCollider = gobjMap.AddComponent<TerrainCollider>();
        Terrain _Terrain2 = gobjMap.AddComponent<Terrain>();

        _TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;

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
}

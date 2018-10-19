using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The terrain map object holding methods for interacting with the map object
/// </summary>
public class TerrainMap : MonoBehaviour {
    private GameObject mgobjTerrainMap;
    private List<Building> marrBuildingsOnMap;

    public TerrainMap()
    {
        mgobjTerrainMap = CreateTerrainObject();
        marrBuildingsOnMap = new List<Building>();
    }
	

    private GameObject CreateTerrainObject()
    {
        GameObject gobjMap = new GameObject("gobjMap");
        // Create map
        TerrainData _TerrainData = new TerrainData();
        _TerrainData.size = new Vector3(10, 600, 10);
        _TerrainData.heightmapResolution = 512;
        _TerrainData.baseMapResolution = 1024;
        _TerrainData.SetDetailResolution(1024, 16);

        TerrainCollider _TerrainCollider = gobjMap.AddComponent<TerrainCollider>();
        Terrain _Terrain2 = gobjMap.AddComponent<Terrain>();

        _TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;

        return gobjMap;
    }

    public bool PlaceBuilding(Building pBuildingToPlace, Vector2 pvec2PointToPlace)
    {
        bool blnCanPlace = false;
        marrBuildingsOnMap.Add(pBuildingToPlace);
        //Attempt to place building and return result

        if(blnCanPlace)
        {
            pBuildingToPlace.BuildingPosition = pvec2PointToPlace;
        }
        return blnCanPlace;
    }

    public List<Building> GetBuildings()
    {
        return marrBuildingsOnMap;
    }
}

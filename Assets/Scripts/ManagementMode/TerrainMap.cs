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
        GameObject gobjMap = null;
        // Create map

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for managing all map related actions
/// </summary>
public class MapManager : MonoBehaviour {
    public TerrainMap GameMap;
    public Building PlayerVillage;
    public List<Building> EnemyVillages;
    public int TotalEnemies;

	// Use this for initialization
	void Start () {
        Building bldEnemyBuilding = null;
        //Create map terrain
        GameMap = new TerrainMap();

        //Create and place player village
        PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE);
        Vector2 vec2VillagePos = CalculateStartingPlayerPosition();
        GameMap.PlaceBuilding(PlayerVillage, vec2VillagePos);

        //Create and place enemy villages
        for(int enemyCount = 0; enemyCount < TotalEnemies; enemyCount++)
        {
            bldEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE);
            vec2VillagePos = CalculateStartingEnemyPosiion();
            GameMap.PlaceBuilding(bldEnemyBuilding, vec2VillagePos);
            EnemyVillages.Add(bldEnemyBuilding);
        }
	}

    private Vector2 CalculateStartingEnemyPosiion()
    {
        throw new NotImplementedException();
    }

    private Vector2 CalculateStartingPlayerPosition()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update () {
		
	}
}

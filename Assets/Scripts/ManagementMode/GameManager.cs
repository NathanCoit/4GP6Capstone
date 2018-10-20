using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Game manager for the Management mode. Controls intializing of level 
/// and transitions from Combat Mode to and from Management Mode.
/// </summary>
public class GameManager : MonoBehaviour {

    public TerrainMap GameMap;
    public Building PlayerVillage;
    public List<Building> EnemyVillages;
    public int TotalEnemies;
    public List<Faction> CurrentFactions;

    // Use this for initialization
    void Start()
    {
        Building bldEnemyBuilding = null;
        Faction PlayerFaction = null;
        CurrentFactions = new List<Faction>();
        //Create map terrain
        GameMap = new TerrainMap();

        // Create the player Faction
        PlayerFaction = new Faction("YourGod");
        CurrentFactions.Add(PlayerFaction);

        //Create and place player village
        PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction);
        Vector2 vec2VillagePos = CalculateStartingPlayerPosition();
        GameMap.PlaceBuilding(PlayerVillage, vec2VillagePos);

        ////Create and place enemy villages
        //for (int enemyCount = 0; enemyCount < TotalEnemies; enemyCount++)
        //{
        //    PlayerFaction = new Faction("EnemyGod" + enemyCount);
        //    CurrentFactions.Add(PlayerFaction);
        //    bldEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction);
        //    vec2VillagePos = CalculateStartingEnemyPosiion();
        //    GameMap.PlaceBuilding(bldEnemyBuilding, vec2VillagePos);
        //    EnemyVillages.Add(bldEnemyBuilding);
        //}
    }

    private Vector2 CalculateStartingEnemyPosiion()
    {
        throw new NotImplementedException();
    }

    private Vector2 CalculateStartingPlayerPosition()
    {
        return new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game info object to be used for reading/writing and cross scene info
/// Check for the existence of this object at the beginning of a scene, if it does not yet
/// exist, create a new instance of one
/// </summary>
public class GameInfo : MonoBehaviour {
    public enum BATTLESTATUS
    {
        Victory,
        Defeat,
        Retreat
    }

    public struct SavedBuilding
    {
        public Vector3 BuildingPosition;
        public Building.BUILDING_TYPE BuildingType;
        public int UpgradeLevel;
        public int Miners;
    }
    public struct SavedFaction
    {
        public string GodName;
        public int MatieralCount;
        public int WorshipperCount;
        public float Morale;
        public List<SavedBuilding> OwnedBuildings;
        public List<float[]> FactionArea;
        public Faction.GodType Type;
        public int GodTier;
        public int RewardPoints;
        public List<string> Abilities;
    }
    // Initialize any variables that need to be stored here, give each a default value.
    // Variables shared by combat and management mode
    public int PlayerWorshipperCount = 300;
    public float PlayerMorale = 0;
    public List<string> PlayerRewards = new List<string>();
    public int EnemyWorshipperCount = 200;
    public float EnemyMorale = 0;
    public List<string> EnemyAbilites = new List<string>();
    public bool FinishedBattle = false;
    public SavedFaction EnemyFaction;
    public SavedFaction PlayerFaction;

    // Combat mode variables
    public BATTLESTATUS LastBattleStatus = BATTLESTATUS.Victory;

    // Management mode variables for loading scene
    public List<SavedFaction> SavedFactions = new List<SavedFaction>();
    public int CurrentTier = 0;
    public float MapRadius = 100f;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}

    public static SavedBuilding CreateSavedBuilding(Building buildingToSave)
    {
        SavedBuilding savedBuilding = new SavedBuilding
        {
            BuildingPosition = buildingToSave.BuildingPosition,
            BuildingType = buildingToSave.BuildingType,
            UpgradeLevel = buildingToSave.UpgradeLevel,
            Miners = buildingToSave.BuildingType == Building.BUILDING_TYPE.MATERIAL ? ((MineBuilding)buildingToSave).Miners : 0
        };

        return savedBuilding;
    }

    public static SavedFaction CreateSavedFaction(Faction factionToSave)
    {
        SavedFaction savedFaction = new SavedFaction
        {
            GodName = factionToSave.GodName,
            MatieralCount = factionToSave.MaterialCount,
            Morale = factionToSave.Morale,
            WorshipperCount = factionToSave.WorshipperCount,
            FactionArea = factionToSave.FactionArea,
            Type = factionToSave.Type,
            GodTier = factionToSave.GodTier,
            RewardPoints = factionToSave.TierRewardPoints,
            OwnedBuildings = new List<SavedBuilding>(),
            Abilities = new List<string>()
        };
        
        foreach(Ability ability in factionToSave.CurrentAbilites)
        {
            savedFaction.Abilities.Add(ability.AbilityName);
        }
        foreach (Building building in factionToSave.OwnedBuildings)
        {
            savedFaction.OwnedBuildings.Add(CreateSavedBuilding(building));
        }

        return savedFaction;
    }	
}

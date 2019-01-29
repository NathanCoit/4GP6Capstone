using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Game info object to be used for reading/writing and cross scene info
/// Check for existence of this object at beginning of scene to load data
/// </summary>
public class GameInfo : MonoBehaviour {
    public enum BATTLESTATUS
    {
        Victory,
        Defeat,
        Retreat
    }
    [System.Serializable]
    public struct SavedBuilding
    {
        public float x;
        public float y;
        public float z;
        public Building.BUILDING_TYPE BuildingType;
        public int UpgradeLevel;
        public int Miners;
    }
    [System.Serializable]
    public struct SavedArea
    {
        public float StartingRad;
        public float EndingRad;
        public float StartingAngle;
        public float EndingAngle;
    }
    [System.Serializable]
    public struct SavedFaction
    {
        public string GodName;
        public int MatieralCount;
        public int WorshipperCount;
        public float Morale;
        public SavedBuilding[] OwnedBuildings;
        public SavedArea[] FactionArea;
        public Faction.GodType Type;
        public int GodTier;
        public int RewardPoints;
        public string[] Abilities;
    }
    
    // Initialize any variables that need to be stored here, give each a default value.
    // Variables shared by combat and management mode
    public bool FinishedBattle = false;
    public SavedFaction EnemyFaction;
    public SavedFaction PlayerFaction;

    // Combat mode variables
    public BATTLESTATUS LastBattleStatus = BATTLESTATUS.Victory;

    // Management mode variables for loading scene
    public SavedFaction[] SavedFactions;
    public int CurrentTier = 0;
    public float MapRadius = 100f;
    public float PlayerMoraleCap = 1.0f;
    public bool NewGame = true;
    public bool FromSave = false;
    public string[] PlayerRewards;
    public int EnemyChallengeTimer;
    public float[] MaterialMultipliers;
    public float[] WorshipperMultipliers;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}

    // Method for serializing a Building instance.
    public static SavedBuilding CreateSavedBuilding(Building buildingToSave)
    {
        SavedBuilding savedBuilding = new SavedBuilding
        {
            x = buildingToSave.BuildingPosition.x,
            y = buildingToSave.BuildingPosition.y,
            z = buildingToSave.BuildingPosition.z,
            BuildingType = buildingToSave.BuildingType,
            UpgradeLevel = buildingToSave.UpgradeLevel,
            Miners = buildingToSave.BuildingType == Building.BUILDING_TYPE.MATERIAL ? ((MineBuilding)buildingToSave).Miners : 0
        };

        return savedBuilding;
    }

    // Method for serializing a Faction instance
    public static SavedFaction CreateSavedFaction(Faction factionToSave)
    {
        SavedFaction savedFaction = new SavedFaction
        {
            GodName = factionToSave.GodName,
            MatieralCount = factionToSave.MaterialCount,
            Morale = factionToSave.Morale,
            WorshipperCount = factionToSave.WorshipperCount,
            FactionArea = new SavedArea[factionToSave.FactionArea.Count],
            Type = factionToSave.Type,
            GodTier = factionToSave.GodTier,
            RewardPoints = factionToSave.TierRewardPoints,
            OwnedBuildings = new SavedBuilding[factionToSave.OwnedBuildings.Count],
            Abilities = new string[factionToSave.CurrentAbilites.Count]
        };
        for (int i = 0; i < factionToSave.CurrentAbilites.Count; i++)
        {
            savedFaction.Abilities[i] = factionToSave.CurrentAbilites[i].AbilityName;
        }
        for(int i = 0; i < factionToSave.OwnedBuildings.Count; i++)
        {
            savedFaction.OwnedBuildings[i] = CreateSavedBuilding(factionToSave.OwnedBuildings[i]);
        }
        for (int i = 0; i < factionToSave.FactionArea.Count; i++)
        {
            savedFaction.FactionArea[i] = CreatedSavedFactionArea(factionToSave.FactionArea[i]);
        }

        return savedFaction;
    }

    // Method for serializing an area List
    public static SavedArea CreatedSavedFactionArea(float[] parrAreatoSave)
    {
        SavedArea savedArea = new SavedArea
        {
            StartingRad = parrAreatoSave[0],
            EndingRad = parrAreatoSave[1],
            StartingAngle = parrAreatoSave[2],
            EndingAngle = parrAreatoSave[3]
        };
        return savedArea;
    }
}

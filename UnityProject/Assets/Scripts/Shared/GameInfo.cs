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
/// Used to abstract the differences between modes by having all necessary data located in one place.
/// Connects Main menu to management mode, management mode to combat mode, combat mode to management mode, and management mode to a save state.
/// Check for existence of this object at beginning of scene to load data.
/// Contains some helper methods for serializing data from different modes.
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

    [System.Serializable]
    public struct SavedTreasure
    {
        public float x;
        public float y;
        public float z;
        public Treasure.TreasureType Type;
    }
    
    // Initialize any variables that need to be stored here, give each a default value.
    // Variables shared by combat and management mode
    public bool FinishedBattle = false;
    public SavedFaction EnemyFaction;
    public SavedFaction PlayerFaction;
    public float GodHealthMultiplier = 1f;
    public float GodAttackMultiplier = 1f;
    public float GodDefenseMultiplier = 1f;

    // Combat mode variables
    public BATTLESTATUS LastBattleStatus = BATTLESTATUS.Victory;

    // Management mode variables for loading scene
    public SavedFaction[] SavedFactions;
    public SavedTreasure[] SavedTreasures;
    public int CurrentTier = 0;
    public float MapRadius = 100f;
    public float PlayerMoraleCap = 1.0f;
    public bool NewGame = true;
    public bool FromSave = false;
    public string[] PlayerRewards;
    public int EnemyChallengeTimer;
    public float[] MaterialMultipliers;
    public float[] WorshipperMultipliers;
    public InformationBoxDisplay.TutorialFlag TutorialFlag;
    public int[] WorshipperAttackBuffs;
    public int[] WorshipperDefenseBuffs;
    public int[] WorshipperMovementBuffs;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}

    // Method for serializing a Building instance.
    public static SavedBuilding CreateSavedBuilding(Building pmusBuildingToSave)
    {
        SavedBuilding musSavedBuilding = new SavedBuilding
        {
            x = pmusBuildingToSave.ObjectPosition.x,
            y = pmusBuildingToSave.ObjectPosition.y,
            z = pmusBuildingToSave.ObjectPosition.z,
            BuildingType = pmusBuildingToSave.BuildingType,
            UpgradeLevel = pmusBuildingToSave.UpgradeLevel,
            Miners = pmusBuildingToSave.BuildingType == Building.BUILDING_TYPE.MATERIAL ? ((MineBuilding)pmusBuildingToSave).Miners : 0
        };

        return musSavedBuilding;
    }

    // Method for serializing a Faction instance
    public static SavedFaction CreateSavedFaction(Faction pmusFactionToSave)
    {
        SavedFaction pmusSavedFaction = new SavedFaction
        {
            GodName = pmusFactionToSave.GodName,
            MatieralCount = pmusFactionToSave.MaterialCount,
            Morale = pmusFactionToSave.Morale,
            WorshipperCount = pmusFactionToSave.WorshipperCount,
            FactionArea = new SavedArea[pmusFactionToSave.FactionArea.Count],
            Type = pmusFactionToSave.Type,
            GodTier = pmusFactionToSave.GodTier,
            RewardPoints = pmusFactionToSave.TierRewardPoints,
            OwnedBuildings = new SavedBuilding[pmusFactionToSave.OwnedBuildings.Count],
            Abilities = new string[pmusFactionToSave.CurrentAbilites.Count]
        };
        for (int i = 0; i < pmusFactionToSave.CurrentAbilites.Count; i++)
        {
            pmusSavedFaction.Abilities[i] = pmusFactionToSave.CurrentAbilites[i].AbilityName;
        }
        for(int i = 0; i < pmusFactionToSave.OwnedBuildings.Count; i++)
        {
            pmusSavedFaction.OwnedBuildings[i] = CreateSavedBuilding(pmusFactionToSave.OwnedBuildings[i]);
        }
        for (int i = 0; i < pmusFactionToSave.FactionArea.Count; i++)
        {
            pmusSavedFaction.FactionArea[i] = CreatedSavedFactionArea(pmusFactionToSave.FactionArea[i]);
        }

        return pmusSavedFaction;
    }

    // Method for serializing an area List
    public static SavedArea CreatedSavedFactionArea(float[] parrAreatoSave)
    {
        SavedArea musSavedArea = new SavedArea
        {
            StartingRad = parrAreatoSave[0],
            EndingRad = parrAreatoSave[1],
            StartingAngle = parrAreatoSave[2],
            EndingAngle = parrAreatoSave[3]
        };
        return musSavedArea;
    }
}

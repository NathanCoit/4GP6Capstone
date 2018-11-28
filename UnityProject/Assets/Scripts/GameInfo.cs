using System.Collections;
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
    

    public static List<string> SingleTargetAbilities = new List<string>
    {
        "throwmushroom",
        "paintslap",
        "throwfork",
        "kick",
        "quack",
        "chihuahua",
        "batonslap",
        "blowakiss",
        "hammerslap",
        "fireball",
        "drown",
        "smite",
        "tornado"
    };
    public static List<string> MultiTargetAbilities = new List<string>
    {
        "mushroomlaser",
        "paintcannon",
        "forksweep",
        "legsweep",
        "quack!!",
        "corgi",
        "jazzhands",
        "giantheartslap",
        "dropanvil",
        "lavariver",
        "tsunami",
        "electricfield",
        "earthquake"
    };
    public static List<string> BuffAbilities = new List<string>
    {
        "eatmushroom",
        "warpaint",
        "eatspaghett",
        "yeezys",
        "quack?",
        "poodle",
        "saxsolo",
        "slapass",
        "sharpenarms",
        "igniteweapons",
        "stayhydrated"
    };
    public static List<string> DebuffAbilities = new List<string>
    {
        "spreadspores",
        "forkflash",
        "coloursplash",
        "brokenankles",
        "quack¿",
        "cat",
        "outoftunesolo",
        "charm",
        "armorbreak",
        "burn",
        "stun",
        "root"
    };
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
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}

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

    public static Ability LoadAbility(string pstrAbilityName)
    {
        Ability loadedAbility = null;
        string strFormattedAbilityName = pstrAbilityName.ToLower().Replace(" ", string.Empty);

        if (SingleTargetAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new SingleTargetAbility(strFormattedAbilityName);
        }
        else if(MultiTargetAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new MultiTargetAbility(pstrAbilityName);
        }
        else if (BuffAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new BuffAbility(pstrAbilityName);
        }
        else if (DebuffAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new DebuffAbility(pstrAbilityName);
        }

        return loadedAbility;
    }
}

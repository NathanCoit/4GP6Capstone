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

    // Initialize any variables that need to be stored here, give each a default value.
    // Variables shared by combat and management mode
    public int PlayerWorshipperCount = 0;
    public float PlayerMorale = 0;
    public List<string> PlayerAbilities = new List<string>();
    public int EnemyWorshipperCount = 0;
    public float EnemyMorale = 0;
    public List<string> EnemyAbilites = new List<string>();
    public bool FinishedBattle = false;

    // Combat mode variables
    public BATTLESTATUS LastBattleStatus = BATTLESTATUS.Victory;

    // Management mode variables for loading scene

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
}

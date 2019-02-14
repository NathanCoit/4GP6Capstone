using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour {

    /*
     * This class is associated with the God-like ability that does AoE (area of effect) damage.
     * Basically used to get all targets within the AoE.
     * Attached to the AOE objects
     */

    //Use two lists because c# doesn't like it when you change what you're iterating
    private List<GameObject> targets;
    private List<GameObject> marrAllCollisions;

    private BoardManager BoardMan;

	// Use this for initialization
	void Start ()
    {
        targets = new List<GameObject>();
        marrAllCollisions = new List<GameObject>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    public List<GameObject> getTargets(bool isPlayer)
    {
        if (marrAllCollisions != null)
        {
            foreach (GameObject g in marrAllCollisions)
            {
                if (BoardMan.enemyUnits.Contains(g) && isPlayer)
                    targets.Add(g);
                else if (BoardMan.playerUnits.Contains(g) && !isPlayer)
                    targets.Add(g);
            }
        }
        marrAllCollisions = new List<GameObject>();
        return targets;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision);
        marrAllCollisions.Add(collision.gameObject);
    }


}


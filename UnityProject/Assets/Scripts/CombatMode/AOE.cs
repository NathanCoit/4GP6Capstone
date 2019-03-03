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
    private List<Unit> targets;
    private List<GameObject> marrAllCollisions;

    private BoardManager BoardMan;

	// Use this for initialization
	void Start ()
    {
        targets = new List<Unit>();
        marrAllCollisions = new List<GameObject>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    public List<Unit> getTargets(bool isPlayer)
    {
        if (marrAllCollisions != null)
        {
            foreach (GameObject g in marrAllCollisions)
            {
                if (isPlayer)
                {
                    foreach (Unit u in BoardMan.enemyUnits)
                    {
                        if (g == u.unitGameObject())
                        {
                            targets.Add(u);
                        }
                    }
                }
                else
                {
                    foreach (Unit u in BoardMan.playerUnits)
                    {
                        if (g == u.unitGameObject())
                        {
                            targets.Add(u);
                        }
                    }
                }
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


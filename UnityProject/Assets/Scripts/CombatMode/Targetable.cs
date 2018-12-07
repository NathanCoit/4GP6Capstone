using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour {

    public Vector2 pos;
    private bool autoClick;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private SetupManager SetupMan;

    public Ability a;
    public Material mousedOverValid;
    public Material mousedOverInvalid;
    public Material air;
    public GameObject CubeAOEPrefab;
    public GameObject SphereAOEPrefab;
    private GameObject AOEShape;

    // Use this for initialization
    void Start ()
    {
        autoClick = false;

        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    public void OnMouseOver()
    {
        List<GameObject> targets = new List<GameObject>();
        bool valid = false;

        if (a.Type == Ability.AbilityType.SingleTarget)
        {
            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                targets = BoardMan.enemyUnits;
            }
            else
            {
                targets = BoardMan.playerUnits;
            }

            foreach (GameObject g in targets)
            {
                if (pos.x == g.GetComponent<Units>().getPos().x && pos.y == g.GetComponent<Units>().getPos().y)
                    valid = true;
            }

            if (valid)
            {
                GetComponent<MeshRenderer>().material = mousedOverValid;
            }
            else
            {
                GetComponent<MeshRenderer>().material = mousedOverInvalid;
            }
        }
        else if(a.Type == Ability.AbilityType.MultiTarget)
        {
            MultiTargetAbility aMi = (MultiTargetAbility)MultiTargetAbility.LoadAbilityFromName(a.AbilityName);
            if (AOEShape == null)
            {
                

                if (aMi.AbilityShape == Ability.MultiTargetShape.Cone)
                {
                    AOEShape = Instantiate(SphereAOEPrefab);
                    AOEShape.transform.localScale *= aMi.Length;
                    AOEShape.gameObject.layer = 2;
                    AOEShape.transform.position = transform.position;
                }
                else if (aMi.AbilityShape == Ability.MultiTargetShape.Line)
                {
                    AOEShape = Instantiate(CubeAOEPrefab);
                    AOEShape.gameObject.layer = 2;
                    AOEShape.transform.localScale = new Vector3(AOEShape.transform.localScale.x * aMi.Length, AOEShape.transform.localScale.y, AOEShape.transform.localScale.z);
                    AOEShape.transform.position = new Vector3(transform.position.x + aMi.Length / 2, transform.position.y, transform.position.z);
                }
                else if (aMi.AbilityShape == Ability.MultiTargetShape.Square)
                {
                    AOEShape = Instantiate(CubeAOEPrefab);
                    AOEShape.transform.localScale *= aMi.Length;
                    AOEShape.gameObject.layer = 2;
                    AOEShape.transform.position = transform.position;
                }

            }

            if(aMi.AbilityShape == Ability.MultiTargetShape.Line)
            {
                if (MapMan.Selected.GetComponent<Gods>().direction == 0)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 0, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x + aMi.Length / 2, transform.position.y, transform.position.z);

                }
                else if (MapMan.Selected.GetComponent<Gods>().direction == 1)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 270, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + aMi.Length / 2);

                }
                else if (MapMan.Selected.GetComponent<Gods>().direction == 2)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 180, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x - aMi.Length / 2, transform.position.y, transform.position.z);

                }
                else if (MapMan.Selected.GetComponent<Gods>().direction == 3)
                {
                    AOEShape.transform.eulerAngles = new Vector3(0, 90, 0);
                    AOEShape.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - aMi.Length / 2);

                }
            }

            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                targets = AOEShape.GetComponent<AOE>().getTargets(true);
            }
            else
            {
                targets = AOEShape.GetComponent<AOE>().getTargets(false);
            }
            

            if (targets != new List<GameObject>())
                valid = true;
            else
                valid = false;

            if (valid)
            {
                GetComponent<MeshRenderer>().material = mousedOverValid;
                AOEShape.GetComponent<MeshRenderer>().material = mousedOverValid;
            }
            else
            {
                GetComponent<MeshRenderer>().material = mousedOverInvalid;
                AOEShape.GetComponent<MeshRenderer>().material = mousedOverInvalid;
            }
            
            
        }


        //Actually clicking
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            if (a.Type == Ability.AbilityType.SingleTarget)
            {
                SingleTargetAbility aSi = (SingleTargetAbility)SingleTargetAbility.LoadAbilityFromName(a.AbilityName);
                GameObject target = new GameObject();
                foreach (GameObject g in targets)
                {
                    if (pos.x == g.GetComponent<Units>().getPos().x && pos.y == g.GetComponent<Units>().getPos().y)
                        target = g;
                }

                target.GetComponent<Units>().setWorshiperCount(target.GetComponent<Units>().getWorshiperCount() - aSi.AbilityDamage);

                if(target.GetComponent<Units>().WorshiperCount >= 0)
                {
                    if (BoardMan.enemyUnits.Contains(target))
                        BoardMan.enemyUnits.Remove(target);
                    else if (BoardMan.playerUnits.Contains(target))
                        BoardMan.playerUnits.Remove(target);
                    Destroy(target);
                    checkEnd();
                }
                Debug.Log("Using single target ability " + a.AbilityName + " !!!");
            }
            else if (a.Type == Ability.AbilityType.MultiTarget)
            {
                MultiTargetAbility aMi = (MultiTargetAbility)MultiTargetAbility.LoadAbilityFromName(a.AbilityName);

                foreach (GameObject g in targets)
                {
                    g.GetComponent<Units>().setWorshiperCount(g.GetComponent<Units>().getWorshiperCount() - aMi.AbilityDamage);
                    if (g.GetComponent<Units>().WorshiperCount <= 0)
                    {
                        if (BoardMan.enemyUnits.Contains(g))
                            BoardMan.enemyUnits.Remove(g);
                        else if (BoardMan.playerUnits.Contains(g))
                            BoardMan.playerUnits.Remove(g);
                        Destroy(g);
                        checkEnd();
                    }
                }

                GameObject[] AOEShapes = GameObject.FindGameObjectsWithTag("AOEShapes");
                foreach(GameObject g in AOEShapes)
                {
                    Destroy(g);
                }

                Debug.Log("Using multi target ability " + a.AbilityName + " !!!");
            }
            else if (a.Type == Ability.AbilityType.Buff)
            {
                Debug.Log("Using buff ability " + a.AbilityName + " !!!");
            }
            else if (a.Type == Ability.AbilityType.Debuff)
            {
                Debug.Log("Using debuff ability " + a.AbilityName + " !!!");
            }

            //Hide Menu
            MapMan.Selected.GetComponent<Units>().EndTurnButton();

            //Unslecting
            MapMan.Selected = null;

            //Clean up Tiles
            MapMan.ClearSelection();

        }

        if (Input.GetMouseButtonDown(1))
            if (MapMan.Selected.GetComponent<Gods>().direction < 3)
                MapMan.Selected.GetComponent<Gods>().direction++;
            else
                MapMan.Selected.GetComponent<Gods>().direction = 0;

    }

    private void checkEnd()
    {
        //check if someone won the game (note we're checking if its 1 since we dont have killing gods in yet)
        if (BoardMan.playerUnits.Count == 1)
        {
            BoardMan.Defeat();
        }
        else if (BoardMan.enemyUnits.Count == 1)
        {
            BoardMan.Victory();
        }
    }

    public void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material = air;
        Destroy(AOEShape);
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }
}

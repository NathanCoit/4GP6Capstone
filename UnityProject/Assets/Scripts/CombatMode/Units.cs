using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Concerned with all of the stats involved in the making of a worshipper in combat mode.
 *      - their morale
 *      - their attack strength
 *      - whether they can act during this turn
 *      - also involves all the attack/move/end turn functions associated with each worshipper
 *      Is attached to all units
 */
public class Units : MonoBehaviour {

    public bool canAct;

    private bool isGod;

    private MapManager MapMan;
    private Vector2 pos;
    private BoardManager BoardMan;
    private List<int> depths = new List<int>();
    private HashSet<Tile> visited = new HashSet<Tile>();


    public GameObject MovableTile;
    public GameObject AttackableTile;
    public int Movement = 2;
    public int MaxMovement;
    public float morale;
    public bool isPlayer;
    public int MovePriority;

    //For use without models, can be removed later
    public Material playerAvailable;
    public Material playerNotAvailable;
    public Material enemyAvailable;
    public Material enemyNotAvailable;
    public int WorshiperCount;
    public float AttackStrength;


    private bool autoClick = false;

    // Use this for initialization
    void Start()
    {
        //You know who to call, ITS MAP MAN!
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        //Boardman is here also
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        
        if(isPlayer)
            GetComponent<MeshRenderer>().material = playerNotAvailable;
        else
            GetComponent<MeshRenderer>().material = enemyNotAvailable;

        AttackStrength = WorshiperCount * 0.25f * morale;

        //For AI
        MovePriority = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Updating morale every frame becuase it's very broken on occasion.
        if (isPlayer)
            morale = BoardMan.PlayerMorale;
        else
            morale = BoardMan.enemyMorale;

        updateAttackStrength();
    }

    public void Draw(Tile[,] tiles)
    {
        //Centers Unit on tile (I know it looks ugly but it SHOULD work for any model)
        transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
    }

    //I'm honestly not sure if we use this anymore
    public void Move(Vector2 amount, Tile[,] tiles)
    {
        //Check array bounds on tiles
        if (pos.x + amount.x < tiles.GetLength(0) && pos.y + amount.y < tiles.GetLength(1))
            if (pos.x + amount.x >= 0 && pos.y + amount.y >= 0)
                this.pos += amount;

        Draw(tiles);
    }

    //Resets units on new turn
    public void AllowAct() //this Unit has not yet acted in this round
    {
        canAct = true;
        Movement = MaxMovement;

        //Render stuff for use without proper models, can be removed later
        if (isPlayer)
            GetComponent<MeshRenderer>().material = playerAvailable;
        else
            GetComponent<MeshRenderer>().material = enemyAvailable;
    }

    //When a units end their turn
    public void EndAct() //this Unit has completed their allotted actions in this round
    {
        canAct = false;

        //Render stuff for use without proper models, can be removed later
        if (isPlayer)
            GetComponent<MeshRenderer>().material = playerNotAvailable;
        else
            GetComponent<MeshRenderer>().material = enemyNotAvailable;

        if(!isGod)
            transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);
        else if(GetComponent<Gods>().isInBattle())
            transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);
        else if(!GetComponent<Gods>().isInBattle())
            transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(false);

    }

    //Button used by the end turn button (and many other things, like AI and gods)
    public void EndTurnButton()
    {
        //check if somebody won
        if (BoardMan.GetComponent<BoardManager>().playerUnits.Count == 0)
        {
            BoardMan.GetComponent<BoardManager>().Defeat();
        }
        else if (BoardMan.GetComponent<BoardManager>().enemyUnits.Count == 0)
        {
            BoardMan.GetComponent<BoardManager>().Victory();
        }

        MapMan.ClearSelection();
        EndAct();
        BoardMan.GetComponent<BoardManager>().DecreaseNumActions();
    }

    //Sets a clicked unit to be selected
    public void OnMouseOver()
    {
        if ((Input.GetMouseButtonDown(0) || autoClick) && canAct && BoardMan.playerUnits.Contains(gameObject))
        {
            MapMan.Selected = this.gameObject;
            MapMan.newSelected = true;
            autoClick = false;
        }
            
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }

    //Get all the tiles a unit can move to, based on their remaining movement
    public void showMovable()
    {
        Tile[,] tiles = getTiles();

        HashSet<Tile> MovableTiles = new HashSet<Tile>();

        //Setup Invalid Tiles (the one with units on)
        List<GameObject> invalidTiles = new List<GameObject>();
        invalidTiles.AddRange(BoardMan.GetComponent<BoardManager>().playerUnits);
        invalidTiles.Remove(this.gameObject);

        //Calculate Movable Tiles
        MovableTiles = tiles[(int)getPos().x, (int)getPos().y].findAtDistance(tiles[(int)getPos().x, (int)getPos().y], Movement, invalidTiles, BoardMan.enemyUnits, tiles);

        //We need to do this because the above function breaks some connection (like the one that can't be moved through)
        MapMan.DefineConnections();

        //Clean up all the other tiles
        MapMan.ClearSelection();

        //Draw movable tiles
        foreach (Tile t in MovableTiles)
        {
            GameObject temp = Instantiate(MovableTile);
            temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }
    }

    //Shows attackable tiles (for attacking)
    public void showAttackable()
    {
        Tile[,] tiles = getTiles();
        HashSet<Tile> AttackableTiles = new HashSet<Tile>();
        List<Tile> ConnectedTiles = tiles[(int)getPos().x, (int)getPos().y].getConnected();
        List<GameObject> targets = new List<GameObject>();

        if (BoardMan.GetComponent<BoardManager>().playerUnits.Contains(MapMan.Selected))
        {
            targets = BoardMan.GetComponent<BoardManager>().enemyUnits;
        }
        else
        {
            targets = BoardMan.GetComponent<BoardManager>().playerUnits;
        }

        //Take the tiles connect to this unit's tile and see if theres an enemy unit on it
        foreach (Tile t in ConnectedTiles)
            foreach (GameObject g in targets)
                if (new Vector2(t.getX(), t.getZ()) == g.GetComponent<Units>().getPos())
                    AttackableTiles.Add(t);


        //Clean up all the other tiles
        MapMan.ClearSelection();

        //Draw movable tiles
        foreach (Tile t in AttackableTiles)
        {
            GameObject temp = Instantiate(AttackableTile);
            temp.GetComponent<Attackable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }

    }

    //This is what we actually use to move (who knows what the above is for)
    public void MoveTo(Vector2 pos, Tile[,] tiles)
    {
        this.pos = pos;

        Draw(tiles);
    }

    public Vector2 getPos()
    {
        return pos;
    }

    public bool HasAct()
    {
        return canAct;
    }

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    public bool CheckIfGod()
    {
        return isGod;
    }

    public int getWorshiperCount()
    {
        return WorshiperCount;
    }

    public void setWorshiperCount(int count)
    {
        WorshiperCount = count;
    }

    public float getMorale()
    {
        return morale;
    }
    public void setMorale(float mor)
    {
        morale = mor;
    }

    public float getAttackStrength()
    {
        return AttackStrength;
    }

    //For using skill, will be done later (if units ever have skills)
    public void useSkill(int number)
    {

    }

    public void setGod()
    {
        isGod = true;
    }

    //Attack strength = 1/4 of worshipper count * morale
    public void updateAttackStrength()
    {
        AttackStrength = WorshiperCount * 0.25f * morale;
    }

}

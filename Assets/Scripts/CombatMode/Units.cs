using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour {

    public bool canAct;

    private GameObject MapMan;
    private Vector2 pos;

    //For use without models, can be removed later
    public Material Available;
    public Material NotAvailable;
    private int WorshiperCount;

 
	// Use this for initialization
	void Start ()
    {
        //You know who to call, ITS MAP MAN!
        MapMan = GameObject.FindGameObjectWithTag("MapManager");

        //AllowAct();
        GetComponent<MeshRenderer>().material = NotAvailable;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Draw(Tile[,] tiles)
    {
        //Centers Unit on tile (I know it looks ugly but it SHOULD work for any model)
        transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
    }

    public void Move(Vector2 amount, Tile[,] tiles)
    {
        //Check array bounds on tiles
        if (pos.x + amount.x < tiles.GetLength(0) && pos.y + amount.y < tiles.GetLength(1))
            if (pos.x + amount.x >= 0 && pos.y + amount.y >= 0)
                this.pos += amount;

        this.Draw(tiles);
    }

    public void MoveTo(Vector2 pos, Tile[,] tiles)
    {
        this.pos = pos;

        this.Draw(tiles);
    }

    public Vector2 getPos()
    {
        return pos;
    }

    public bool HasAct()
    {
        return canAct;
    }

    public int getWorhsiperCount()
    {
        return WorshiperCount;
    }

    public void setWorshiperCount(int count)
    {
        WorshiperCount = count;
    }

    public void AllowAct() //this Unit has not yet acted in this round
    {
        canAct = true;

        //Render stuff for use without proper models, can be removed later
        GetComponent<MeshRenderer>().material = Available;
    }

    public void EndAct() //this Unit has completed their allotted actions in this round
    {
        canAct = false;

        //Render stuff for use without proper models, can be removed later
        GetComponent<MeshRenderer>().material = NotAvailable;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && canAct)
        {
            MapMan.GetComponent<MapManager>().Selected = this.gameObject;
            MapMan.GetComponent<MapManager>().newSelected = true;
        }
            
    }
}

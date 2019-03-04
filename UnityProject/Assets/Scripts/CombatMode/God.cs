using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : Unit
{
    private string godName;
    public bool isInBattle;
    private string[] Abilites;

    public God(string name) : base()
    {
        godName = name;
        isInBattle = false;
    }

    public void setAbilities(string[] Abilites)
    {
        this.Abilites = Abilites;
    }

    public string[] getAbilites()
    {
        return Abilites;
    }

    public string getName()
    {
        return godName;
    }

    public void enterBattle()
    {
        isInBattle = true;
        unitGameObject().GetComponent<UnitObjectScript>().drawEnterBattleTiles();
        UIMan.removeMenu();
        UIMan.godEnteringBattle = true;
    }

    public void forcedEnterBattle()
    {
        isInBattle = true;
        

        List<Tile> travesableTiles = new List<Tile>();

        foreach (Tile t in MapMan.tiles)
        {
            if (t.isTraversable())
            {
                travesableTiles.Add(t);
            }
        }

        System.Random r = new System.Random();
        Tile randomTile = travesableTiles[r.Next(0, travesableTiles.Count - 1)];

        MoveTo(new Vector2(randomTile.getX(), randomTile.getZ()), MapMan.tiles);
    }

    public override void Draw(Tile[,] tiles)
    {
        if(isInBattle)
            parentObject.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - parentObject.transform.lossyScale.x) / 2) + parentObject.transform.lossyScale.x / 2,
                tiles[(int)pos.x, (int)pos.y].getY() + parentObject.transform.lossyScale.y - 1.5f, 
                tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - parentObject.transform.lossyScale.z) / 2) + parentObject.transform.lossyScale.x / 2);
    }

    public override void updateAttackStrength()
    {
        //There a god, you can't make them hit less hard
    }


}

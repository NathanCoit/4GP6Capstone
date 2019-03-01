using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : Unit
{
    private string godName;
    private bool onGrid;
    private string[] Abilites;

    public God(string name) : base()
    {
        godName = name;
        onGrid = false;
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

    public override void Draw(Tile[,] tiles)
    {
        if(onGrid)
            parentObject.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - parentObject.transform.lossyScale.x) / 2) + parentObject.transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + parentObject.transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - parentObject.transform.lossyScale.z) / 2) + parentObject.transform.lossyScale.x / 2);
    }


}

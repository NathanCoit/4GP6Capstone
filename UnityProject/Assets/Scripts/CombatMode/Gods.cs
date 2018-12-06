using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gods : MonoBehaviour {

    private string Name;
    private bool inBattle;
    private string[] Abilities;

    // Use this for initialization
    void Start()
    {
        inBattle = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showAbilities()
    {
        foreach(string s in Abilities)
            Debug.Log(s);
    }

    public string getName()
    {
        return Name;
    }
    public bool isInBattle()
    {
        return inBattle;
    }



    public void setName(string name)
    {
        Name = name;
    }

    public void getInBattle()
    {
        inBattle = true;
    }

    public void setAbilites(string[] abilities)
    {
        Abilities = abilities;
    }
}

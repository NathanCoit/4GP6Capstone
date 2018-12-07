using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gods : MonoBehaviour {

    private string Name;
    private bool inBattle;
    private string[] Abilities;
    private MapManager MapMan;
    public int direction;

    public GameObject emptyButton;
    public GameObject TargetableTile;

    // Use this for initialization
    void Start()
    {
        inBattle = false;
        direction = 0;
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showAbilities()
    {
        //Disable first canvas and show abilites canvas
        transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(false);
        transform.GetChild(2).GetComponent<Canvas>().gameObject.SetActive(true);
        for(int i = 0; i < Abilities.Length; i++)
        {
            GameObject temp = Instantiate(emptyButton);
            temp.transform.parent = transform.GetChild(2);

            //Unity stupidity to postion the buttons
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1.05f, (Abilities.Length - i) * 0.5f + 1, 0);
            temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y, temp.transform.root.position.z);

            //Set appopritate text
            temp.GetComponentInChildren<Text>().text = Abilities[i];

            //Add listener to acutally use stuff
            temp.GetComponent<Button>().onClick.AddListener(delegate { useAbility(temp.GetComponentInChildren<Text>().text); });
        }

        //Setup return button
        GameObject temp2 = Instantiate(emptyButton);
        temp2.transform.parent = transform.GetChild(2);

        temp2.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1.05f, 1, 0);
        temp2.transform.position = new Vector3(temp2.transform.position.x, temp2.transform.position.y, temp2.transform.root.position.z);

        temp2.GetComponentInChildren<Text>().text = "Back to Actions";

        temp2.GetComponent<Button>().onClick.AddListener(delegate { returnToActions(); });
    }

    public void useAbility(string name)
    {
        transform.GetChild(2).GetComponent<Canvas>().gameObject.SetActive(false);

        Ability a = Ability.LoadAbilityFromName(name);

        foreach(Tile t in MapMan.tiles)
        {
            GameObject temp = Instantiate(TargetableTile);
            temp.GetComponent<Targetable>().a = a;
            temp.GetComponent<Targetable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }

        
    }

    private void returnToActions()
    {
        transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(true);
        transform.GetChild(2).GetComponent<Canvas>().gameObject.SetActive(false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour {

    bool canAct = true;

    public bool HasAct() {
        return canAct;
    }

    public void AllowAct() //this Unit has not yet acted in this round
    {
        canAct = true;
    }

    public void EndAct() //this Unit has completed their allotted actions in this round
    {
        canAct = false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

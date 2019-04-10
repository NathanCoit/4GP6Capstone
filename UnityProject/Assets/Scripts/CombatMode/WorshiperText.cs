using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Updates the health bar text

public class WorshiperText : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Text>().text = transform.root.GetComponent<UnitObjectScript>().getUnit().getWorshiperCount().ToString();
	}
}

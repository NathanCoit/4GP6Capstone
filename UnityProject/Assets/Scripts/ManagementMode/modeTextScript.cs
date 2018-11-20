using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class modeTextScript : MonoBehaviour {

	Text modeText;
	// Use this for initialization
	void Start () {
		modeText = gameObject.GetComponent<Text> ();
		modeText.text = "EXPLORE MODE";
	}

	// Update is called once per frame
	void Update () {
	}

	public void textChange(string text){
		switch (text) 
		{
		case "explore":
			modeText.text = "EXPLORE MODE";
			break;
		case "building":
			modeText.text = "BUILD MODE";
			break;
		case "upgrade":
			modeText.text = "UPGRADE/EDIT MODE";
			break;
		}
	}

    public string GetText()
    {
        return modeText.text;
    }
}

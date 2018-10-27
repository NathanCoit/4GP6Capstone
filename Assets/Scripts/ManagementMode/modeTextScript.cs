using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class modeTextScript : MonoBehaviour {

	Text modeText;
	// Use this for initialization
	void Start () {
		modeText = gameObject.GetComponent<Text> ();
		modeText.text = "Explore Mode";
	}

	// Update is called once per frame
	void Update () {
	}

	public void textChange(string text){
		switch (text) 
		{
		case "explore":
			modeText.text = "Explore Mode";
			break;
		case "building":
			modeText.text = "Building Mode";
			break;
		}
	}

    public string GetText()
    {
        return modeText.text;
    }
}

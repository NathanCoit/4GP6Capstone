using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class resourceScript : MonoBehaviour {

	Text resourceText;
	// Use this for initialization
	void Start () {
		resourceText = gameObject.GetComponent<Text> ();
		resourceText.text = "Materials:" + 0.ToString() + "     Worshippers:" + 0.ToString() + "     Morale:" + 0.ToString();
	}

	// Update is called once per frame
	void Update () {
	}

	public void resourceUIUpdate(int mat, int wor, float mor){
		resourceText.text = "Materials:" + mat.ToString() + "     Worshippers:" + wor.ToString() + "     Morale:" + mor.ToString();
	}
}

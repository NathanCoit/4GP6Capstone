using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class resourceScript : MonoBehaviour {

	public Text resourceText;
    public GameObject MaterialDisplay = null;
    private TextMeshProUGUI materialText = null;
	// Use this for initialization
	void Start () {
		resourceText = gameObject.GetComponent<Text> ();
		resourceText.text = "Materials: " + 0.ToString() + " Worshippers: " + 0.ToString() + " Morale: " + 0.ToString();
        materialText = MaterialDisplay.GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update () {
	}

	public void resourceUIUpdate(int mat, int wor, float mor){
		resourceText.text = "Materials: " + mat.ToString() + " Worshippers: " + wor.ToString() + " Morale: " + mor.ToString();
        materialText.text = mat.ToString();
	}
}

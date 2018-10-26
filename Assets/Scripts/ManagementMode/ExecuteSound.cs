using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteSound : MonoBehaviour {

	public AudioClip NotMaterials;
	public AudioSource musicSource;

	// Use this for initialization
	void Start () {
//		musicSource.clip = Animosity;
//		musicSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			PlaySound ("PlaceBuilding");
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			PlaySound ("NotMaterials");
		}
	}


	public void PlaySound(string soundName){
		switch (soundName) 
		{
		case "NotMaterials":
			musicSource.clip = NotMaterials;
			musicSource.Play ();
			break;
		}
	}

		
}

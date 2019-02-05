using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteSound : MonoBehaviour
{

    public AudioClip NotMaterials;
    public AudioClip PlaceBuilding;
    public AudioClip StartMusic;

    public AudioSource musicSource;

    // Use this for initialization
    void Start()
    {
        musicSource.clip = StartMusic;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "NotMaterials":
                musicSource.clip = NotMaterials;
                musicSource.Play();
                break;
            case "PlaceBuilding":
                musicSource.clip = PlaceBuilding;
                musicSource.Play();
                break;

        }
    }


}

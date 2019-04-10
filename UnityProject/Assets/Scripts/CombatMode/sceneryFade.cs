using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cuts out a piece of scenery when the camera get close

public class sceneryFade : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(0, Camera.main.transform.position.y, Camera.main.transform.position.z), new Vector3(0, transform.position.y, transform.position.z)) < 6f)
        {
            float distance = Vector3.Distance(new Vector3(0, Camera.main.transform.position.y, Camera.main.transform.position.z), new Vector3(0, transform.position.y, transform.position.z));
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 
                0);
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 1);
        }
    }
}

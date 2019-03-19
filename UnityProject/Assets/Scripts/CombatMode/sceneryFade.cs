using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneryFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(new Vector3(0, Camera.main.transform.position.y, Camera.main.transform.position.z), new Vector3(0, transform.position.y, transform.position.z)) < 6f)
        {
            float distance = Vector3.Distance(new Vector3(0, Camera.main.transform.position.y, Camera.main.transform.position.z), new Vector3(0, transform.position.y, transform.position.z));
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 
                0);
            
            //transform.localScale = new Vector3(1 - 1 / distance, 1 - 1 / distance, 1 - 1 / distance);
            //Debug.Log("Boop");
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 1);
            //transform.localScale = new Vector3(1, 1, 1);
        }
    }
}

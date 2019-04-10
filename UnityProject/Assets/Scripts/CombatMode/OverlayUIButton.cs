using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Goes on the end all, select next button and cancel. Set them active on m=player turn and inactive on enemy turn

public class OverlayUIButton : MonoBehaviour
{
    private BoardManager BoardMan;

    // Start is called before the first frame update
    void Start()
    {
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(BoardMan.isPlayerTurn()))
            gameObject.GetComponent<Button>().interactable = false;
        else
            gameObject.GetComponent<Button>().interactable = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitObjectScript : MonoBehaviour
{
    private BoardManager BoardMan;
    private MapManager MapMan;
    private UIManager UIMan;

    private Unit relatedUnit;

    public GameObject infoPanel;

    public Material playerAvailable;
    public Material playerNotAvailable;
    public Material enemyAvailable;
    public Material enemyNotAvailable;

    private Canvas infoCanvas;

    private bool frameOne;

    // Start is called before the first frame update
    void Start()
    {
        //Mapman, here to save the day!
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        //Boardman is here also
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        //Here to give you the best ui, it's
        UIMan = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        //infoCanvas = GameObject.FindGameObjectWithTag("InfoCanvas").GetComponent<Canvas>();

        frameOne = true;

        //Assign Screenspace - Camera camera to the main camera (because you can't do it in the prefab)
        gameObject.transform.GetChild(0).GetComponent<Canvas>().worldCamera = UIMan.mainCamera;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Unity takes a few frame to find the editor reference for some reason
        if (infoPanel != null)
        {
            /*
            float offset = 0.01f;

            RectTransform infoPanelRect = infoPanel.GetComponent<RectTransform>();

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + offset, gameObject.transform.position.z - offset));

            //Same as worldspacetoscreenspace in UIManager, but we need to undo the scaling
            Vector2 WorldObject_ScreenPosition = new Vector2(
            (((ViewportPosition.x * infoPanelRect.sizeDelta.x) - (infoPanelRect.sizeDelta.x * 0.5f)) / (gameObject.transform.localScale.x)),
            (((ViewportPosition.y * infoPanelRect.sizeDelta.y) - (infoPanelRect.sizeDelta.y * 0.5f)) / (gameObject.transform.localScale.y)));

            infoPanelRect.anchoredPosition = WorldObject_ScreenPosition;

            //Update cancas plane distance based only on z (depth)
            gameObject.transform.GetChild(0).GetComponent<Canvas>().planeDistance = Vector3.Distance(new Vector3(0, gameObject.transform.position.y, gameObject.transform.position.z - offset), 
                new Vector3(0, UIMan.mainCamera.transform.position.y, UIMan.mainCamera.transform.position.z));

            //gameObject.transform.GetChild(0).GetComponent<Canvas>().planeDistance = Vector3.Distance(gameObject.transform.position, UIMan.mainCamera.transform.position) - 0.2f;
            */
            //Base scale on how far away the camera is
            //infoPanel.transform.localScale = new Vector3(0.75f,0.5f,0.75f) * 1/Vector3.Distance(gameObject.transform.position, UIMan.mainCamera.transform.position);
            infoPanel.transform.localEulerAngles = new Vector3(-Mathf.Atan2(gameObject.transform.position.y - Camera.main.transform.position.y, 
                gameObject.transform.position.z - Camera.main.transform.position.z) * 180 / Mathf.PI, 0, 0);
        }
        
    }

    public Unit getUnit()
    {
        return relatedUnit;
    }

    public void setUnit(Unit u)
    {
        relatedUnit = u;
    }

    public void drawEnterBattleTiles()
    {
        foreach (Tile t in MapMan.tiles)
        {
            if (t.isTraversable())
            {
                GameObject temp = Instantiate(BoardMan.MovableTile);
                temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
                temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                    t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            }
        }
    }

    //Sets a clicked unit to be selected
    public void OnMouseOver()
    {
        if (relatedUnit != null)
        {
            if ((Input.GetMouseButtonDown(0) || relatedUnit.autoClick) && relatedUnit.canAct && BoardMan.playerUnits.Contains(relatedUnit) && MapMan.Selected != relatedUnit.unitGameObject() && !EventSystem.current.IsPointerOverGameObject())
            {
                MapMan.Selected = relatedUnit.unitGameObject();
                MapMan.newSelected = true;
                relatedUnit.autoClick = false;
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The terrain map object holding methods for interacting with the map object
/// Controls calculations for object placement and faction division.
/// </summary>
public class TerrainMap
{
    private GameObject muniTerrainMap;
    private List<MapObject> marrObjectsOnMap;
    private List<GameObject> marrLineDrawers;
    private System.Random mSysRandomNumGenerator = new System.Random();

    /// <summary>
    /// Contructor to generate map object for collision detection
    /// when clicking and placing objects
    /// </summary>
    /// <param name="pfMapRadius"></param>
    /// <param name="puniMapTexture"></param>
    public TerrainMap(float pfMapRadius, Texture puniMapTexture)
    {
        muniTerrainMap = CreateTerrainObject(pfMapRadius, puniMapTexture);
        marrObjectsOnMap = new List<MapObject>();
        marrLineDrawers = new List<GameObject>();
    }

    /// <summary>
    /// Create the game object used as the base of the game map.
    /// Creates a very thin cylinder to emulate a circle
    /// </summary>
    /// <param name="pfMapRadius"></param>
    /// <param name="puniMapTexture"></param>
    /// <returns></returns>
    private GameObject CreateTerrainObject(float pfMapRadius, Texture puniMapTexture)
    {
        GameObject uniMapGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        uniMapGameObject.transform.localScale = new Vector3(pfMapRadius, 0.1f, pfMapRadius);
        GameObject.Destroy(uniMapGameObject.GetComponent<CapsuleCollider>());
        uniMapGameObject.AddComponent<MeshCollider>();
        uniMapGameObject.AddComponent<LineRenderer>().positionCount = 0;
        uniMapGameObject.GetComponent<Renderer>().material.mainTexture = puniMapTexture;
        uniMapGameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(40, 40));
        return uniMapGameObject;
    }
    
    /// <summary>
    /// Helper method for placing a list of buildings from a save state
    /// Saved buildings ignore many placement checks as they were checked when placed before
    /// </summary>
    /// <param name="parrBuildingsToPlace"></param>
    /// <param name="pmusOwningFaction"></param>
    public void PlaceSavedFactionBuildings(GameInfo.SavedBuilding[] parrBuildingsToPlace, Faction pmusOwningFaction)
    {
        Building musLoadedBuilding = null;
        foreach (GameInfo.SavedBuilding musSavedBuildingToPlace in parrBuildingsToPlace)
        {
            switch (musSavedBuildingToPlace.BuildingType)
            {
                case Building.BUILDING_TYPE.MATERIAL:
                    musLoadedBuilding = new MineBuilding(musSavedBuildingToPlace, pmusOwningFaction);
                    break;
                default:
                    musLoadedBuilding = new Building(musSavedBuildingToPlace, pmusOwningFaction);
                    break;
            }
            PlaceBuilding(musLoadedBuilding, new Vector3(musSavedBuildingToPlace.x, musSavedBuildingToPlace.y, musSavedBuildingToPlace.z), true);
        }
    }
    
    /// <summary>
    /// Method for attempting to place a building at a given coordinate
    /// Checks to ensure player is allowed to place building where they are attempting to
    /// for feedback purposes.
    /// </summary>
    /// <param name="pmusBuildingToPlace"></param>
    /// <param name="puniPointToPlaceVector3"></param>
    /// <param name="pblnIgnoreOtherBuildings">Specify as true to ignore building placement checks.</param>
    /// <returns></returns>
    public bool PlaceBuilding(Building pmusBuildingToPlace, Vector3 puniPointToPlaceVector3, bool pblnIgnoreOtherBuildings = false)
    {
        bool blnCanPlace = true;
        //Attempt to place building and return result
        // Check if trying to place too close to another building
        if (!pblnIgnoreOtherBuildings)
        {
            blnCanPlace = CheckForValidPlacementPointInFactionArea(puniPointToPlaceVector3, pmusBuildingToPlace.OwningFaction.FactionArea);
        }
        else
        {
            blnCanPlace = true;
        }

        if (blnCanPlace)
        {
            marrObjectsOnMap.Add(pmusBuildingToPlace);
            pmusBuildingToPlace.ObjectPosition = puniPointToPlaceVector3;
            pmusBuildingToPlace.OwningFaction.OwnedBuildings.Add(pmusBuildingToPlace);
        }
        return blnCanPlace;
    }

    /// <summary>
    /// Method for placing a treasure object on the map
    /// Follows the same principles as the building placement but does not have to worry about territory checks
    /// </summary>
    /// <param name="pmusTreasureToPlace"></param>
    /// <param name="pPointToPlaceVector3"></param>
    /// <param name="pblnIgnorePlacementCheck"></param>
    /// <returns></returns>
    public bool PlaceTreasure(Treasure pmusTreasureToPlace, Vector3 pPointToPlaceVector3, bool pblnIgnorePlacementCheck = false)
    {
        bool blnCanPlace = true;
        if (!pblnIgnorePlacementCheck)
        {
            blnCanPlace = CheckForValidTreasurePlacement(pPointToPlaceVector3);
        }
        if (blnCanPlace)
        {
            marrObjectsOnMap.Add(pmusTreasureToPlace);
            pmusTreasureToPlace.ObjectPosition = pPointToPlaceVector3;
        }
        return blnCanPlace;
    }

    /// <summary>
    /// Calculation for whether a given point is too close to a building
    /// or is not within the given territory 
    /// </summary>
    /// <param name="puniPointToPlaceVector3"></param>
    /// <param name="parrFactionAreas"></param>
    /// <returns></returns>
    public bool CheckForValidPlacementPointInFactionArea(Vector3 puniPointToPlaceVector3, List<float[]> parrFactionAreas)
    {
        bool blnCanPlace = true;
        bool blnInAnArea = false;
        float fDistanceBetweenBuildings = 0f;
        float fAngleOfPlacement = 0f;
        float fRadiusOfPlacement = 0f;

        foreach (MapObject musObjectOnMap in marrObjectsOnMap)
        {
            fDistanceBetweenBuildings = Vector3.Distance(puniPointToPlaceVector3, musObjectOnMap.ObjectPosition);
            if (fDistanceBetweenBuildings < MapObject.ObjectRadius * 2)
            {
                blnCanPlace = false;
            }
        }

        fRadiusOfPlacement = Vector3.Distance(new Vector3(0, 0.5f, 0), puniPointToPlaceVector3);

        fAngleOfPlacement = Vector3.Angle(new Vector3(100f, 0.5f, 0), puniPointToPlaceVector3) * Mathf.PI / 180;
        // In third or fourth quadrant, add Pi as .angle will always return smallest vector
        if (puniPointToPlaceVector3.z < 0)
        {
            fAngleOfPlacement = 2 * Mathf.PI - fAngleOfPlacement;
        }

        foreach (float[] arrplayerArea in parrFactionAreas)
        {
            // Check if you are placing in your own area.
            if ((fAngleOfPlacement > arrplayerArea[2] && fAngleOfPlacement < arrplayerArea[3])
                && fRadiusOfPlacement > arrplayerArea[0] && fRadiusOfPlacement < arrplayerArea[1])
            {
                blnInAnArea = true;
            }
        }

        return blnInAnArea && blnCanPlace;
    }

    /// <summary>
    /// Check to see if a point is within a game tier.
    /// Allows for random point generation and checking for placing treasures.
    /// </summary>
    /// <param name="puniPointToPlaceVector3"></param>
    /// <param name="pfStartingRadius"></param>
    /// <param name="pfEndingRadius"></param>
    /// <returns></returns>
    public bool CheckForValidTreasurePlacement(Vector3 puniPointToPlaceVector3)
    {
        bool blnValid = true;
        float fDistanceBetweenObjects = 0f;

        // check if point is too close to an object
        foreach (MapObject musMapObject in marrObjectsOnMap)
        {
            fDistanceBetweenObjects = Vector3.Distance(puniPointToPlaceVector3, musMapObject.ObjectPosition);
            if (fDistanceBetweenObjects < MapObject.ObjectRadius * 2) // radius of self + radius of other
            {
                blnValid = false;
            }
        }
        return blnValid;
    }

    /// <summary>
    /// Filter objects on map for only building objects
    /// </summary>
    /// <returns></returns>
    public List<Building> GetBuildings()
    {
        return marrObjectsOnMap.FindAll(musMapObject => musMapObject.ObjectType == MapObject.MapObjectType.Building).Cast<Building>().ToList();
    }

    /// <summary>
    /// Filter objects on map for only treasure objects
    /// </summary>
    /// <returns></returns>
    public List<Treasure> GetTreasures()
    {
        return marrObjectsOnMap.FindAll(musMapObject => musMapObject.ObjectType == MapObject.MapObjectType.Treasure).Cast<Treasure>().ToList();
    }

    /// <summary>
    /// Return the game object of the map for collision detection
    /// </summary>
    /// <returns></returns>
    public GameObject GetMapObject()
    {
        return muniTerrainMap;
    }

    /// <summary>
    /// Remove a building from the current list of map objects
    /// </summary>
    /// <param name="pmusBuildingToRemove"></param>
    public void RemoveBuilding(Building pmusBuildingToRemove)
    {
        if (pmusBuildingToRemove != null)
        {
            marrObjectsOnMap.Remove(pmusBuildingToRemove);
        }
    }

    /// <summary>
    /// Remove a treasure from the current list of map objects
    /// </summary>
    /// <param name="pmusTreasureToRemove"></param>
    public void RemoveTreasure(Treasure pmusTreasureToRemove)
    {
        if (pmusTreasureToRemove != null)
        {
            marrObjectsOnMap.Remove(pmusTreasureToRemove);
        }

    }

    /// <summary>
    /// Calculate the width of a territory for a tier based on the starting and ending radius
    /// Allows for variable number of gods per tier.
    /// Called only on new game creation for splitting up the map
    /// </summary>
    /// <param name="parrCurrentFactions"></param>
    /// <param name="pfStartingRad"></param>
    /// <param name="pfEndingRad"></param>
    public void DivideMap(List<Faction> parrCurrentFactions, float pfStartingRad, float pfEndingRad)
    {
        float fFullCircleRad = 2 * Mathf.PI;
        float fAreaAngle = fFullCircleRad / parrCurrentFactions.Count;
        float fAngle = 0;
        foreach (Faction FactionToPlace in parrCurrentFactions)
        {
            FactionToPlace.FactionArea = new List<float[]>
            {
                new float[] { pfStartingRad, pfEndingRad, fAngle, fAngle + fAreaAngle }
            };

            fAngle += fAreaAngle;
        }
    }

    /// <summary>
    /// Method for loading and placing the god landscape prefabs underneath each
    /// god's specific territories.
    /// Scope lowered to 3 gods per tier, object scaling and trig now assumes 3 gods per tier and 4 in the first tier
    /// </summary>
    /// <param name="parrCurrentFactions"></param>
    public void AddGodLandscapes(List<Faction> parrCurrentFactions)
    {
        GameObject uniLandObject;
        int intPosx = 1;
        int intPosy = 1;
        foreach (Faction musFactionToPlace in parrCurrentFactions)
        {
            foreach (float[] arrFactionArea in musFactionToPlace.FactionArea)
            {
                if (arrFactionArea[0] == 0)
                {
                    intPosx = (arrFactionArea[0] + arrFactionArea[1]) / 2 * Mathf.Cos((arrFactionArea[2] + arrFactionArea[3]) / 2) > 0 ? 1 : -1;
                    intPosy = (arrFactionArea[0] + arrFactionArea[1]) / 2 * Mathf.Sin((arrFactionArea[2] + arrFactionArea[3]) / 2) > 0 ? 1 : -1;
                    // pie shaped area, place pie area
                    uniLandObject = (GameObject)GameObject.Instantiate(Resources.Load("GodLands/" + musFactionToPlace.Type.ToString() + "Pie"));
                    uniLandObject.transform.position = new Vector3(0, 0.15f, 0);
                    uniLandObject.transform.Rotate(new Vector3(0, ((arrFactionArea[2] + arrFactionArea[3]) / 2 * -180 / Mathf.PI) - 45, 0));
                    uniLandObject.transform.localScale = new Vector3(82.5f, 82.5f, 82.5f);
                }
                else if (arrFactionArea[0] > 80 && arrFactionArea[0] < 85)
                {
                    // Rounded rectangle
                    uniLandObject = (GameObject)GameObject.Instantiate(Resources.Load("GodLands/" + musFactionToPlace.Type.ToString() + "RoundedRect"));
                    uniLandObject.transform.position = new Vector3(0, 0.15f, 0);
                    uniLandObject.transform.Rotate(new Vector3(0, -((arrFactionArea[2] + arrFactionArea[3]) / 2 * 180 / Mathf.PI) - 30, 0));
                    uniLandObject.transform.localScale = new Vector3(82.5f, 82.5f, 82.5f);
                }
                else
                {
                    // Rounded rect3
                    uniLandObject = (GameObject)GameObject.Instantiate(Resources.Load("GodLands/" + musFactionToPlace.Type.ToString() + "RoundedRect3"));
                    uniLandObject.transform.position = new Vector3(0, 0.15f, 0);
                    uniLandObject.transform.Rotate(new Vector3(0, -((arrFactionArea[2] + arrFactionArea[3]) / 2 * 180 / Mathf.PI) - 30, 0));
                    uniLandObject.transform.localScale = new Vector3(82.5f, 82.5f, 82.5f);
                }
            }
        }
    }

    /// <summary>
    /// Outline the border around a specific faction
    /// Legacy code to allow player to see their area.
    /// </summary>
    /// <param name="pmusFaction"></param>
    public void DrawFactionArea(Faction pmusFaction)
    {
        OutlineFaction(pmusFaction);
    }

    /// <summary>
    /// Disables rendering of the base cylinder map as it is covered by god territory prefabs
    /// </summary>
    public void HideMap()
    {
        Renderer uniMapRenderer = muniTerrainMap.GetComponent<Renderer>();
        uniMapRenderer.enabled = false;
    }

    /// <summary>
    /// Method to draw the areas around multiple factions 
    /// to allow the player to see all god territories.
    /// Deprecated
    /// </summary>
    /// <param name="parrFactions"></param>
    public void DrawMultipleFactionAreas(List<Faction> parrFactions)
    {
        foreach (Faction musFaction in parrFactions)
        {
            OutlineFaction(musFaction);
        }
    }

    /// <summary>
    /// Given a faction, calculates a point that is within one of that factions territories
    /// Ignores building collision checks.
    /// Used to generate random positions for enemies to build at.
    /// </summary>
    /// <param name="pobjFactionToPlace"></param>
    /// <returns></returns>
    public Vector3 CalculateRandomPositionForFaction(Faction pobjFactionToPlace)
    {
        Vector3 uniStartingPositionVector3 = new Vector3(0, 0, 0);
        // Pick a random territory to build on
        int intAreaIndex = mSysRandomNumGenerator.Next(pobjFactionToPlace.FactionArea.Count);
        float[] arrFactionArea = pobjFactionToPlace.FactionArea[intAreaIndex];

        // Shrink angle range slightly to try and avoid building directly on territory borders
        float fAngle = Random.Range(arrFactionArea[2] + (Building.ObjectRadius / 100f), arrFactionArea[3] - (Building.ObjectRadius / 100f));
        float fRad = Random.Range(arrFactionArea[0] + Building.ObjectRadius, arrFactionArea[1] - Building.ObjectRadius);

        uniStartingPositionVector3 = new Vector3(fRad * Mathf.Cos(fAngle), 1.5f, fRad * Mathf.Sin(fAngle));
        return uniStartingPositionVector3;
    }

    /// <summary>
    /// Given a map tier, generate a random point within that tier.
    /// Ignores building checks and assumes a map diameter of 500.
    /// Used to place treasures at the beginning of a new game.
    /// </summary>
    /// <param name="pintGameTier"></param>
    /// <returns></returns>
    public Vector3 CalculateRandomPositionInTier(int pintGameTier)
    {
        Vector3 uniVector3;
        float fAngle = Random.Range(0, 360);
        float fRad = Random.Range((pintGameTier) * (250 / 3), (pintGameTier + 1) * (250 / 3));

        uniVector3 = new Vector3(fRad * Mathf.Cos(fAngle), 1.5f, fRad * Mathf.Sin(fAngle));
        return uniVector3;
    }

    /// <summary>
    /// Math and calculations for drawing the outlines of the different shapes for a faction
    /// </summary>
    /// <param name="pmusFaction"></param>
    private void OutlineFaction(Faction pmusFaction)
    {
        LineRenderer uniPlayerBoundsLineRenderer;
        Vector3 uniLinePositionVector3;
        List<float[]> arrPlayerArea;
        marrLineDrawers = new List<GameObject>();
        GameObject uniLineDrawerObject;
        float fAngleOfPoint;
        float fDeltaTheta;

        arrPlayerArea = pmusFaction.FactionArea;
        // Each territory requires a new line drawer
        foreach (float[] arrTerritory in arrPlayerArea)
        {
            uniLineDrawerObject = new GameObject("LineDrawer");
            uniLineDrawerObject.transform.parent = muniTerrainMap.transform;
            marrLineDrawers.Add(uniLineDrawerObject);
            uniPlayerBoundsLineRenderer = uniLineDrawerObject.AddComponent<LineRenderer>();
            uniPlayerBoundsLineRenderer.useWorldSpace = true;
            uniPlayerBoundsLineRenderer.widthMultiplier = 0.2f;

            if (arrTerritory[0] == 0)
            {
                // In starting tier, pie shape
                uniPlayerBoundsLineRenderer.positionCount = 33;
                uniLinePositionVector3 = new Vector3(arrTerritory[1] * Mathf.Cos(arrTerritory[3]), 0.5f, arrTerritory[1] * Mathf.Sin(arrTerritory[3]));
                uniPlayerBoundsLineRenderer.SetPosition(0, uniLinePositionVector3);

                uniLinePositionVector3 = new Vector3(0, 0.5f, 0);
                uniPlayerBoundsLineRenderer.SetPosition(1, uniLinePositionVector3);

                uniLinePositionVector3 = new Vector3(arrTerritory[1] * Mathf.Cos(arrTerritory[2]), 0.5f, arrTerritory[1] * Mathf.Sin(arrTerritory[2]));
                uniPlayerBoundsLineRenderer.SetPosition(2, uniLinePositionVector3);

                fDeltaTheta = (arrTerritory[3] - arrTerritory[2]) / 30;
                fAngleOfPoint = arrTerritory[2] + fDeltaTheta;
                for (int i = 0; i < 30; i++)
                {
                    uniLinePositionVector3 = new Vector3(arrTerritory[1] * Mathf.Cos(fAngleOfPoint), 0.5f, arrTerritory[1] * Mathf.Sin(fAngleOfPoint));
                    uniPlayerBoundsLineRenderer.SetPosition(i + 3, uniLinePositionVector3);
                    fAngleOfPoint += fDeltaTheta;
                }
            }
            else
            {
                // An outer tier, rounded rectangle
                uniPlayerBoundsLineRenderer.positionCount = 64;

                // Left line
                uniLinePositionVector3 = new Vector3(arrTerritory[1] * Mathf.Cos(arrTerritory[3]), 0.5f, arrTerritory[1] * Mathf.Sin(arrTerritory[3]));
                uniPlayerBoundsLineRenderer.SetPosition(0, uniLinePositionVector3);

                uniLinePositionVector3 = new Vector3(arrTerritory[0] * Mathf.Cos(arrTerritory[3]), 0.5f, arrTerritory[0] * Mathf.Sin(arrTerritory[3]));
                uniPlayerBoundsLineRenderer.SetPosition(1, uniLinePositionVector3);

                // Inner ring
                fDeltaTheta = (arrTerritory[2] - arrTerritory[3]) / 30;
                fAngleOfPoint = arrTerritory[3] + fDeltaTheta;
                for (int i = 0; i < 30; i++)
                {
                    uniLinePositionVector3 = new Vector3(arrTerritory[0] * Mathf.Cos(fAngleOfPoint), 0.5f, arrTerritory[0] * Mathf.Sin(fAngleOfPoint));
                    uniPlayerBoundsLineRenderer.SetPosition(i + 2, uniLinePositionVector3);
                    fAngleOfPoint += fDeltaTheta;
                }

                // Right line
                uniLinePositionVector3 = new Vector3(arrTerritory[0] * Mathf.Cos(arrTerritory[2]), 0.5f, arrTerritory[0] * Mathf.Sin(arrTerritory[2]));
                uniPlayerBoundsLineRenderer.SetPosition(32, uniLinePositionVector3);

                uniLinePositionVector3 = new Vector3(arrTerritory[1] * Mathf.Cos(arrTerritory[2]), 0.5f, arrTerritory[1] * Mathf.Sin(arrTerritory[2]));
                uniPlayerBoundsLineRenderer.SetPosition(33, uniLinePositionVector3);

                // Outer ring
                fDeltaTheta = (arrTerritory[3] - arrTerritory[2]) / 30;
                fAngleOfPoint = arrTerritory[2] + fDeltaTheta;
                for (int i = 0; i < 30; i++)
                {
                    uniLinePositionVector3 = new Vector3(arrTerritory[1] * Mathf.Cos(fAngleOfPoint), 0.5f, arrTerritory[1] * Mathf.Sin(fAngleOfPoint));
                    uniPlayerBoundsLineRenderer.SetPosition(i + 34, uniLinePositionVector3);
                    fAngleOfPoint += fDeltaTheta;
                }
            }
        }
    }
}

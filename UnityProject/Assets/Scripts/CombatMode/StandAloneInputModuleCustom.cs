using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//************************LEGACY**********************
//I honestly dont even know

public class StandAloneInputModuleCustom : StandaloneInputModule
{
    public PointerEventData GetLastPointerEventDataPublic(int id)
    {
        return GetLastPointerEventData(id);
    }
}

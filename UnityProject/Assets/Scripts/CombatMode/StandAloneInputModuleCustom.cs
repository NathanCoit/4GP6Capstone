using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StandAloneInputModuleCustom : StandaloneInputModule
{
    public PointerEventData GetLastPointerEventDataPublic(int id)
    {
        return GetLastPointerEventData(id);
    }
}

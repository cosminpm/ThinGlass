using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Button for resetting the map, rsetting it from zero
public class BtnReset : MonoBehaviour
{
    public void ResetMap()
    {
        GameObject _map = GameObject.Find("Cube");
        ControllerPlayer _scriptMap = _map.GetComponent<ControllerPlayer>(); 
        _scriptMap.ResetMap();
    }
    
}

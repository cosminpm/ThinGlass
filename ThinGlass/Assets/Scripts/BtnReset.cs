using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Button for resetting the map, rsetting it from zero
public class BtnReset : MonoBehaviour
{
    public void ResetMap()
    {
        GameObject _controllerPlayer = GameObject.Find("Cube");
        ControllerPlayer _scriptPlayer = _controllerPlayer.GetComponent<ControllerPlayer>();

        GameObject _map = GameObject.Find("Map");
        MapGenerator _scriptMap = _map.GetComponent<MapGenerator>();
        
        foreach (Transform panel in _map.transform)
        {
            if (panel.name == "Plane")
                Destroy(panel.gameObject);
        }

        _scriptMap._scaler = Random.Range(0.01f, 0.99f);
        _scriptMap.GenerateMap();
        _scriptPlayer.ResetMap();
    }
    
}

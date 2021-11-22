using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Button for resetting the map, rsetting it from zero
public class BtnReset : MonoBehaviour
{
    public void ResetMap()
    {
        GameObject controllerPlayer = GameObject.Find("Cube");
        ControllerPlayer scriptPlayer = controllerPlayer.GetComponent<ControllerPlayer>();

        GameObject map = GameObject.Find("Map");
        MapGenerator scriptMap = map.GetComponent<MapGenerator>();
        
        foreach (Transform panel in map.transform)
        {
            if (panel.name == "Plane")
                Destroy(panel.gameObject);
        }

        scriptMap.scaler = Random.Range(0.01f, 0.99f);
        scriptMap.ResetHearthsToEmpty();
        scriptMap.GenerateMap();
        scriptPlayer.ResetMap();
        scriptMap._generateExit();
        
    }
    
}

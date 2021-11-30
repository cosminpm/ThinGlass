using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Button for resetting the map, rsetting it from zero
public class BtnReset : MonoBehaviour
{
    public void ResetMap()
    {
        LevelController controllerPlayer = GameObject.Find("LevelController").GetComponent<LevelController>();
        controllerPlayer.GenerateNextLevel();
    }
    
}

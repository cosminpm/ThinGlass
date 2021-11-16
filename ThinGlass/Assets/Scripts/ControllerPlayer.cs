using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _map;
    
    IEnumerator Start()
    {
        _map = GameObject.Find("Map");
        yield return new WaitUntil(() => _map.GetComponent<MapGenerator>().isInitialized);
        gameObject.transform.position = new Vector3(getCenter(_map).x, 10,getCenter(_map).z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private Vector3 getCenter(GameObject map)
    {
        MapGenerator scriptMap = map.GetComponent<MapGenerator>();
        int[] center = {scriptMap.height / 2, scriptMap.width / 2};
        int[] originalCenter = center;

        for (int i = 0; i < scriptMap.width/2; i++)
        {
            for (int j = 0; j < scriptMap.height/2; j++)
            {
                if (scriptMap.ArrOfPlanes[center[0], center[1]])
                {
                    return scriptMap.ArrOfPlanes[center[0], center[1]].transform.position;
                }
                center[0] = originalCenter[0] + j;
            }
            center[0] = originalCenter[0];
            center[1] = originalCenter[1] + i;
        }
        throw new Exception("No center found in the map created");
    }
}

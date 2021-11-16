using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _map;
    private MapGenerator _scriptMap;
    private int[] _actualPosition;
    IEnumerator Start()
    {
        _map = GameObject.Find("Map");
        yield return new WaitUntil(() => _map.GetComponent<MapGenerator>().isInitialized);
        _scriptMap = _map.GetComponent<MapGenerator>();
        
        int[] center = getCenter(_map);
        gameObject.transform.position = 
            new Vector3(_scriptMap.ArrOfPlanes[center[0],center[1]].transform.position.x, 10,_scriptMap.ArrOfPlanes[center[0],center[1]].transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        ControllerPLayer();
    }

    void ControllerPLayer()
    {
        if (Input.GetKeyDown("up"))
        {
            _actualPosition = new []{_actualPosition[0] - 1, _actualPosition[1]};
            gameObject.transform.position =
                new Vector3(_scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.x, 5, _scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.z);
        }
        if (Input.GetKeyDown("down"))
        {
            _actualPosition = new []{_actualPosition[0] + 1, _actualPosition[1]};
            gameObject.transform.position =
                new Vector3(_scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.x, 5, _scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.z);
        }
        if (Input.GetKeyDown("left"))
        {
            _actualPosition = new []{_actualPosition[0], _actualPosition[1]-1};
            gameObject.transform.position =
                new Vector3(_scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.x, 5, _scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.z);
        }
        if (Input.GetKeyDown("right"))
        {
            _actualPosition = new []{_actualPosition[0], _actualPosition[1]+1};
            gameObject.transform.position =
                new Vector3(_scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.x, 5, _scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.z);
        }
    }

    private int[] getCenter(GameObject map)
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
                    _actualPosition = center;
                    return center;
                }
                center[0] = originalCenter[0] + j;
            }
            center[0] = originalCenter[0];
            center[1] = originalCenter[1] + i;
        }
        throw new Exception("No center found in the map created");
    }
}

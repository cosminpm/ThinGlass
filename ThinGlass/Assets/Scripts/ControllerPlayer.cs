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
        
        int[] center = GetCenter(_map);
        gameObject.transform.position = 
            new Vector3(_scriptMap.ArrOfPlanes[center[0],center[1]].transform.position.x, 10,_scriptMap.ArrOfPlanes[center[0],center[1]].transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        ControllerPLayer();
    }

    private void ControllerPLayer()
    {
        if (Input.GetKeyDown("up"))
        {
            BreakPanel(_actualPosition[0], _actualPosition[1]);
            MoveCube(-1, 0);
            CheckMovement(_actualPosition[0], _actualPosition[1]);
        }
        if (Input.GetKeyDown("down"))
        {
            BreakPanel(_actualPosition[0], _actualPosition[1]);
            MoveCube(1, 0);
            CheckMovement(_actualPosition[0], _actualPosition[1]);
        }
        if (Input.GetKeyDown("left"))
        {
            BreakPanel(_actualPosition[0], _actualPosition[1]);
            MoveCube(0, -1);
            CheckMovement(_actualPosition[0], _actualPosition[1]);
        }
        if (Input.GetKeyDown("right"))
        {
            
            BreakPanel(_actualPosition[0], _actualPosition[1]);
            MoveCube(0, 1);
            CheckMovement(_actualPosition[0], _actualPosition[1]);
        }
    }

    private int[] GetCenter(GameObject map)
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

    private void BreakPanel(int i, int j)
    {
        _scriptMap.ArrOfPlanes[i,j].GetComponent<Renderer>().material.color = new Color(255, 0, 0); 
    }

    private void MoveCube(int x, int z)
    {
        _actualPosition = new []{_actualPosition[0] + x, _actualPosition[1] + z};
        gameObject.transform.position =
            new Vector3(_scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.x, 
                5, _scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.position.z);
    }

    private void CheckMovement(int x, int z)
    {
        if (_scriptMap.ArrOfPlanes[x, z].GetComponent<Renderer>().material.color.Equals(new Color(255, 0, 0)))
        {
            Debug.Log("HAS PISADO MAL");
        }
    }
    
}

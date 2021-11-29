using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ControllerPlayer : MonoBehaviour
{
    private int[] _actualPosition;
    public List<int[]> glassStepped;

    void Awake()
    {
        // To initializate the blocks the player stepped on
        glassStepped = new List<int[]>();
        _actualPosition = new []{0,0};
    }

    private void Update()
    {
        //throw new NotImplementedException();
    }

    public int[] MovePlayer(GameObject[,] map)
    {
        int x = 0;
        int z = 0;
        if (Input.GetKeyDown("up"))
        {
            x = -1;
            z = 0;
        }

        if (Input.GetKeyDown("down"))
        {
            x = 1;
            z = 0;
        }

        if (Input.GetKeyDown("left"))
        {
            x = 0;
            z = -1;
        }

        if (Input.GetKeyDown("right"))
        {
            x = 0;
            z = 1;
        }

        MoveCube(x, z, map);
        _actualPosition[0] += x;
        _actualPosition[1] += z;

        return _actualPosition;
    }

    public int[] GetActualPosition()
    {
        return _actualPosition;
    }

    private void MoveCube(int x, int z, GameObject[,] map)
    {
        _actualPosition = new[] {_actualPosition[0] + x, _actualPosition[1] + z};
        gameObject.transform.localPosition =
            new Vector3(map[_actualPosition[0], _actualPosition[1]].transform.localPosition.x,
                5, map[_actualPosition[0], _actualPosition[1]].transform.localPosition.z);
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }
}
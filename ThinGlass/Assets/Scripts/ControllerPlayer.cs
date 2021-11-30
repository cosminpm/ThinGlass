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
    public bool isInitialized;

    void Start()
    {
        // To initializate the blocks the player stepped on
        glassStepped = new List<int[]>();
        _actualPosition = new[] {0, 0};
        isInitialized = true;
    }
    
    public int[] MovePlayer()
    {
        int x, z;
        if (Input.GetKeyDown("up"))
        {
            x = -1;
            z = 0;
            ActionsInput(x,z);
        }

        if (Input.GetKeyDown("down"))
        {
            x = 1;
            z = 0;
            ActionsInput(x,z);
        }

        if (Input.GetKeyDown("left"))
        {
            x = 0;
            z = -1;
            ActionsInput(x,z);
        }

        if (Input.GetKeyDown("right"))
        {
            x = 0;
            z = 1;
            ActionsInput(x,z);
        }

        
        return _actualPosition;
    }

    private void ActionsInput(int x, int z)
    {
        _actualPosition[0] += x;
        _actualPosition[1] += z;
        
        glassStepped.Add((int[])_actualPosition.Clone());
    }

    public int[] GetActualPosition()
    {
        return _actualPosition;
    }

    public void SetPosition(Vector3 position, int[] coor)
    {
        _actualPosition[0] = coor[0];
        _actualPosition[1] = coor[1];
        gameObject.transform.position = position;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public int width;
    public int height;

    public GameObject[,] arrOfPlanes;
    [HideInInspector] public float scaler = 0.15f;

    [HideInInspector] public float widthPlane, heightPlane;
    [HideInInspector] public int[] exitCoor;
    public int[] center;

    public void GenerateMap(Color badBoxesColor, Color goodBoxesColor)
    {
        arrOfPlanes = new GameObject[width, height];
        widthPlane = _getSizeOfPlane(1f, 1f)[0];
        heightPlane = _getSizeOfPlane(1f, 1f)[1];

        for (int i = 0; i < width; i++)
        for (int j = 0; j < height; j++)
        {
            // Get the position of where should be the object
            float positionX = i * widthPlane;
            float positionZ = j * heightPlane;

            float perlinNoiseValue = Mathf.PerlinNoise(positionX * scaler, positionZ * scaler);

            // The ones you can touch
            if (perlinNoiseValue > 0.3 + 0.1 * distance_squared(positionX, positionZ))
            {
                arrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                arrOfPlanes[i, j].gameObject.transform.SetParent(gameObject.transform);
                arrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                arrOfPlanes[i, j].gameObject.transform.localPosition = new Vector3(positionX, 0, positionZ);
                arrOfPlanes[i, j].GetComponent<Renderer>().material.color = goodBoxesColor;
            }
            // The ones you cant touch, if you touch them you die
            else
            {
                arrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                arrOfPlanes[i, j].gameObject.transform.SetParent(gameObject.transform);
                arrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                arrOfPlanes[i, j].GetComponent<Renderer>().material.color = badBoxesColor;
                arrOfPlanes[i, j].gameObject.transform.localPosition = new Vector3(positionX, 0, positionZ);
            }
        }

        center = GetCenter();
        ClearExitCells();
    }

    // Return the size of a plane given the scale of x and z
    private float[] _getSizeOfPlane(float x, float z)
    {
        var aux = GameObject.CreatePrimitive(PrimitiveType.Plane);
        aux.transform.localScale = new Vector3(1, 1, 1);
        float heightAux = aux.GetComponent<MeshFilter>().mesh.bounds.extents.z * 2;
        float widthAux = aux.GetComponent<MeshFilter>().mesh.bounds.extents.x * 2;
        Destroy(aux);
        return new[] {widthAux, heightAux};
    }

    // Get the distanced squared from the center, this function is used to make a map
    private float distance_squared(float x, float y)
    {
        float dx = 2 * x / (width * _getSizeOfPlane(1f, 1f)[0]) - 1;
        float dy = 2 * y / (height * _getSizeOfPlane(1f, 1f)[1]) - 1;
        //at this point 0 <= dx <= 1 and 0 <= dy <= 1
        return dx * dx + dy * dy;
    }

    // For not overcomplicate the project and not implementing a A* algorithm we just check that the exit has 4 adjacent 
    // white spaces, could be improved with a A* algorithm or even a solution that find the optimal path to go through all
    // white boxes.
    public void _generateExit()
    {
        int widthExit = Random.Range(2, width - 2);
        int heightExit = Random.Range(2, height - 2);

        while ((widthExit == center[0] && heightExit == center[1]) ||
               (widthExit == center[0] + 1 && heightExit == center[1]) ||
               (widthExit == center[0] - 1 && heightExit == center[1]) ||
               (widthExit == center[0] && heightExit == center[1] + 1) ||
               (widthExit == center[0] && heightExit == center[1] - 1))
        {
            widthExit = Random.Range(2, width - 2);
            heightExit = Random.Range(2, height - 2);
        }

        exitCoor = new[] {widthExit, heightExit};
        arrOfPlanes[widthExit, heightExit].GetComponent<Renderer>().material.color = new Color(0, 255, 0);
    }

    // Clears the four up, down, left, and right squares from the start position
    private void ClearExitCells()
    {
        arrOfPlanes[center[0] + 1, center[1]].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        arrOfPlanes[center[0] - 1, center[1]].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        arrOfPlanes[center[0], center[1] + 1].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        arrOfPlanes[center[0], center[1] - 1].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
    }

    public int[] GetCenter()
    {
        int[] auxCenter = {height / 2, width / 2};
        int[] originalCenter = auxCenter;

        for (int i = 0; i < width / 2; i++)
        {
            for (int j = 0; j < height / 2; j++)
            {
                if (arrOfPlanes[auxCenter[0], auxCenter[1]])
                {
                    return auxCenter;
                }

                auxCenter[0] = originalCenter[0] + j;
            }

            auxCenter[0] = originalCenter[0];
            auxCenter[1] = originalCenter[1] + i;
        }

        throw new Exception("No center found in the map created");
    }

    public Vector3 GetCenterVector3()
    {
        return arrOfPlanes[center[0], center[1]].transform.position;
    }

    public void SetPanelsToColor(List<int[]> panels, Color color)
    {
        // Reset all the red boxes the player stepped into white
        foreach (var pos in panels)
        {
            arrOfPlanes[pos[0], pos[1]].GetComponent<Renderer>().material.color = color;
        }
    }

    // Destroys the map and clears all the panels
    public void DestroyMap()
    {
        foreach (Transform panel in transform)
        {
            if (panel.name == "Plane")
                Destroy(panel.gameObject);
        }
    }

    public void IncreaseLevelOfDiff()
    {
        height += 1;
        width += 1;
        scaler = Random.Range(0.01f, 0.99f);
    }

    public Color GetColorPanel(int[] getColorOfPanel)
    {
        return arrOfPlanes[getColorOfPanel[0], getColorOfPanel[1]].GetComponent<Renderer>().material.color;
    }
    
    public void BreakPanel(int [] position)
    {
        arrOfPlanes[position[0], position[1]].GetComponent<Renderer>().material.color = new Color(255, 0, 0);
    }
    
}
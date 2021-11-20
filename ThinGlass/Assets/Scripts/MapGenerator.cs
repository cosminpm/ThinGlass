using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public int width;
    public int height;
    
    public GameObject[,] ArrOfPlanes;
    [HideInInspector]
    public float _scaler = 0.15f;
    
    [HideInInspector]
    public float widthPlane;
    [HideInInspector]
    public float heightPlane;
    [HideInInspector]
    public bool isInitialized;
    [HideInInspector]
    public int[] exitCoor;
    private ControllerPlayer _scriptPlayer;
    private Camera mainCamera;
    public int[] center;
    public int level;
    public Text levelText;
    public int pointsNeeded;
    public Text pointsNeededText;
    public GameObject hearthObject;
    
    private List<GameObject> _livesObjects;
    public List<int[]> PositionHearths;
    
    void Start()
    {
        _livesObjects = new List<GameObject>();
        
        GenerateMap();
        _generateExit();
        level = 0;
        isInitialized = true;
        _scriptPlayer = GameObject.Find("Cube").GetComponent<ControllerPlayer>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        levelText.text = level.ToString();
        pointsNeededText.text = pointsNeeded.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        _rotateHearth();
    }

    private void _rotateHearth()
    {
        foreach (var h in _livesObjects)
        {
            h.transform.Rotate(0, Time.deltaTime * 100f, 0, Space.Self);
        }
    }
    
    
    public void GenerateMap()
    {
        pointsNeeded = height * width / 3;
        ArrOfPlanes = new GameObject[width, height];
        widthPlane = _getSizeOfPlane(1f, 1f)[0];
        heightPlane = _getSizeOfPlane(1f, 1f)[1];

        for (int i = 0; i < width; i++)
        for (int j = 0; j < height; j++)
        {
            // Get the position of where should be the object
            float positionX = i * widthPlane;
            float positionZ = j * heightPlane;

            float perlinNoiseValue = Mathf.PerlinNoise(positionX * _scaler, positionZ * _scaler);

            // The ones you can touch
            if (perlinNoiseValue > 0.3 + 0.1 * distance_squared(positionX, positionZ))
            {
                ArrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ArrOfPlanes[i, j].gameObject.transform.SetParent(gameObject.transform);
                ArrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                ArrOfPlanes[i, j].gameObject.transform.localPosition = new Vector3(positionX, 0, positionZ);
                ArrOfPlanes[i, j].GetComponent<Renderer>().material.color = new Color(255, 255, 255); 
            }
            // The ones you cant touch, if you touch them you die
            else
            {
                ArrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ArrOfPlanes[i, j].gameObject.transform.SetParent(gameObject.transform);
                ArrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                ArrOfPlanes[i, j].GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                ArrOfPlanes[i, j].gameObject.transform.localPosition = new Vector3(positionX, 0, positionZ);
            }
        }

        center = GetCenter();
        ClearExitCells();
        GenerateExtraLife();
        

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
        float dx = 2 * x / (width*_getSizeOfPlane(1f, 1f)[0]) - 1;
        float dy = 2 * y / (height*_getSizeOfPlane(1f, 1f)[1]) - 1;
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
        ArrOfPlanes[widthExit, heightExit].GetComponent<Renderer>().material.color = new Color(0, 255, 0);
    }

    // Clears the four up, down, left, and right squares from the start position
    private void ClearExitCells()
    { ;
        ArrOfPlanes[center[0] + 1, center[1]].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        ArrOfPlanes[center[0] - 1,center[1]].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        ArrOfPlanes[center[0], center[1] + 1].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        ArrOfPlanes[center[0], center[1] - 1].GetComponent<Renderer>().material.color = new Color(255, 255, 255);
    }
    
    public int[] GetCenter()
    {
        int[] auxCenter = {height / 2, width / 2};
        int[] originalCenter = auxCenter;

        for (int i = 0; i < width/2; i++)
        {
            for (int j = 0; j < height/2; j++)
            {
                if (ArrOfPlanes[auxCenter[0], auxCenter[1]])
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
    
    
    public void GenerateNextLevel()
    {

        foreach (Transform panel in transform)
        {
            if (panel.name == "Plane")
                Destroy(panel.gameObject);
        }
        
        height += 1;
        width += 1;
        
        _scaler = Random.Range(0.01f, 0.99f);
        GenerateMap();
        pointsNeeded = (height * width / 3) + _scriptPlayer.totalScore - 1;
        pointsNeededText.text = pointsNeeded.ToString();
        var transform1 = mainCamera.transform;
        var position = transform1.position;
        position = new Vector3(position.x+5.5f, position.y + 100, position.z+6.5f);
        transform1.position = position;
        _scriptPlayer.ResetMap();
        level += 1;
        levelText.text = level.ToString();
        _generateExit();
    }



    private void GenerateExtraLife()
    {
        int probHearth = Random.Range(1, 100);
        int minimum = 0;

        if (probHearth >= minimum)
        {
            int widthValue = Random.Range(2, width - 2);
            int heightValue = Random.Range(2, height - 2);
        
            while ((widthValue == center[0] && heightValue == center[1]) || 
                   (widthValue == center[0] + 1 && heightValue == center[1]) ||
                   (widthValue == center[0] - 1 && heightValue == center[1]) || 
                   (widthValue == center[0] && heightValue == center[1] + 1) || 
                   (widthValue == center[0] && heightValue == center[1] - 1) &&
                    ArrOfPlanes[widthValue, heightValue].GetComponent<Renderer>().material.color.Equals(new Color(255,255,255)))
            {
                widthValue = Random.Range(2, width - 2);
                heightValue = Random.Range(2, height - 2);
            }
            
            GameObject hearth = Instantiate(hearthObject, new Vector3(ArrOfPlanes[widthValue, heightValue].transform.position.x,
                10,ArrOfPlanes[widthValue,heightValue].transform.position.z), hearthObject.transform.rotation);
            hearth.transform.localScale = new Vector3(175f, 175f, 175f);
            
            PositionHearths.Add(new []{widthValue, heightValue});
            _livesObjects.Add(hearth);
        }
    }
}

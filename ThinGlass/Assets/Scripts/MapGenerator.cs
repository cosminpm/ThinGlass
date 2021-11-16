using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public int width;
    public int height;
    
    public GameObject[,] ArrOfPlanes;
    private float _scaler = 0.15f;
    
    [HideInInspector]
    public float widthPlane;
    [HideInInspector]
    public float heightPlane;
    public bool isInitialized;
    void Start()
    {
        width = 39;
        height = 45;
        ArrOfPlanes = new GameObject[width, height];
        Debug.Log(width);
        widthPlane = _getSizeOfPlane(1f, 1f)[0];
        heightPlane = _getSizeOfPlane(1f, 1f)[1];

        for (int i = 0; i < width; i++)
        for (int j = 0; j < height; j++)
        {
            // Get the position of where should be the object
            float positionX = i * widthPlane;
            float positionZ = j * heightPlane;

            float perlinNoiseValue = Mathf.PerlinNoise(positionX * _scaler, positionZ * _scaler);

            if (perlinNoiseValue > 0.3 + 0.1 * distance_squared(positionX, positionZ))
            {
                ArrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ArrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                ArrOfPlanes[i, j].gameObject.transform.SetParent(gameObject.transform);
                ArrOfPlanes[i, j].gameObject.transform.position = new Vector3(positionX, 0, positionZ);
            }
        }

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Return the size of a plane given the scale of x and z
    private float[] _getSizeOfPlane(float x, float z)
    {
        var aux = GameObject.CreatePrimitive(PrimitiveType.Plane);
        aux.transform.localScale = new Vector3(1, 1, 1);
        float height = aux.GetComponent<MeshFilter>().mesh.bounds.extents.z * 2;
        float width = aux.GetComponent<MeshFilter>().mesh.bounds.extents.x * 2;
        Destroy(aux);
        return new[] {width, height};
    }
    
    // Get the distanced squared from the center, this function is used to make a map
    private float  distance_squared(float x, float y)
    {
        float dx = 2 * x / (width*_getSizeOfPlane(1f, 1f)[0]) - 1;
        float dy = 2 * y / (height*_getSizeOfPlane(1f, 1f)[1]) - 1;
        //at this point 0 <= dx <= 1 and 0 <= dy <= 1
        return dx * dx + dy * dy;
    }
    
}

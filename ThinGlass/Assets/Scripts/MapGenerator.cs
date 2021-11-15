using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    private int _width = 10;
    private int _height = 10;
    private GameObject[,] _arrOfPlanes;
    private float _scaler = 0.15f;
    private float _widthPlane;
    private float _heightPlane;
    
    void Start()
    {
        _arrOfPlanes = new GameObject[_width, _height];

        _widthPlane = _getSizeOfPlane(1f, 1f)[0];
        _heightPlane = _getSizeOfPlane(1f, 1f)[1];

        for (int i = 0; i < _width; i++)
        for (int j = 0; j < _height; j++)
        {
            // Get the position of where should be the object
            float positionX = i * _widthPlane;
            float positionZ = j * _heightPlane;

            float perlinNoiseValue = Mathf.PerlinNoise(positionX * _scaler, positionZ * _scaler);

            if (perlinNoiseValue > 0.3 + 0.1 * distance_squared(positionX, positionZ))
            {
                _arrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _arrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                _arrOfPlanes[i, j].gameObject.transform.SetParent(gameObject.transform);
                _arrOfPlanes[i, j].gameObject.transform.position = new Vector3(positionX, 0, positionZ);
            }
        }
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
        float heightPlane = aux.GetComponent<MeshFilter>().mesh.bounds.extents.z * 2;
        float widthPlane = aux.GetComponent<MeshFilter>().mesh.bounds.extents.x * 2;
        Destroy(aux);
        return new[] {widthPlane, heightPlane};
    }
    
    // Get the distanced squared from the center, this function is used to make a map
    private float  distance_squared(float x, float y)
    {
        float dx = 2 * x / (_width*_getSizeOfPlane(1f, 1f)[0]) - 1;
        float dy = 2 * y / (_height*_getSizeOfPlane(1f, 1f)[1]) - 1;
        //at this point 0 <= dx <= 1 and 0 <= dy <= 1
        return dx * dx + dy * dy;
    }
    
}

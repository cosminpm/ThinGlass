using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    
   
    private int _width = 5;
    private int _height = 3;
    private GameObject[,] _arrOfPlanes;
    void Start()
    {
        _arrOfPlanes = new GameObject[_width, _height];
            
        for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _arrOfPlanes[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    _arrOfPlanes[i, j].gameObject.transform.localScale = new Vector3(1, 1, 1);
                    
                    float heightPlane = _arrOfPlanes[i, j].gameObject.GetComponent<MeshFilter>().mesh.bounds.extents.z * 2;
                    float widthPlane = _arrOfPlanes[i, j].gameObject.GetComponent<MeshFilter>().mesh.bounds.extents.x * 2;
                    float positionX = i * widthPlane;
                    float positionZ = j * heightPlane;
                    
                    //_arrOfPlanes[i, j].gameObject.transform.position = new Vector3(positionX, 0, positionZ);
                    _arrOfPlanes[i ,j].gameObject.transform.SetParent(gameObject.transform);

                    if (Mathf.PerlinNoise(positionX, positionZ) > 0.3 + 0.4 * distance_squared(positionX, positionZ))
                    {
                        _arrOfPlanes[i, j].gameObject.transform.position = new Vector3(positionX, 0, positionZ);
                    }
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float  distance_squared(float x, float y)
    {
        float dx = 2 * x / _width - 1;
        float dy = 2 * y / _height - 1;
        //at this point 0 <= dx <= 1 and 0 <= dy <= 1
        return dx * dx + dy * dy;
    }
    
}

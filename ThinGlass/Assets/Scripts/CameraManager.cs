
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;
    public bool isInitialized;
    private void Start()
    {
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        isInitialized = true;
    }
    
    public void EditCamera(int width, int height, float widthPlane, float heightPlane, Vector3 centerGrid)
    {
        _mainCamera.orthographicSize = (widthPlane * width + heightPlane * height) / 2.5f;
        _mainCamera.transform.position = new Vector3(centerGrid.x, 140, centerGrid.z);
    }
  
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hearthObject;
    public List<GameObject> heartsObjects, startHearthsObjects;
    public List<int[]> positionHearts, startHeartsPosition;
    private int minProbHearth, _livesAvaiable;
    public List<RawImage> hearthsImages;
    public AudioSource takeHearthSound;
    public bool isInitialized;
    
    public void Start()
    {
        positionHearts = new List<int[]>();
        startHeartsPosition = new List<int[]>();
        heartsObjects = new List<GameObject>();
        startHearthsObjects = new List<GameObject>();

        minProbHearth = 0;
        _livesAvaiable = 3;
        isInitialized = true;
    }

    public void ResetHearthsLevel()
    {
        positionHearts = new List<int[]>(startHeartsPosition);
        heartsObjects = new List<GameObject>(startHearthsObjects);
        foreach (var heart in heartsObjects)
        {
            heart.SetActive(true);
        }
        
    }
    
    public bool GetHearth(int[] playerPosition)
    {
        int index = 0;
        bool heartGot = false;
        
        List<int> hearthsToRemove = new List<int>();
        foreach (var posHeart in positionHearts)
        {
            if (posHeart[0] == playerPosition[0] && posHeart[1] == playerPosition[1])
            {
                if (!takeHearthSound.isPlaying)
                {
                    takeHearthSound.Play();
                }

                if (_livesAvaiable < 3)
                {
                    _livesAvaiable += 1;
                    hearthsImages[_livesAvaiable - 1].enabled = true;
                }

                heartsObjects[index].SetActive(false);
                heartsObjects.RemoveAt(index);
                hearthsToRemove.Add(index);
                heartGot = true;
            }

            index += 1;
        }

        foreach (var i in hearthsToRemove)
        {
            positionHearts.RemoveAt(i);
        }

        return heartGot;
    }

    public bool CheckIfPlayerAllHearts()
    {
        return _livesAvaiable == 3;
    }

    // Update is called once per frame
    void Update()
    {
        _rotateHearth();
    }

    private void _rotateHearth()
    {
        foreach (var h in heartsObjects)
        {
            h.transform.Rotate(0, Time.deltaTime * 100f, 0, Space.Self);
        }
    }

    /*
     * ResetHearthsToEmpty
     * When a new level appears, we destroy all hearths, and a new level should start.
     */

    public void DestroyHearts()
    {
        // Empty previous list of hearths
        foreach (var h in heartsObjects)
        {
            Destroy(h);
        }

        foreach (var h in startHearthsObjects)
        {
            Destroy(h);
        }

        heartsObjects.Clear();
        positionHearts.Clear();
        startHearthsObjects.Clear();
        startHeartsPosition.Clear();
    }


    /*
     * GenerateExtraLife()
     * Maybe generates an extra life in the game, depends on the probability
     */
    public void GenerateExtraLives()
    {
        // Gets the map
        MapGenerator map = GameObject.Find("Map").GetComponent<MapGenerator>();
        int mapWidth = map.width;
        int mapHeight = map.height;
        int[] mapCenter = map.center;

        int probHearth = Random.Range(1, 100);

        if (probHearth >= minProbHearth)
        {
            int widthValue = Random.Range(2, mapWidth - 2);
            int heightValue = Random.Range(2, mapHeight - 2);

            while ((widthValue == mapCenter[0] && heightValue == mapCenter[1] ||
                    widthValue == mapCenter[0] + 1 && heightValue == mapCenter[1] ||
                    widthValue == mapCenter[0] - 1 && heightValue == mapCenter[1] ||
                    widthValue == mapCenter[0] && heightValue == mapCenter[1] + 1 ||
                    widthValue == mapCenter[0] && heightValue == mapCenter[1] - 1) ||
                   !map.arrOfPlanes[widthValue, heightValue].GetComponent<Renderer>().material.color
                       .Equals(new Color(255, 255, 255)))
            {
                widthValue = Random.Range(2, mapWidth - 2);
                heightValue = Random.Range(2, mapHeight - 2);
            }

            GameObject hearth = Instantiate(hearthObject, new Vector3(
                map.arrOfPlanes[widthValue, heightValue].transform.position.x,
                10, map.arrOfPlanes[widthValue, heightValue].transform.position.z), hearthObject.transform.rotation);
            hearth.transform.localScale = new Vector3(175f, 175f, 175f);

            positionHearts.Add(new[] {widthValue, heightValue});
            heartsObjects.Add(hearth);
        }

        startHearthsObjects = new List<GameObject>(heartsObjects);
        startHeartsPosition = new List<int[]>(positionHearts);
    }

    public void DecreaseLive()
    {
        hearthsImages[_livesAvaiable - 1].enabled = false;
        _livesAvaiable -= 1;
        heartsObjects = new List<GameObject>(startHearthsObjects);
        positionHearts = new List<int[]>(startHeartsPosition);

        foreach (var h in heartsObjects)
        {
            h.SetActive(true);
        }
    }

    public bool CheckIfAllLivesGone()
    {
        return _livesAvaiable == 0;
    }
}
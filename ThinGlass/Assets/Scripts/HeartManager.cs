using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hearthObject;
    public List<GameObject> hearthsObjects, startHearthsObjects;
    public List<int[]> positionHearths, startHearthsPosition;
    private int minProbHearth, _livesAvaiable;
    public List<RawImage> hearthsImages;
    public AudioSource takeHearthSound;
    private List<int[]> positionHearts;

    void Awake()
    {
        positionHearths = new List<int[]>();
        startHearthsPosition = new List<int[]>();
        hearthsObjects = new List<GameObject>();
        startHearthsObjects = new List<GameObject>();

        minProbHearth = 50;
        _livesAvaiable = 3;
    }

    public void GetHearth(int[] playerPosition)
    {
        int index = 0;
        List<int> hearthsToRemove = new List<int>();
        foreach (var pos in positionHearts)
        {
            if (pos[0] == playerPosition[0] && pos[1] == playerPosition[1])
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

                hearthsObjects[index].SetActive(false);
                hearthsObjects.RemoveAt(index);
                hearthsToRemove.Add(index);
            }

            index += 1;
        }

        foreach (var i in hearthsToRemove)
        {
            positionHearths.RemoveAt(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _rotateHearth();
    }

    private void _rotateHearth()
    {
        foreach (var h in hearthsObjects)
        {
            h.transform.Rotate(0, Time.deltaTime * 100f, 0, Space.Self);
        }
    }

    /*
     * ReserHearthsToEmpty
     * When a new level appears, we destroy all hearths, and a new level should start.
     */

    public void DestroyHearts()
    {
        // Empty previous list of hearths
        foreach (var h in hearthsObjects)
        {
            Destroy(h);
        }

        foreach (var h in startHearthsObjects)
        {
            Destroy(h);
        }

        hearthsObjects.Clear();
        positionHearths.Clear();
        startHearthsObjects.Clear();
        startHearthsPosition.Clear();
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

            positionHearths.Add(new[] {widthValue, heightValue});
            hearthsObjects.Add(hearth);
        }
    }

    public void DecreaseLive()
    {
        hearthsImages[_livesAvaiable - 1].enabled = false;
        _livesAvaiable -= 1;
        hearthsObjects = new List<GameObject>(startHearthsObjects);
        positionHearths = new List<int[]>(startHearthsPosition);

        foreach (var h in hearthsObjects)
        {
            h.SetActive(true);
        }
    }

    public bool CheckIfAllLivesGone()
    {
        return _livesAvaiable == 0;
    }
}
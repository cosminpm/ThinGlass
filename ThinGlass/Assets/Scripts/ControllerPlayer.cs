using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ControllerPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _map;
    private MapGenerator _scriptMap;
    private int[] _actualPosition;
    [HideInInspector]
    public int[] startingPosition;
    private List<int[]> _glassStepped;
    public AudioSource moveSound;
    public AudioSource dieSound;
    public AudioSource winSound;
    public AudioSource byebyeSound;
    public Text scoreText;
    private int _score = 0;
    public List<RawImage> hearthsImages;
    private int _hearthsAvailable;
    public Text youDiedText;
    public Button resetButton;
    private int _totalScore;
    IEnumerator Start()
    {
        _map = GameObject.Find("Map");
        yield return new WaitUntil(() => _map.GetComponent<MapGenerator>().isInitialized);
        _scriptMap = _map.GetComponent<MapGenerator>();
        // To put the player in the middle of the map
        int[] center = _scriptMap.center;
        _actualPosition = center;
        gameObject.transform.localPosition = 
            new Vector3(_scriptMap.ArrOfPlanes[center[0],center[1]].transform.localPosition.x, 10,_scriptMap.ArrOfPlanes[center[0],center[1]].transform.localPosition.z);
        startingPosition = center;
        // To initializate the blocks the player stepped on
        _glassStepped = new List<int[]>();
        // To write the 0 on the score
        scoreText.text = _totalScore.ToString();
        _hearthsAvailable = 3;
        youDiedText.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        ControllerPLayer();
    }

    private void ControllerPLayer()
    {
        if (Input.GetKeyDown("up"))
        {
           ActionsWhenMove(-1,0);
        }
        if (Input.GetKeyDown("down"))
        {
            ActionsWhenMove(1,0 );
        }
        if (Input.GetKeyDown("left"))
        {
            ActionsWhenMove(0, -1);
        }
        if (Input.GetKeyDown("right"))
        {
            ActionsWhenMove(0, 1);
        }
    }

    private void ActionsWhenMove(int x, int z)
    {
        int[] previousMove = { _actualPosition[0], _actualPosition[1]};
        
        if(!moveSound.isPlaying) {
            moveSound.Play();
        }
        
        if (!CheckMovement(_actualPosition[0] + x, _actualPosition[1] + z))
        {
            MoveCube(x, z);
            
            // If winner goes to exit and finishes
            if (CheckIfExit(_actualPosition[0], _actualPosition[1]))
            {
                if(!winSound.isPlaying) {
                    winSound.Play();
                }
                
                _totalScore += _score;
                _scriptMap.GenerateNextLevel();
                return;
            }
            
            BreakPanel(previousMove[0], previousMove[1]);
            _score += 1;
            int auxScore = _totalScore + _score;
            scoreText.text = auxScore.ToString();
            _glassStepped.Add(_actualPosition);
        }
        // Player dies "Ups"
        else
        {
           
            // Player still has lives
            if (_hearthsAvailable > 0)
            {
                ResetMap();
                if(!dieSound.isPlaying) {
                    dieSound.Play();
                }
                hearthsImages[_hearthsAvailable - 1].enabled = false;
                _hearthsAvailable -= 1;
            }
            // Player has no lives left
            else
            {
                if(!byebyeSound.isPlaying) {
                    byebyeSound.Play();
                }
                youDiedText.enabled = true;
                _map.SetActive(false);
                gameObject.SetActive(false);
                resetButton.gameObject.SetActive(false);
            }
            
        }
    }
    
    // Reset the map to its origina position
    public void ResetMap()
    {
        // Reset position when player steps incorrectly
        gameObject.transform.localPosition = new Vector3(_scriptMap.ArrOfPlanes[startingPosition[0],startingPosition[1]].transform.localPosition.x, 10,_scriptMap.ArrOfPlanes[startingPosition[0],startingPosition[1]].transform.localPosition.z);
        _actualPosition = startingPosition;
        // Reset all the red boxes the player stepped into white
        foreach (var pos in _glassStepped)
        {
            _scriptMap.ArrOfPlanes[pos[0],pos[1]].GetComponent<Renderer>().material.color = new Color(255, 255, 255); 
        }
        _glassStepped.Clear();
        _score = 0;
        scoreText.text = _totalScore.ToString();
        
    }
    
    private void BreakPanel(int i, int j)
    {
        _scriptMap.ArrOfPlanes[i,j].GetComponent<Renderer>().material.color = new Color(255, 0, 0); 
    }

    private void MoveCube(int x, int z)
    {
        _actualPosition = new []{_actualPosition[0] + x, _actualPosition[1] + z};
        gameObject.transform.localPosition =
            new Vector3(_scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.localPosition.x, 
                5, _scriptMap.ArrOfPlanes[_actualPosition[0], _actualPosition[1]].transform.localPosition.z);
    }

    private bool CheckMovement(int x, int z)
    {
        if (CheckOutsideMap(x, z))
        {
            return true;
        }
        if (_scriptMap.ArrOfPlanes[x, z].GetComponent<Renderer>().material.color.Equals(new Color(255, 0, 0)))
        {
            return true;
        }
        return false;
    }

    private bool CheckOutsideMap(int x, int z)
    {
        if (x >= _scriptMap.width || x < 0 || z >= _scriptMap.height || z < 0)
        {
            return true;
        }
        return false;
    }

    // Check if the player arrived to the exit
    private bool CheckIfExit(int x, int z)
    {
        if (x == _scriptMap.exitCoor[0] && z == _scriptMap.exitCoor[1])
            return true;
        return false;
    }


    
}

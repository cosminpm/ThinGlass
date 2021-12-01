using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelController : MonoBehaviour
{
    private HeartManager _heartManager;
    private MapGenerator _mapGenerator;
    private ControllerPlayer _controllerPlayer;
    private CameraManager _cameraManager;
    private GameObject _map;
    private CheckSolution _checkerSolution;
    
    public int pointsNeeded;
    private int _level, _scoreActualLevel;
    public Text scoreText, pointsNeededText, levelText, youDiedText;

    private readonly Color _badBoxesColor = new Color(255, 0, 0);
    private readonly Color _goodBoxesColor = new Color(255, 255, 255);

    
    public AudioSource dieSound, winSound, byebyeSound;
    IEnumerator Start()
    {
        _heartManager = gameObject.GetComponent<HeartManager>();
        _map = GameObject.Find("Map");
        _mapGenerator = _map.GetComponent<MapGenerator>();
        _controllerPlayer = GameObject.Find("Player").GetComponent<ControllerPlayer>();
        _cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        _checkerSolution = GetComponent<CheckSolution>();
        
        // Updates of the others methods can start
        yield return new WaitUntil(() => _heartManager.isInitialized);
        yield return new WaitUntil(() => _cameraManager.isInitialized);
        GenerateLevel();
        yield return new WaitUntil(() => _mapGenerator.isInitialized);
        yield return new WaitUntil(() => _controllerPlayer.isInitialized);
        
        
        _controllerPlayer.SetPosition(_mapGenerator.GetCenterVector3(), _mapGenerator.GetCenter());
        youDiedText.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //checkerSolution.AlgorithmDFS();
       
    }

    private void PlayerMoves()
    {
        int[] previousPosition = (int[]) _controllerPlayer.GetActualPosition().Clone();
        // Gets the position where the player wants to move
        _mapGenerator.BreakPanel(previousPosition);
        int[] actualPosition = _controllerPlayer.MovePlayer();
        // Moves the player to that direction

        if (previousPosition[0] != actualPosition[0] || previousPosition[1] != actualPosition[1])
        {
            if (CheckIfFinish(actualPosition))
            {
                GenerateNextLevel();
            }
            
            if (!CheckIfBadMove(actualPosition))
            {
                // Moves the player if everything is correct
                _controllerPlayer.SetPosition(_mapGenerator.GetCenterOfPanel(actualPosition), actualPosition);
                IncrActualLvlScore(1);
                
                if(_heartManager.GetHearth(actualPosition) && _heartManager.CheckIfPlayerAllHearts())
                {
                    IncrActualLvlScore(10);
                }
            }
            
            else
            {
                // Removes the last glass stepped so it does not count as white box
                try
                {
                    _controllerPlayer.glassStepped.RemoveAt(_controllerPlayer.glassStepped.Count - 1);
                }
                catch
                {
                    // ignored
                }

                DecreaseLives();
            }
        }
    }

    private void IncrActualLvlScore(int score)
    {
        _scoreActualLevel += score;
        scoreText.text = _scoreActualLevel.ToString();
    }

    public bool CheckIfBadMove(int[] position)
    {
        if (CheckOutsideMap(position) || _mapGenerator.GetColorPanel(position).Equals(_badBoxesColor))
        {
            return true;
        }
        return false;
    }

    private void DecreaseLives()
    {
        RelocateToMap();
        if (!dieSound.isPlaying)
        {
            dieSound.Play();
        }

        _heartManager.DecreaseLive();


        if (_heartManager.CheckIfAllLivesGone())
        {
            if (!byebyeSound.isPlaying)
            {
                byebyeSound.Play();
            }

            youDiedText.enabled = true;
            _map.SetActive(false);
        }
    }

    private void GenerateLevel()
    {
        // Generates new map
        _mapGenerator.IncreaseLevelOfDiff();
        _mapGenerator.GenerateMap(_badBoxesColor, _goodBoxesColor);
        _heartManager.GenerateExtraLives();

        RelocateToMap();
        IncreasePointsNeeded();

        Vector3 centerOfCamera = Vector3.Lerp(_mapGenerator.arrOfPlanes[0, 0].transform.position,
            _mapGenerator.arrOfPlanes[_mapGenerator.width - 1, _mapGenerator.height - 1].transform.position, 0.5f);
        
        _cameraManager.EditCamera(_mapGenerator.width, _mapGenerator.height, _mapGenerator.widthPlane,
            _mapGenerator.heightPlane, centerOfCamera);
        
        
    }

    public void GenerateNextLevel()
    {
        // Destroys Previous Level
        _mapGenerator.DestroyMap();
        _heartManager.DestroyHearts();
        _controllerPlayer.glassStepped = new List<int[]>();
        GenerateLevel();
        _checkerSolution.AlgorithmDfs();
    }

    private void IncreasePointsNeeded()
    {
        int height = _mapGenerator.height;
        int width = _mapGenerator.width;
        pointsNeeded = (height * width / 10);

        pointsNeededText.text = pointsNeeded.ToString();
        _level += 1;
        levelText.text = _level.ToString();
    }


    // Reset the map to its origina position
    private void RelocateToMap()
    {
        _controllerPlayer.SetPosition(_mapGenerator.GetCenterVector3(), _mapGenerator.GetCenter());
        _mapGenerator.SetPanelsToColor(_controllerPlayer.glassStepped, _goodBoxesColor);

        _scoreActualLevel = 0;
        scoreText.text = "0";
        _heartManager.ResetHearthsLevel();
    }


    // Check if the player arrived to the exit
    private bool CheckIfExit(int[] playerPosition)
    {

        if (playerPosition[0] == _mapGenerator.exitCoor[0] && playerPosition[1] == _mapGenerator.exitCoor[1])
            return true;
        return false;
    }

    private bool CheckIfFinish(int[] playerPosition)
    {
        if (_scoreActualLevel >= pointsNeeded && CheckIfExit(playerPosition))
        {
            return true;
        }
        
        return false;
    }

    private bool CheckOutsideMap(int[] position)
    {
        if (position[0] >= _mapGenerator.width || position[0] < 0 || position[1] >= _mapGenerator.height ||
            position[1] < 0)
        {
            return true;
        }
        return false;
    }
}
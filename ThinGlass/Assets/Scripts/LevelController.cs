using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [HideInInspector] public bool isInitialized;
    private HeartManager _heartManager;
    private MapGenerator _mapGenerator;
    private ControllerPlayer _controllerPlayer;
    private CameraManager _cameraManager;
    private GameObject _map;
    private int _pointsNeeded, _level, _scoreActualLevel, _totalScore;
    public Text _scoreText, _pointsNeededText, _levelText, _youDiedText;

    private Color badBoxesColor = new Color(255, 0, 0);
    private Color goodBoxesColor = new Color(255, 255, 255);

    public AudioSource dieSound, winSound, byebyeSound;

    IEnumerator Start()
    {
        _heartManager = gameObject.GetComponent<HeartManager>();
        _map = GameObject.Find("Map");
        _mapGenerator = _map.GetComponent<MapGenerator>();
        _controllerPlayer = GameObject.Find("Player").GetComponent<ControllerPlayer>();
        _cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();

        // Updates of the others methods can start
        yield return new WaitUntil(() => _heartManager.isInitialized);
        yield return new WaitUntil(() => _cameraManager.isInitialized);
        GenerateLevel();
        yield return new WaitUntil(() => _mapGenerator.isInitialized);
        yield return new WaitUntil(() => _controllerPlayer.isInitialized);
        
        _controllerPlayer.SetPosition(_mapGenerator.GetCenterVector3(), _mapGenerator.GetCenter());
        _youDiedText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMoves();
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
            if (!CheckIfBadMove(actualPosition))
            {
                // Moves the player if everything is correct
                _controllerPlayer.SetPosition(_mapGenerator.GetCenterOfPanel(actualPosition), actualPosition);
                // _heartManager.GetHearth(actualPosition);
                if (CheckIfFinish(actualPosition))
                {
                     GenerateNextLevel();
                }
            }
            else
            {
                // Removes the last glass stepped so it does not count as white box
                _controllerPlayer.glassStepped.RemoveAt(_controllerPlayer.glassStepped.Count - 1);
                
                DecreaseLives();
            }
        }
    }

    private bool CheckIfBadMove(int[] position)
    {
        if (_mapGenerator.GetColorPanel(position).Equals(badBoxesColor) || CheckOutsideMap(position))
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

            _youDiedText.enabled = true;
            _map.SetActive(false);
            //resetButton.gameObject.SetActive(false);
        }
    }

    private void GenerateLevel()
    {
        // Generates new map
        _mapGenerator.IncreaseLevelOfDiff();
        _mapGenerator.GenerateMap(badBoxesColor, goodBoxesColor);
        _heartManager.GenerateExtraLives();

        RelocateToMap();
        IncreasePointsNeeded();
        _cameraManager.EditCamera(_mapGenerator.width, _mapGenerator.height, _mapGenerator.widthPlane,
            _mapGenerator.heightPlane, _mapGenerator.GetCenterVector3());
    }

    public void GenerateNextLevel()
    {
        // Destroys Previous Level
        _mapGenerator.DestroyMap();
        _heartManager.DestroyHearts();

        GenerateLevel();
    }

    private void IncreasePointsNeeded()
    {
        int height = _mapGenerator.height;
        int width = _mapGenerator.width;
        _pointsNeeded = (height * width / 3) + _totalScore - 1;

        _pointsNeededText.text = _pointsNeeded.ToString();
        _level += 1;
        _levelText.text = _level.ToString();
    }


    // Reset the map to its origina position
    private void RelocateToMap()
    {
        _controllerPlayer.SetPosition(_mapGenerator.GetCenterVector3(), _mapGenerator.GetCenter());
        _mapGenerator.SetPanelsToColor(_controllerPlayer.glassStepped, goodBoxesColor);

        _scoreActualLevel = 0;
        _scoreText.text = _totalScore.ToString();
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
        if (_scoreActualLevel > _pointsNeeded && CheckIfExit(playerPosition))
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
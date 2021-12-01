using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckSolution : MonoBehaviour
{
    public class Node
    {
        public int hashCode;
        public int[] position;

        public Node(int hashCode, int[] position)
        {
            this.hashCode = hashCode;
            this.position = position;
        }

        public Node Clone()
        {
            Node n = new Node(hashCode, position);
            position = (int[]) position.Clone();
            return n;
        }
    }

    // Start is called before the first frame update
    private int[] _startPosition, _goalPosition;
    private MapGenerator _map;
    private LevelController _lvlController;
    private ControllerPlayer _controllerPlayer;
    private IDictionary<int, Node> _nodeParents = new Dictionary<int, Node>();


    IEnumerator Start()
    {
        _map = GameObject.Find("Map").GetComponent<MapGenerator>();
        _controllerPlayer = GameObject.Find("Player").GetComponent<ControllerPlayer>();

        yield return new WaitUntil(() => _map.isInitialized);
        yield return new WaitUntil(() => _controllerPlayer.isInitialized);

        _lvlController = GetComponent<LevelController>();
        
    }

    public void AlgorithmDfs()
    {
        _startPosition = _map.center;
        _goalPosition = (int[]) _map.exitCoor.Clone();
        _nodeParents.Clear();
        
        
        var visited = new HashSet<int>();
        var stack = new Stack<int[]>();

        stack.Push(_startPosition);
        while (stack.Count != 0)
        {
            var current = stack.Pop();
            
            if (current[0] == _goalPosition[0] && current[1] == _goalPosition[1] && GetLengthOfSolution() >= _lvlController.pointsNeeded)
            {
                PrintSolution();
                return;
            }

            if (!visited.Add(GetHashCode(current)))
                continue;

            var nodes = GetNeighbours(current)
                .Where(n => !visited.Contains(GetHashCode(n)));

            foreach (var neighbour in nodes)
            {
                stack.Push(neighbour);
                try
                {
                    _nodeParents.Add(GetHashCode(neighbour),new Node(GetHashCode(current),current));
                }
                catch
                {
                    // ignored
                }
            }
        }

        Debug.Log("NO ENCONTRAMOS LA SOLUCION :(");
    }

    private int GetLengthOfSolution()
    {
        int value = 0;
        Node aux = new Node(GetHashCode(_goalPosition),(int[]) _goalPosition.Clone());
        
        while (aux.hashCode != GetHashCode(_startPosition))
        {
            value += 1;
            aux = _nodeParents[aux.hashCode].Clone(); 
        }

        return value;
    }

    public void PrintSolution()
    {
        float value = 0;
        Node aux = new Node(GetHashCode(_goalPosition),_goalPosition);

        int length = GetLengthOfSolution();
        while (aux.hashCode != GetHashCode(_startPosition))
        {
            value += 1;

            _map.arrOfPlanes[aux.position[0], aux.position[1]].GetComponent<Renderer>().material.color =
                Color.Lerp(Color.yellow, Color.blue, value / length);
            
            aux = _nodeParents[aux.hashCode]; 
        }
        
        Debug.Log(value);
        _map.arrOfPlanes[_goalPosition[0], _goalPosition[1]].GetComponent<Renderer>().material.color = Color.green;
    }

    public int GetHashCode(int[] array)
    {
        return ((IStructuralEquatable) array).GetHashCode(EqualityComparer<int>.Default);
    }

    public List<int[]> GetNeighbours(int[] pos)
    {
        List<int[]> result = new List<int[]>();

        var tryNeigh = new[] {pos[0] + 1, pos[1]};
        
        if (!_lvlController.CheckIfBadMove(tryNeigh))
        {
            result.Add(tryNeigh);
        }
        
        tryNeigh = new[] {pos[0], pos[1] + 1};
        if (!_lvlController.CheckIfBadMove(tryNeigh))
        {
            result.Add(tryNeigh);
        }

        tryNeigh = new[] {pos[0], pos[1] - 1};
        if (!_lvlController.CheckIfBadMove(tryNeigh))
        {
            result.Add(tryNeigh);
        }
        
        tryNeigh = new[] {pos[0] - 1, pos[1]};
        if (!_lvlController.CheckIfBadMove(tryNeigh))
        {
            result.Add(tryNeigh);
        }
        
        for (int i = 0; i < result.Count; i++) {
            int[] temp = (int[]) result[i].Clone();
            int randomIndex = Random.Range(i, result.Count);
            result[i] = (int[]) result[randomIndex].Clone();
            result[randomIndex] = temp;
        }

        return result;
    }
}
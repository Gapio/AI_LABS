using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grid
{
    public class Pathfinder : MonoBehaviour
    {
        private GridManager _gridManager;
        private readonly float _stepCost = 1f;
        private InputAction _findPathAction;
        public List<Node> CalculatedPath;

        private void Awake()
        {
            _gridManager = GetComponent<GridManager>();
        }

        private void OnEnable()
        {
            _findPathAction = new InputAction(name: "findPath", type: InputActionType.Button, binding: "<Mouse>/rightButton");
            _findPathAction.performed += OnFindPathPerformed;
            _findPathAction.Enable();

        }
        
        private void OnDisable()
        {
            if (_findPathAction != null)
            {
                _findPathAction.performed -= OnFindPathPerformed;
                _findPathAction.Disable();
                _findPathAction.Dispose();
            }
        }


        private void OnFindPathPerformed(InputAction.CallbackContext context)
        {
           FindPath();
        }

        private void FindPath()
        {
             
            //somehow select start and goal
            Node startNode = _gridManager.GetNode(0,0);
            Node goalNode = _gridManager.GetNode(_gridManager.Width - 1, _gridManager.Height - 1);
            
            for (int i = 0; i < _gridManager.Width; i++)
            {
                for (int j = 0; j < _gridManager.Height; j++)
                {
                    Node node = _gridManager.GetNode(i, j);
                    _gridManager.SetTileMaterial(node, !node.Walkable ? _gridManager.wallMaterial : _gridManager.walkableMaterial);
                }
            }

            CalculatedPath = FindPath(startNode, goalNode);
            _gridManager.SetTileMaterial(startNode, _gridManager.startMaterial);
            _gridManager.SetTileMaterial(goalNode, _gridManager.goalMaterial);
        }

        private List<Node> FindPath(Node startNode, Node goalNode)
        {

            // 1. Reset node costs
            _gridManager.ResetNodeCosts();
            
            // 2. Initialize openSet and closedSet
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            
            // 3. Set gCost and hCost for startNode
            startNode.GCost = 0;
            startNode.HCost = HeuristicCost(startNode, goalNode);
            openSet.Add(startNode);
            
            // 4. Loop until openSet is empty:
            // - pick node with lowest fCost
            // - if this is goalNode, reconstruct and return path
            
            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || (Mathf.Approximately(openSet[i].FCost, currentNode.FCost) && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                if (currentNode == goalNode)
                {
                    List<Node> path = new List<Node>();
                    
                    while (currentNode != null)
                    {
                        path.Add(currentNode);
                        _gridManager.SetTileMaterial(currentNode, _gridManager.pathMaterial);
                        if (currentNode == startNode)
                        {
                            break;
                        }
                        currentNode = currentNode.Parent;
                    }
                    path.Reverse();
                    return path;
                }
                // - otherwise, move it to closedSet
                // - for each neighbour:
                // - skip if null, not walkable, or in closedSet
                // - compute tentativeG = current.gCost + stepCost
                // - if tentativeG < neighbour.gCost:
                // - update neighbour.parent, gCost, hCost
                // - ensure neighbour is in openSet
                
                closedSet.Add(currentNode);
                openSet.Remove(currentNode);

                foreach (Node neighbour in _gridManager.GetNeighbours(currentNode))
                {
                    if (closedSet.Contains(neighbour) || neighbour == null || !neighbour.Walkable)
                    {
                        continue;
                    }

                    float tentativeG = currentNode.GCost + _stepCost;
                    if (tentativeG < neighbour.GCost)
                    {
                        neighbour.Parent = currentNode;
                        neighbour.GCost = tentativeG;
                        neighbour.HCost = HeuristicCost(neighbour, goalNode);
                    }

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
            
            return null;
        }

        private float HeuristicCost(Node a, Node b)
        {
            int dx = Mathf.Abs(a.X - b.X);
            int dy = Mathf.Abs(a.Y - b.Y);
            return dx + dy;
        }
    }
}

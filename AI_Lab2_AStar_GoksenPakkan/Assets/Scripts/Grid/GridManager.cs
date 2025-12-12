using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 1f;
        
        [Header("Prefabs & Materials")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] public Material walkableMaterial;
        [SerializeField] public Material wallMaterial;
        [SerializeField] public Material pathMaterial;
        
        private Node[,] _nodes;
        private readonly Dictionary<GameObject, Node> _tileToNode = new();
        
        // Input action for click
        private InputAction _clickAction;
        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;
        
        private void Awake()
        {
            GenerateGrid();
        }
        private void OnEnable()
        {
            _clickAction = new InputAction(
                name: "Click",
                type: InputActionType.Button,
                binding: "<Mouse>/leftButton"
            );
            _clickAction.performed += OnClickPerformed;
            _clickAction.Enable();
        }
        private void OnDisable()
        {
            if (_clickAction != null)
            {
                _clickAction.performed -= OnClickPerformed;
                _clickAction.Disable();
            }
        }
        private void GenerateGrid()
        {
            _nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPos = new Vector3(x * cellSize, 0f, y * cellSize);
                    GameObject tileGo = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                    tileGo.name = $"Tile_{x}_{y}";
                    Node node = new Node(x, y, true, tileGo);
                    _nodes[x, y] = node;
                    _tileToNode[tileGo] = node;
                    SetTileMaterial(node, walkableMaterial);
                }
            }
        }
        private void OnClickPerformed(InputAction.CallbackContext
            ctx)
        {
            HandleMouseClick();
        }
        private void HandleMouseClick()
        {
            Camera cam = Camera.main;
            if (cam == null) return;
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clicked = hit.collider.gameObject;
                if (_tileToNode.TryGetValue(clicked, out Node node))
                {
                    bool newWalkable = !node.Walkable;
                    SetWalkable(node, newWalkable);
                }
            }
        }
        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return null;
            return _nodes[x, y];
        }
        
        public Node GetNodeFromWorldPosition(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x / cellSize);
            int y = Mathf.RoundToInt(worldPos.z / cellSize);
            return GetNode(x, y);
        }
        
        public IEnumerable<Node> GetNeighbours(Node node, bool allowDiagonals = false)
        {
            int x = node.X;
            int y = node.Y;
            // 4-neighbour
            yield return GetNode(x + 1, y);
            yield return GetNode(x - 1, y);
            yield return GetNode(x, y + 1);
            yield return GetNode(x, y - 1);
            if (allowDiagonals)
            {
                yield return GetNode(x + 1, y + 1);
                yield return GetNode(x - 1, y + 1);
                yield return GetNode(x + 1, y - 1);
                yield return GetNode(x - 1, y - 1);
            }
        }
        private void SetWalkable(Node node, bool walkable)
        {
            node.Walkable = walkable;
            SetTileMaterial(node, walkable ? walkableMaterial : wallMaterial);
        }
        

        public void SetTileMaterial(Node node, Material mat)
        {
            var meshRenderer = node.Tile.GetComponent<MeshRenderer>();
            if (meshRenderer != null && mat != null)
            {
                meshRenderer.material = mat;
            }
        }

        public void ResetTileMaterials()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SetTileMaterial(GetNode(x, y), null);
                }
            }
        }
        
        public void ResetNodeCosts()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _nodes[x, y].GCost = float.PositiveInfinity;
                    _nodes[x, y].HCost = 0f;
                    _nodes[x, y].Parent = null;
                }
            }
        }
    }
}
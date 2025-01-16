using System.Collections;
using System.Collections.Generic; // For using lists (optional)
using UnityEngine;
using UnityHFSM;
using System;
using System.IO;

public class AstarBehavior : MonoBehaviour
{
    public int[,] grid; // Your 2D array grid
    public float moveSpeed = 0.5f;  // Speed of movement
    public int distance = 5;
    public Transform playerTransform;
    private List<Vector2Int> path; // Holds the resulting path
    private bool confirmNearest = false;
    private StateMachine fsm;
    private Vector2Int targetPos;
    private string filePath = "Assets/Visual/Maps/extracted_edges.txt";
    
    Vector2Int getNearestEdge(Vector2Int pos) {
        Debug.Log($"Starting BFS from position: {pos}");
        if (pos.x < 0 || pos.x >= grid.GetLength(0) || pos.y < 0 || pos.y >= grid.GetLength(1)) {
            Debug.LogError($"Invalid start position: {pos}");
            return new Vector2Int(-1, -1);
        }

        if (grid[pos.x, pos.y] == 1) {
            Debug.Log($"Starting position is already an edge: {pos}");
            return pos;
        }

        Vector2Int[] directions = {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(pos);
        visited.Add(pos);

        while (queue.Count > 0) {
            Vector2Int current = queue.Dequeue();
            foreach (var dir in directions) {
                Vector2Int neighbor = current + dir;

                // Check bounds
                if (neighbor.x >= 0 && neighbor.x < grid.GetLength(0) && 
                    neighbor.y >= 0 && neighbor.y < grid.GetLength(1)) {
                    
                    if (grid[neighbor.x, neighbor.y] == 1) {
                        Debug.Log($"Found nearest edge at: {neighbor}");
                        return neighbor;
                    }

                    if (!visited.Contains(neighbor)) {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("No edge found!");
        return new Vector2Int(-1, -1);
    }


    Vector2Int worldToGrid(Vector2Int vec) {
        return new Vector2Int(vec.x + grid.GetLength(0)/2, vec.y + grid.GetLength(1)/2);
    }

    Vector2Int worldToGrid(Vector3 vec) {
        return new Vector2Int(
            Mathf.FloorToInt(vec.x + grid.GetLength(0) / 2),
            Mathf.FloorToInt(vec.y + grid.GetLength(1) / 2)
        );
    }

    Vector3 gridToWorld(Vector2Int vec) {
        return new Vector3(
            vec.x - grid.GetLength(0) / 2,
            vec.y - grid.GetLength(1) / 2,
            0
        );
    }

    void farthestFromPlayer(Vector3 playerPos) {
        // To do
        var vec = worldToGrid(new Vector3(playerPos.x + distance, playerPos.y));
        targetPos = getNearestEdge(vec);
        Debug.Log(targetPos);
    }

    void LoadMap(string path)
    {
        try
        {
            // Read all lines from the file, ensuring to use "\n" to properly handle line endings
            string[] lines = File.ReadAllLines(path);

            // Assuming all lines should be the same length, determine number of columns from the first line
            int rows = lines.Length;
            int cols = lines[0].Trim().Length; // Trim leading/trailing spaces and check column length

            // Initialize the grid
            grid = new int[rows, cols];

            // Parse the file into the grid
            for (int y = 0; y < rows; y++)
            {
                string line = lines[y].Trim(); // Remove any leading/trailing whitespace
                
                if (line.Length != cols)
                {
                    Debug.LogError($"Line {y + 1} has an incorrect length. Expected {cols} characters, but got {line.Length}.");
                    return;
                }

                for (int x = 0; x < cols; x++)
                {
                    // Convert each character ('0' or '1') into an integer
                    grid[x, y] = line[x] == '1' ? 1 : 0;
                }
            }

            Debug.Log($"Map loaded successfully: {rows} x {cols}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load map: {e.Message}");
        }
    }

    void Start()
    {
        LoadMap(filePath);
        fsm = new StateMachine();
        fsm.AddState("Idle", onLogic: state => {
            if (grid == null) return;
            Debug.Log(transform.position);
            transform.position = gridToWorld(getNearestEdge(worldToGrid(transform.position)));
        });
        // fsm.AddState("Move", new CoState(
        //     this,
        //     walkPath,
        //     loop: false,
        //     needsExitTime:true
        // ));
        // fsm.AddTransition("Idle", "Move", transition => {
        //     farthestFromPlayer(transform.position);
        //     var path = FindPath(worldToGrid(transform.position), targetPos);
        //     Debug.Log(path);
        //     return path != null;
        // });
        // fsm.AddTransition("Move", "Idle", transition => worldToGrid(transform.position) == targetPos);
        fsm.SetStartState("Idle");
        fsm.Init();
    }

    private void Update() {
        fsm.OnLogic();
    }

    private IEnumerator walkPath() {
        foreach (Vector2Int targetPosition in path)
        {
            var toWorld = gridToWorld(targetPosition);
            Vector3 worldPosition = new Vector3(toWorld.x, toWorld.y, 0);

            // Move toward the target position
            while (Vector3.Distance(transform.position, worldPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, worldPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Ensure the object snaps exactly to the target position
            transform.position = worldPosition;
        }

        Debug.Log("Reached the last position!");
        //fsm.StateCanExit();
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        // Open and Closed lists
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        // Initialize the start node
        Node startNode = new Node(start, null, 0, Heuristic(start, goal));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Get the node with the lowest F cost
            Node currentNode = openList[0];
            foreach (var node in openList)
                if (node.F < currentNode.F)
                    currentNode = node;

            // Check if we've reached the goal
            if (currentNode.Position == goal)
                return ReconstructPath(currentNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Check neighbors
            foreach (var neighborPos in GetNeighbors(currentNode.Position))
            {
                if (!IsWalkable(neighborPos) || closedList.Contains(new Node(neighborPos)))
                    continue;

                int gCost = currentNode.G + 1; // Assume uniform cost
                Node neighbor = new Node(neighborPos, currentNode, gCost, Heuristic(neighborPos, goal));

                // If the neighbor is already in the open list with a lower cost, skip it
                Node openNeighbor = openList.Find(n => n.Position == neighborPos);
                if (openNeighbor != null && neighbor.G >= openNeighbor.G)
                    continue;

                // Add or update the neighbor in the open list
                if (openNeighbor != null)
                    openList.Remove(openNeighbor);
                openList.Add(neighbor);
            }
        }

        // No path found
        return null;
    }

    List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    IEnumerable<Vector2Int> GetNeighbors(Vector2Int position)
{
    // Define 8-directional movement (up, down, left, right, and diagonals)
    Vector2Int[] directions =
    {
        Vector2Int.up,              // Up
        Vector2Int.down,            // Down
        Vector2Int.left,            // Left
        Vector2Int.right,           // Right
        new Vector2Int(-1, 1),      // Top-left
        new Vector2Int(1, 1),       // Top-right
        new Vector2Int(-1, -1),     // Bottom-left
        new Vector2Int(1, -1)       // Bottom-right
    };

    foreach (var dir in directions)
    {
        Vector2Int neighbor = position + dir;
        // Check if the neighbor is within bounds of the grid
        if (neighbor.x >= 0 && neighbor.x < grid.GetLength(0) &&
            neighbor.y >= 0 && neighbor.y < grid.GetLength(1))
        {
            yield return neighbor;
        }
    }
}

    bool IsWalkable(Vector2Int position)
    {
        return grid[position.x, position.y] == 1;
    }

    int Heuristic(Vector2Int a, Vector2Int b)
    {
        // Manhattan distance
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Node class for A*
    private class Node
    {
        public Vector2Int Position;
        public Node Parent;
        public int G; // Cost from start
        public int H; // Heuristic cost to goal
        public int F => G + H; // Total cost

        public Node(Vector2Int position, Node parent = null, int g = 0, int h = 0)
        {
            Position = position;
            Parent = parent;
            G = g;
            H = h;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node other)
                return Position == other.Position;
            return false;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}
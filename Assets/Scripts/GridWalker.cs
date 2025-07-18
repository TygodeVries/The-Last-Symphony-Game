using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class GridWalker : MonoBehaviour
{
    [HideInInspector] [SerializeField] public Tile tile;
    public void SetTile(Tile tile)
    {
        this.tile = tile;

#if UNITY_EDITOR
        EditorUtility.SetDirty(tile);
        EditorUtility.SetDirty(this);
#endif
    }

    public static void SnapAll()
    {
        Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        GridWalker[] gridWalkers = UnityEngine.Object.FindObjectsByType<GridWalker>(FindObjectsSortMode.None);

        foreach (GridWalker gridWalker in gridWalkers)
        {
            Tile nearest = null;
            float nearestDistance = 1000;
            foreach (Tile tile in tiles)
            {
                float distance = Vector3.Distance(gridWalker.transform.position, tile.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = tile;
                }
            }

            if (nearest != null)
            {
                gridWalker.transform.position = nearest.transform.position;
                gridWalker.SetTile(nearest);
            }
            else
            {
                Debug.LogWarning($"Could not find good location for {gridWalker.transform.name}");
            }
        }
    }

    public void Start()
    {
        if(tile == null)
        {
            Debug.LogError("Walker has no tile assigned. Make sure to snap the walkers before starting!");
            Destroy(this.gameObject);
            return;
        }
    }

    public IEnumerator Navigate(Tile goal)
    {
        List<Tile> tiles = CalculatePath(goal, tile);
        yield return StartCoroutine(WalkPath(tiles));

        Debug.Log("Path Complete!");
    }

    private IEnumerator WalkPath(List<Tile> path)
    {
        if (path == null) {
            Debug.LogError("Walking a null path???!?");
            yield break;
        }

        for (int i = 0; i < path.Count; i++)
        {
            Tile tile = path[i];
            yield return StartCoroutine(Step(tile));
        }

        Debug.Log("Completed walk!");
    }

    private IEnumerator Step(Tile tile)
    {
        CameraSystem.SetTarget(transform);

        Vector3 startPos = transform.position;
        float waiting = 0.4f * 0.05f;
        for (float t = 0; t < 1f; t += 0.05f)
        {
            // Move
            transform.position = Vector3.Lerp(startPos, tile.transform.position, t);

            // Looking in the correct direction
            Vector3 delta = tile.transform.position - startPos;
            transform.forward = Vector3.Lerp(transform.forward, delta, Mathf.Clamp01(t * 2));

            // Wait for a bit
            yield return new WaitForSeconds(waiting);
        }

        transform.position = tile.transform.position;

        this.tile = tile;

        yield return StartCoroutine(tile.StepBehavior());
    }

    public static List<Tile> CalculatePath(Tile destination, Tile startingTile)
    {
        if(destination == null)
        {
            Debug.LogError("Destination cannot be null!");
            return null;
        }

        if (startingTile == null)
        {
            Debug.LogError("Starting tile cannot be null!");
            return null;
        }

        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Tile, Tile> from = new Dictionary<Tile, Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();

        queue.Enqueue(startingTile);
        visited.Add(startingTile);
        from[startingTile] = null;

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            if (current == destination)
            {
                return ReconstructPath(from, destination);
            }

            foreach (Tile neighbor in current.GetConnections())
            {
                if (!visited.Contains(neighbor) && neighbor.CanNavigateInto)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    from[neighbor] = current;
                }
            }
        }

        return null;
    }
    private static List<Tile> ReconstructPath(Dictionary<Tile, Tile> from, Tile end)
    {
        List<Tile> path = new List<Tile>();
        Tile current = end;

        while (current != null)
        {
            path.Add(current);
            current = from[current];
        }

        path.Reverse();
        return path;
    }

    public void OnDrawGizmos()
    {
        if (tile == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + new Vector3(0, 1, 0), 0.1f);
        }
    }
}

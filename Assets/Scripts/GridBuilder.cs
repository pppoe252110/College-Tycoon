using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    public Vector2 gridSize = Vector2.one * 8;
    public float cellSize = 8;
    [SerializeField]
    public Dictionary<Vector2, Building> grid;

    public static GridBuilder Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        grid = new Dictionary<Vector2, Building>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cellPos = GetCellPosition(x, y);
                grid.Add(new Vector2(cellPos.x, cellPos.z), null);
            }
        }

    }

    public T GetRandomBuildingTypeOf<T>() where T : Building
    {
        var buildings = GetAllBuildingsOfType<T>();
        if (buildings.Count<=0)
            return null;
        var b  =  buildings[Random.Range(0, buildings.Count)];
        return b;
    }

    public int GetFreeDormitoryPlaces()
    {
        int result = 0;
        var free = GetAllBuildingsOfType<DormitoryBuilding>();
        for (int i = 0; i < free.Count; i++)
        {
            result += free[i].maxPeople;
        }
        return result;
    }

    public List<T> GetAllBuildingsOfType<T>() where T : Building
    {
        var buildings = grid.Values.Where(b => b is T).Cast<T>().ToList();
        return buildings;
    }

    public List<DormitoryBuilding> GetAllDormitoryBuilldings()
    {
        var result = grid.Values.OfType<DormitoryBuilding>().ToList();
        return result;
    }

    public Vector2 GetClosestToPoint(Vector3 point)
    {
        return grid.Select(a => a.Key).OrderBy(o => Vector2.Distance(new Vector2(point.x, point.z), new Vector2(transform.position.x + o.x, transform.position.z + o.y))).FirstOrDefault();
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cellPos = GetCellPosition(x, y);
                Gizmos.DrawWireSphere(cellPos + transform.position, 1);
            }
        }
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        Vector3 cellPos = new Vector3(x * cellSize, 0, y * cellSize);
        return cellPos - new Vector3(gridSize.x, 0, gridSize.y) / 2f * cellSize + new Vector3(cellSize / 2f, 0, cellSize / 2f);
    }
}

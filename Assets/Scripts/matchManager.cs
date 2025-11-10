using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public int matchCount = 3;
    public GridManager gridManager;
    public float clearDelay = 1f;

    bool isClearing = false;

    void Awake()
    {
        if (!gridManager)
            gridManager = FindObjectOfType<GridManager>();
    }

    void Start()
    {
       
    }

    // 被 GridManager.UpdateGrid() 的 BroadcastMessage("GridChanged") 调用
    void GridChanged()
    {
        if (!gridManager || gridManager.gridList == null || gridManager.gridList.Count == 0)
            return;

     
        if (isClearing)
            return;

        var toRemove = FindMatchedBlocks();

        if (toRemove.Count > 0)
        {
            StartCoroutine(ClearAfterDelay(toRemove));
        }
        else
        {
            CheckWin();
        }
    }

    IEnumerator ClearAfterDelay(List<MatchBlock> blocks)
    {
        isClearing = true;
       foreach (var b in blocks)
        {
            if (b == null) continue;

       
            var sticky = b.GetComponent<Sticky>();
            if (sticky != null)
                sticky.enabled = false;

       
            b.canMove = false;
        }

    
        yield return new WaitForSeconds(clearDelay);

      
        var valid = new List<MatchBlock>();
        foreach (var b in blocks)
        {
            if (b != null)
                valid.Add(b);
        }

  
        if (valid.Count > 0)
        {
            SfxManager.PlayVanish();
            RemoveBlocks(valid);
        }


        if (gridManager != null)
            gridManager.UpdateGrid();

        isClearing = false;

       
        CheckWin();
    }

    List<MatchBlock> FindMatchedBlocks()
    {
        int width = gridManager.gridList.Count;
        int height = gridManager.gridList[0].Count;

        bool[,] visited = new bool[width, height];
        var result = new HashSet<MatchBlock>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = gridManager.gridList[x][y].GetComponent<Cell>();
                var mb = GetMatchBlock(cell);
                if (mb == null || visited[x, y])
                    continue;

                var group = new List<MatchBlock>();
                FloodFill(x, y, mb.typeId, visited, group);

                if (group.Count >= matchCount)
                {
                    foreach (var b in group)
                        result.Add(b);
                }
            }
        }

        return new List<MatchBlock>(result);
    }

    MatchBlock GetMatchBlock(Cell cell)
    {
        if (cell == null || cell.ContainObj == null) return null;
 
        return cell.ContainObj.GetComponent<MatchBlock>();
    }

    void FloodFill(int startX, int startY, int typeId, bool[,] visited, List<MatchBlock> group)
    {
        int width = gridManager.gridList.Count;
        int height = gridManager.gridList[0].Count;

        var stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            var p = stack.Pop();
            int x = p.x;
            int y = p.y;

            if (x < 0 || x >= width || y < 0 || y >= height) continue;
            if (visited[x, y]) continue;

            var cell = gridManager.gridList[x][y].GetComponent<Cell>();
            var mb = GetMatchBlock(cell);
            if (mb == null || mb.typeId != typeId) continue;

            visited[x, y] = true;
            group.Add(mb);

            stack.Push(new Vector2Int(x + 1, y));
            stack.Push(new Vector2Int(x - 1, y));
            stack.Push(new Vector2Int(x, y + 1));
            stack.Push(new Vector2Int(x, y - 1));
        }
    }

    void RemoveBlocks(List<MatchBlock> blocks)
    {
        foreach (var b in blocks)
        {
            if (b == null) continue;

           
            Cell cell = null;
            if (b.transform.parent != null)
                cell = b.transform.parent.GetComponent<Cell>();

            if (cell != null && cell.ContainObj == b.gameObject)
            {
                cell.RemoveContainObj();
            }

            Destroy(b.gameObject);
        }
    }

    void CheckWin()
    {
        var all = FindObjectsOfType<MatchBlock>();
        if (all.Length == 0 && GameManager.I != null)
        {
            GameManager.I.LevelCompleted();
        }
    }
}

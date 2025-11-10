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

        // 正在执行一轮消除流程时，忽略新的 GridChanged（避免递归）
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

        // 先冻结这些块，特别是 Sticky，避免在等待期间继续参与移动和黏连逻辑
        foreach (var b in blocks)
        {
            if (b == null) continue;

            // 禁用 Sticky 行为：不改源码，只把组件关掉
            var sticky = b.GetComponent<Sticky>();
            if (sticky != null)
                sticky.enabled = false;

            // 禁止再被推动（Block 里的 canMove 是 public 的，允许改）
            b.canMove = false;
        }

        // 等待一小段时间，让玩家看到“连在一起了”
        yield return new WaitForSeconds(clearDelay);

        // 过滤已经被删掉的
        var valid = new List<MatchBlock>();
        foreach (var b in blocks)
        {
            if (b != null)
                valid.Add(b);
        }

        // 真正移除
        if (valid.Count > 0)
        {
            SfxManager.PlayVanish();
            RemoveBlocks(valid);
        }

        // 更新一次网格：此时 isClearing 仍为 true，所以本脚本的 GridChanged 不会再嵌套触发
        if (gridManager != null)
            gridManager.UpdateGrid();

        isClearing = false;

        // 删除完成后检查是否全清
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
        // 只看 ContainObj 本体上的 MatchBlock
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

            // 从所在 Cell 解绑
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

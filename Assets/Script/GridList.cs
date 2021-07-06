using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridList : ScrollRect
{
    private GridLayout temp1;
    private GridLayoutGroup temp2;

    private Vector2 cellSize;
    public Vector2 CellSize
    {
        get
        {
            return cellSize;
        }
        set
        {
            cellSize = value;
        }
    }

    private Vector2 spaceing;
    public Vector2 Spacing
    {
        get
        {
            return spaceing;
        }
        set
        {
            spaceing = value;
        }
    }

    public float top;
    public float bottom;
    public float left;
    public float right;

    public RectOffset padding;

    private int index_line = 0;
    private int index_side = 0;

    private float startLine = -1;       // 当前可视区域的第一行
    private float endLine = -1;         // 当前可视区域最后一行
    public int visibleLineCount_Vertical = 0;  // 可视行数
    public int visibleLineCount_Horizontal = 0;  // 可视行数
    public int itemCountInLine = 0;

    public Button addBtn_0;
    public Button addBtn_1;
    public Button addBtn_2;
    public Button removeBtn_0;
    public Button removeBtn_1;
    public Button removeBtn_2;

    public GridLayoutGroup.Corner startCorner;       // 开始点位，默认用左上角

    protected override void Awake()
    {
        base.Awake();
        this.onValueChanged.AddListener(OnValueChange);

        // test
        CellCount = 31;
        itemCountInLine = 3;
        visibleLineCount_Vertical = 7;
        visibleLineCount_Horizontal = itemCountInLine;
        padding = new RectOffset(20, 20, 10, 10);
        cellSize = new Vector2(50.0f, 50.0f);
        Spacing = new Vector2(5, 5);
        //startCorner = GridLayoutGroup.Corner.UpperLeft;
        // testEnd

        InitContentRt();
        CreateCell();
    }

    protected override void Start()
    {
        base.Start();
        OnValueChange(Vector2.zero);
    }

    private Vector2 anchorMin;
    private Vector2 anchorMax;
    private Vector2 pivot;
    private float horizontalPosDir = 1;
    private float verticalPosDir = -1;
    private float contentPosDir = 1;
    private float contentWidth = 0.0f;
    private float contentHeight = 0.0f;


    private void InitContentRt()
    {
        if (LineCount > 0)
        {
            contentWidth = padding.left + padding.right + cellSize.x * itemCountInLine + spaceing.x * (itemCountInLine - 1);
            contentHeight = padding.top + padding.bottom + cellSize.y * LineCount + spaceing.y * (LineCount - 1);
        }

        switch (startCorner)
        {
            case GridLayoutGroup.Corner.UpperRight:
                anchorMin = new Vector2(1, 1);
                anchorMax = new Vector2(1, 1);
                pivot = new Vector2(1, 1);
                horizontalPosDir = -1;
                verticalPosDir = -1;
                contentPosDir = 1;
                break;
            case GridLayoutGroup.Corner.LowerLeft:
                anchorMin = new Vector2(0, 0);
                anchorMax = new Vector2(0, 0);
                pivot = new Vector2(0, 0);
                horizontalPosDir = 1;
                verticalPosDir = 1;
                contentPosDir = vertical ? -1 : -1;
                break;
            case GridLayoutGroup.Corner.LowerRight:
                anchorMin = new Vector2(1, 0);
                anchorMax = new Vector2(1, 0);
                pivot = new Vector2(1, 0);
                horizontalPosDir = -1;
                verticalPosDir = 1;
                contentPosDir = vertical ? -1 : 1;
                break;
            // 默认左上角
            // case GridLayoutGroup.Corner.UpperLeft:
            default:
                anchorMin = new Vector2(0, 1);
                anchorMax = new Vector2(0, 1);
                pivot = new Vector2(0, 1);
                horizontalPosDir = 1;
                verticalPosDir = -1;
                contentPosDir = vertical ? 1 : -1;
                break;
        }

        content.anchorMin = anchorMin;
        content.anchorMax = anchorMax;
        content.pivot = pivot;

        if (vertical)
        {
            content.sizeDelta = new Vector2(contentWidth, contentHeight);
        }
        else
        {
            content.sizeDelta = new Vector2(contentHeight, contentWidth);
        }

        content.anchoredPosition = Vector2.zero;
    }

    // 总行数
    public int LineCount
    {
        get
        {
            if (itemCountInLine == 0 || cellCount == 0)
            {
                return 0;
            }

            return Mathf.CeilToInt((float)cellCount / itemCountInLine);
        }
    }

    // 总格子数
    private int cellCount;
    public int CellCount
    {
        get { return cellCount; }
        set
        {
            cellCount = value > 0 ? value : 0;
        }
    }

    // 缓存格子结构
    private class CellInfo
    {
        public int index;
        public RectTransform rt;
        public bool idel
        {
            get 
            { 
                return index == -1;
            }
            set
            {
                index = -1;
            }
        }

        public CellInfo(int index, RectTransform rt)
        {
            this.index = index;
            this.rt = rt;
            this.idel = true;
        }

        public void SetActive(bool isActive)
        {
            if (rt == null)
            {
                return;
            }

            rt.gameObject.SetActive(isActive);
        }

        public void SetPos(Vector2 pos)
        {
            if (rt == null)
            {
                return;
            }

            rt.anchoredPosition = pos;
        }
    }

    private LinkedList<CellInfo> cellList = new LinkedList<CellInfo>();
    private Stack<LinkedListNode<CellInfo>> freeCells = new Stack<LinkedListNode<CellInfo>>();
    private GameObject prefab;

    private void CreateCell()
    {
        prefab = null;
        for (int i = content.childCount - 1; i >= 0; --i)
        {
            if (i == 0)
            {
                prefab = content.GetChild(i).gameObject;
            }
            else
            {
                DestroyImmediate(content.GetChild(i).gameObject);
            }
        }

        prefab.SetActive(false);
    }

    private void FreeCell(LinkedListNode<CellInfo> node)
    {
        if (node == null)
        {
            return;
        }

        node.Value.idel = true;
        node.Value.SetActive(false);

        freeCells.Push(node);
    }

    private LinkedListNode<CellInfo> GetCell()
    {
        if (freeCells.Count == 0)
        {
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            go.transform.SetParent(content);
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
            RectTransform tempRt = go.transform as RectTransform;
            tempRt.anchorMin = anchorMin;
            tempRt.anchorMax = anchorMax;

            return new LinkedListNode<CellInfo>(new CellInfo(-1, tempRt));
        }

        return freeCells.Pop();
    }

    //protected void UpdateVisuableLines()
    //{
    //    Vector2 contentPos = this.content.anchoredPosition;
    //    int lineIndex = 0;
    //    var linePos = padding.top + cellSize[index_line] * lineIndex;
    //    if (lineIndex > 1)
    //    {
    //        linePos += Spacing[index_line] * (lineIndex - 1);
    //    }

    //    //float lineSize = zie
    //}

    private int GetLineIndex(float pos)
    {
        if (LineCount <= 0)
        {
            return -1;
        }

        if (LineCount <= 1)
        {
            return 0;
        }

        pos = Mathf.Abs(pos);

        switch (startCorner)
        {
            case GridLayoutGroup.Corner.UpperLeft:
                if (vertical)
                {
                    pos -= padding.top + cellSize.y + Spacing.y * 0.5f;
                }
                else
                {
                    pos -= padding.left + cellSize.x + Spacing.x * 0.5f;
                }
                break;
            case GridLayoutGroup.Corner.UpperRight:
                if (vertical)
                {
                    pos -= padding.top + cellSize.y + Spacing.y * 0.5f;
                }
                else
                {
                    pos -= padding.right + cellSize.x + Spacing.x * 0.5f;
                }
                break;
            case GridLayoutGroup.Corner.LowerLeft:
                if (vertical)
                {
                    pos -= padding.bottom + cellSize.y + Spacing.y * 0.5f;
                }
                else
                {
                    pos -= padding.left + cellSize.x + Spacing.x * 0.5f;
                }
                break;
            case GridLayoutGroup.Corner.LowerRight:
                if (vertical)
                {
                    pos -= padding.bottom + cellSize.y + Spacing.y * 0.5f;
                }
                else
                {
                    pos -= padding.right + cellSize.x + Spacing.x * 0.5f;
                }
                break;
        }

        pos = pos > 0 ? pos : 0;

        float singleSize = 0;
        if (vertical)
        {
            singleSize = cellSize.y + Spacing.y;
        }
        else
        {
            singleSize = cellSize.x + Spacing.x;
        }

        return Mathf.CeilToInt(pos / singleSize);
    }

    // Cell Pivot 为中心
    private Vector2 GetCellPos(int index)
    {
        float x, y;
        int lineIndex = Mathf.FloorToInt(index / itemCountInLine);
        int indexInline = index % itemCountInLine;

        Vector2 startPos = Vector2.zero;

        switch (startCorner)
        {
            case GridLayoutGroup.Corner.UpperLeft:
                startPos = new Vector2(padding.left, padding.top);
                break;
            case GridLayoutGroup.Corner.UpperRight:
                startPos = new Vector2(padding.right, padding.top);
                break;
            case GridLayoutGroup.Corner.LowerLeft:
                startPos = new Vector2(padding.left, padding.bottom);
                break;
            case GridLayoutGroup.Corner.LowerRight:
                startPos = new Vector2(padding.right, padding.bottom);
                break;
        }

        if (vertical)
        {
            x = startPos.x + cellSize.x * 0.5f + (cellSize.x + spaceing.x) * indexInline;
            y = startPos.y + cellSize.y * 0.5f + (cellSize.y + spaceing.y) * lineIndex;
        }
        else
        {
            x = startPos.x + cellSize.x * 0.5f + (cellSize.x + spaceing.x) * lineIndex;
            y = startPos.y + cellSize.y * 0.5f + (cellSize.y + spaceing.y) * indexInline;
        }
     
        x *= horizontalPosDir;
        y *= verticalPosDir;

        return new Vector2(x, y);
    }

    private void OnValueChange(Vector2 pos)
    {
        Vector2 curPos = content.anchoredPosition;
        float tempPos;
        tempPos = vertical ? curPos.y : curPos.x;

        // 如果为负数，说明异向
        if (tempPos * contentPosDir < 0)
        {
            tempPos = 0.0f;
        }

        int temp0 = GetLineIndex(tempPos);

        float viewSize = vertical ? viewRect.rect.height : viewRect.rect.width;
        int temp1 = GetLineIndex(tempPos + contentPosDir * viewSize);

        topLineIndexTest = Mathf.Min(temp0, temp1);
        btmLineIndexTest = Mathf.Max(temp0, temp1);

        lineCountTest = btmLineIndexTest - topLineIndexTest + 1;

        int exLineCount = visibleLineCount_Vertical - lineCountTest;
        exLineCount = exLineCount > 0 ? exLineCount : 0;

        // 如果不能均分，优先下方多一行
        int topEx = Mathf.FloorToInt(exLineCount * 0.5f);
        int btmEx = Mathf.CeilToInt(exLineCount * 0.5f);

        int topLineIndexTest_temp = topLineIndexTest - topEx;
        topLineIndexTest_temp = Mathf.Max(topLineIndexTest_temp, 0);

        int btmLineIndexTest_temp = topLineIndexTest_temp + visibleLineCount_Vertical - 1;
        btmLineIndexTest_temp = Mathf.Min(btmLineIndexTest_temp, LineCount - 1);

        // 回首掏
        topLineIndexTest_temp = btmLineIndexTest_temp - visibleLineCount_Vertical + 1;
        topLineIndexTest_temp = Mathf.Max(topLineIndexTest_temp, 0);

        UpdateView(topLineIndexTest_temp, btmLineIndexTest_temp);
    }

    private void UpdateView(int topLineIndexTest_new, int btmLineIndexTest_new)
    {
        bool change = false;
        if (topLineIndexTest_cache != topLineIndexTest_new || btmLineIndexTest_cache != btmLineIndexTest_new)
        {
            change = true;
        }

        if (!change)
        {
            return;
        }

        // recovery first
        int count = 0;
        int fromIndex = 0;
        int toIndex = -1;
        RectTransform tempRt;

        count = (topLineIndexTest_new - topLineIndexTest_cache) * itemCountInLine;
        if (count > 0)
        {
            fromIndex = topLineIndexTest_cache * itemCountInLine;
            toIndex = fromIndex + count - 1;
            toIndex = Mathf.Min(toIndex, CellCount - 1);

            for (int i = fromIndex; i <= toIndex; ++i)
            {
                if (cellList.Count <= 0)
                {
                    break;
                }

                FreeCell(cellList.First);
                cellList.RemoveFirst();
            }
        }

        count = (btmLineIndexTest_cache - btmLineIndexTest_new) * itemCountInLine;
        if (count > 0)
        {
            fromIndex = (btmLineIndexTest_new + 1) * itemCountInLine;
            toIndex = fromIndex + count - 1;
            toIndex = Mathf.Min(toIndex, CellCount - 1);

            for (int i = fromIndex; i <= toIndex; ++i)
            {
                if (cellList.Count <= 0)
                {
                    break;
                }

                FreeCell(cellList.Last);
                cellList.RemoveLast();
            }
        }

        // Show new cellItem
        count = (topLineIndexTest_new - topLineIndexTest_cache) * itemCountInLine;
        if (count < 0)
        {
            count = -count;
            fromIndex = topLineIndexTest_new * itemCountInLine;
            toIndex = fromIndex + count - 1;
            toIndex = Mathf.Min(toIndex, CellCount - 1);

            for (int i = toIndex; i >= fromIndex; --i)
            {
                LinkedListNode<CellInfo> newCell = GetCell();
                newCell.Value.index = i;
                tempRt = newCell.Value.rt;
                tempRt.gameObject.SetActive(true);
                tempRt.anchoredPosition = GetCellPos(i);
                ShowCellIndex(tempRt, i);
                cellList.AddFirst(newCell);
            }
        }

        count = (btmLineIndexTest_cache - btmLineIndexTest_new) * itemCountInLine;
        if (count < 0)
        {
            count = -count;
            fromIndex = (btmLineIndexTest_cache + 1) * itemCountInLine;
            toIndex = fromIndex + count - 1;
            toIndex = Mathf.Min(toIndex, CellCount - 1);

            for (int i = fromIndex; i <= toIndex; ++i)
            {
                LinkedListNode<CellInfo> newCell = GetCell();
                newCell.Value.index = i;
                tempRt = newCell.Value.rt;
                tempRt.gameObject.SetActive(true);
                tempRt.anchoredPosition = GetCellPos(i);
                ShowCellIndex(tempRt, i);
                cellList.AddLast(newCell);
            }
        }

        topLineIndexTest_cache = topLineIndexTest_new;
        btmLineIndexTest_cache = btmLineIndexTest_new;
    }

    private void ShowCellIndex(RectTransform cell, int index)
    {
        cell.GetComponentInChildren<Text>().text = index.ToString();
    }

    private int topLineIndexTest = 0;
    private int btmLineIndexTest = 0;
    private int lineCountTest = 0;

    private int topLineIndexTest_cache = 0;
    private int btmLineIndexTest_cache = -1;

    public override void OnScroll(PointerEventData data)
    {
        base.OnScroll(data);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    private void OnGUI()
    {
        GUILayout.Label("topLineIndexTest: " + topLineIndexTest);
        GUILayout.Label("btmLineIndexTest: " + btmLineIndexTest);
        GUILayout.Label("topLineIndexTest_cache: " + topLineIndexTest_cache);
        GUILayout.Label("btmLineIndexTest_cache: " + btmLineIndexTest_cache);
        GUILayout.Label("lineCountTest: " + lineCountTest);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //this.removeBtn_0.onClick.RemoveAllListeners();
        //this.removeBtn_1.onClick.RemoveAllListeners();
        //this.removeBtn_2.onClick.RemoveAllListeners();
    }
}

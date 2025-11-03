using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class ScrollableAreaController : MonoBehaviour
{
    enum INITSTATE {
        TOP,
        LEFT,
        CENTER
    }
    [SerializeField]
    private INITSTATE initStae;
    [SerializeField] private ScrollableCell cellPrefab; 
    private RectTransform content;
    [SerializeField] private int NUMBER_OF_COLUMNS = 1;  //表示并排显示几个，比如是上下滑动，当此处为2时表示一排有两个cell
    [SerializeField] private float cellWidth = 30.0f;
    [SerializeField] private float cellHeight = 25.0f;
    private int visibleCellsTotalCount = 0;
    private int visibleCellsRowCount = 0;
    private LinkedList<GameObject> localCellsPool = new LinkedList<GameObject>();
    private LinkedList<GameObject> cellsInUse = new LinkedList<GameObject>();
    private ScrollRect rect;

    public IList allCellsData ;
    private int previousInitialIndex = 0;
    private int initialIndex = 0;
    private float initpostion=0;
    private float adjustSize;
    private Vector3 contentPostion;

    public void Awake()
    {
        rect = this.GetComponent<ScrollRect>();
        content = rect.content;

        if(horizontal && !(initStae == INITSTATE.LEFT || initStae == INITSTATE.CENTER)) {
            initStae = INITSTATE.LEFT;
        } else if(!horizontal && !(initStae == INITSTATE.TOP || initStae == INITSTATE.CENTER)) {
            initStae = INITSTATE.TOP;
        }
        switch(initStae) {
            case INITSTATE.TOP:
                content.anchorMin = new Vector2 (0.5f , 1);
                content.anchorMax = new Vector2 (0.5f , 1);
                content.pivot = new Vector2 (0.5f , 1);
                content.localPosition = new Vector2 (0 , 0);
                break;
            case INITSTATE.LEFT:
                content.anchorMin = new Vector2 (0 , 0.5f);
                content.anchorMax = new Vector2 (0 , 0.5f);
                content.pivot = new Vector2 (0 , 0.5f);
                content.localPosition = new Vector2 (0 , 0);
                break;
            case INITSTATE.CENTER:
                content.anchorMin = new Vector2 (0.5f , 0.5f);
                content.anchorMax = new Vector2 (0.5f , 0.5f);
                content.pivot = new Vector2 (0.5f , 0.5f);
                content.localPosition = new Vector2 (0 , 0);
                break;

        }

        if (NUMBER_OF_COLUMNS <= 0)
            NUMBER_OF_COLUMNS = 1;

        if (horizontal)
            visibleCellsRowCount = Mathf.CeilToInt(rect.viewport.GetComponent<RectTransform>().sizeDelta.x / cellWidth);
        else
            visibleCellsRowCount = Mathf.CeilToInt(rect.viewport.GetComponent<RectTransform>().sizeDelta.y / cellHeight);

        visibleCellsTotalCount = visibleCellsRowCount + 1;
        visibleCellsTotalCount *= NUMBER_OF_COLUMNS;
        contentPostion = content.localPosition;
        this.CreateCellPool();
    }

    public void Update()
    {
        if (allCellsData==null)
            return;
        previousInitialIndex = initialIndex;
        CalculateCurrentIndex();
        InternalCellsUpdate();
    }

    private void InternalCellsUpdate(){
        if (previousInitialIndex != initialIndex)
        {
            bool scrollingPositive = previousInitialIndex < initialIndex;
            int indexDelta = Mathf.Abs(previousInitialIndex - initialIndex);

            int deltaSign = scrollingPositive ? +1 : -1;
            
            for (int i = 1; i <= indexDelta; i++)
                this.UpdateContent(previousInitialIndex + i * deltaSign, scrollingPositive);
        }
    }

    public void setCellsData (System.Object data ,int index = -1) {
        if (allCellsData == null) {
            StartCoroutine(delaySetCellData(data , index));
        }else if (index == -1) {
            index = allCellsData.IndexOf(data);
        }
        if (index < 0) {
            return;
        }
        foreach(GameObject go in cellsInUse) {
            ScrollableCell scrollableCell = go.GetComponent<ScrollableCell> ();
            if(scrollableCell.DataIndex == index) {
                scrollableCell.DataObject = data;
            }
        }
    }

    IEnumerator delaySetCellData(System.Object data, int index) {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        setCellsData(data , index);
    }

    private void CalculateCurrentIndex() {
        if (!horizontal)
            initialIndex = Mathf.FloorToInt((content.localPosition.y - initpostion) / cellHeight);
        else {
            if(initStae == INITSTATE.LEFT)
                initialIndex = (int)((content.localPosition.x - initpostion) / cellWidth);
            else
                initialIndex = (int)((content.localPosition.x - initpostion) / cellWidth);
            initialIndex = Mathf.Abs (initialIndex);
        }
        int limit = Mathf.CeilToInt((float)allCellsData.Count / (float)NUMBER_OF_COLUMNS) - visibleCellsRowCount;
        if (initialIndex < 0)
            initialIndex = 0;
        if (initialIndex >= limit)
            initialIndex = limit - 1;
    }

    private bool horizontal {
        get { return rect.horizontal; }
    }

    private void FreeCell(bool scrollingPositive)
    {
        LinkedListNode<GameObject> cell = null;
        // Add this GameObject to the end of the list
        if (scrollingPositive)
        {
            cell = cellsInUse.First;
            cellsInUse.RemoveFirst();
            localCellsPool.AddLast(cell);
        }
        else
        {
            cell = cellsInUse.Last;
            cellsInUse.RemoveLast();
            localCellsPool.AddFirst(cell);
        }
    }

    private void UpdateContent(int cellIndex, bool scrollingPositive)
    {
        int index = scrollingPositive ? ((cellIndex - 1) * NUMBER_OF_COLUMNS) + (visibleCellsTotalCount) : (cellIndex * NUMBER_OF_COLUMNS);
        LinkedListNode<GameObject> tempCell = null;

        int currentDataIndex = 0;
        for (int i = 0; i < NUMBER_OF_COLUMNS; i++) {
            this.FreeCell(scrollingPositive);
            tempCell = GetCellFromPool(scrollingPositive);
            currentDataIndex = index + i;

            PositionCell(tempCell.Value, index + i);
            ScrollableCell scrollableCell = tempCell.Value.GetComponent<ScrollableCell>();
            if (currentDataIndex >= 0 && currentDataIndex < allCellsData.Count)
            {
                scrollableCell.Init(this, allCellsData[currentDataIndex], currentDataIndex);
            }
            else
                scrollableCell.Init(this, null, currentDataIndex);

            scrollableCell.ConfigureCell();
        }
    }


    public void InitializeWithData(IList cellDataList)
    {
        content.localPosition = Vector3.zero;    
        StartCoroutine (initData(cellDataList));
    }

    IEnumerator initData (IList cellDataList) {
        yield return new WaitForSeconds(0.1f);
        if(cellsInUse.Count > 0) {
            foreach(var cell in cellsInUse) {
                localCellsPool.AddLast (cell);
            }
            cellsInUse.Clear ();
        }

        previousInitialIndex = 0;
        initialIndex = 0;
        content.gameObject.SetActive (true);
        LinkedListNode<GameObject> tempCell = null;
        allCellsData = cellDataList;      

        if(horizontal) {
            content.sizeDelta = new Vector2 (allCellsData.Count * cellWidth / NUMBER_OF_COLUMNS , content.sizeDelta.y);
            if(initStae == INITSTATE.CENTER) {
                adjustSize = (content.sizeDelta.x / 2 - rect.viewport.sizeDelta.x / 2);
                content.localPosition = contentPostion + new Vector3(adjustSize, 0, 0);
            }
            initpostion = content.localPosition.x;
        } else {
            content.sizeDelta = new Vector2 (content.sizeDelta.x , allCellsData.Count * cellHeight / NUMBER_OF_COLUMNS);
            if(initStae == INITSTATE.CENTER) {
                adjustSize = (content.sizeDelta.y / 2 - rect.viewport.sizeDelta.y / 2);
                content.localPosition = contentPostion + new Vector3 (0 , -adjustSize , 0);
            }
            initpostion = content.localPosition.y;
        }

        int currentDataIndex = 0;
        for(int i = 0; i < visibleCellsTotalCount; i++) {

            tempCell = GetCellFromPool (true);
            if(tempCell == null || tempCell.Value == null)
                continue;
            currentDataIndex = i + initialIndex * NUMBER_OF_COLUMNS;

            PositionCell (tempCell.Value , currentDataIndex);
            tempCell.Value.SetActive (true);
            ScrollableCell scrollableCell = tempCell.Value.GetComponent<ScrollableCell> ();
            if(currentDataIndex < cellDataList.Count)
                scrollableCell.Init (this , cellDataList[i] , currentDataIndex);
            else
                scrollableCell.Init (this , null , currentDataIndex);
            scrollableCell.ConfigureCell ();
        }
    }
    private void PositionCell(GameObject go,int index)
    {
        int rowMod = index % NUMBER_OF_COLUMNS;
        if(!horizontal)
            go.transform.localPosition = FirstCellPosition + new Vector3 (cellWidth * (rowMod) , -(index / NUMBER_OF_COLUMNS) * cellHeight , 0);
        else {
            if(initStae == INITSTATE.LEFT)
                go.transform.localPosition = FirstCellPosition + new Vector3 ((index / NUMBER_OF_COLUMNS) * cellWidth , -cellHeight * (rowMod) , 0);
            else
                go.transform.localPosition = FirstCellPosition + new Vector3 ((index / NUMBER_OF_COLUMNS) * cellWidth , -cellHeight * (rowMod) , 0);
        }
            
    }

    private Vector3 FirstCellPosition{

        get {
            if(!horizontal) {
                if(initStae == INITSTATE.CENTER) {
                    return new Vector3(0, content.sizeDelta.y / 2 - cellHeight / 2, 0); 
                } else {
                    return new Vector3 (0 , -cellHeight / 2 + 1 , 0);
                }
            }else {
                if(initStae == INITSTATE.LEFT)
                    return new Vector3 (cellWidth / 2 , 0 , 0);
                else
                    return new Vector3 (-content.sizeDelta.x / 2 + cellWidth / 2 , 0 , 0);
            }
                
        }
    }

    private void CreateCellPool()
    {
        GameObject tempCell = null;
        for (int i = 0; i < visibleCellsTotalCount; i++)
        {
            tempCell = this.InstantiateCell();
            localCellsPool.AddLast(tempCell);
        }
        content.gameObject.SetActive(false);
    }

    private GameObject InstantiateCell()
    {
        GameObject cellTempObject = Instantiate(cellPrefab.gameObject) as GameObject;
        cellTempObject.layer = this.gameObject.layer;
        cellTempObject.transform.SetParent(content.transform);
        cellTempObject.transform.localScale = cellPrefab.transform.localScale;
        cellTempObject.transform.localPosition = cellPrefab.transform.localPosition;
        cellTempObject.transform.localRotation = cellPrefab.transform.localRotation;
        cellTempObject.SetActive(false);
        return cellTempObject;
    }

    private LinkedListNode<GameObject> GetCellFromPool(bool scrollingPositive)
    {
        if (localCellsPool.Count == 0)
            return null;

        LinkedListNode<GameObject> cell = localCellsPool.First;
        localCellsPool.RemoveFirst();

        if (scrollingPositive)
            cellsInUse.AddLast(cell);
        else
            cellsInUse.AddFirst(cell);
        return cell;
    }

}

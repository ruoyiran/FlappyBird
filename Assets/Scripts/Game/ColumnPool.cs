using System;
using UnityEngine;

public class ColumnPool : MonoBehaviour {
    public GameObject firstColumnObj;
    public float spaceX = 4.5f;
    public float columnMinY = -4.5f;
    public float columnMaxY = -1.5f;
    public int maxColumns = 6;
    private GameObject[] columnObjs;
    private float halfOfViewWidth;
    private float firstColumnObjPosX;

    private void Start()
    {
        halfOfViewWidth = Camera.main.orthographicSize * Screen.width / (float)Screen.height;
        firstColumnObjPosX = firstColumnObj.transform.position.x;
        InitColumnObjs();
    }

    void Update ()
    {
        if (GameManager.Instance.CurrentGameState == GameState.GameOver)
            return;
        if (columnObjs[0].transform.position.x < -halfOfViewWidth-0.5f)
        {
            var lastColumnPos = columnObjs[maxColumns - 1].transform.position;
            columnObjs[0].transform.position = new Vector3(lastColumnPos.x + spaceX, GetRandomY(), 0);
            ShiftMove();
        }
	}

    private void InitColumnObjs()
    {
        if (firstColumnObj == null)
            return;
        columnObjs = new GameObject[maxColumns];
        columnObjs[0] = firstColumnObj;
        for (int i = 1; i < maxColumns; i++)
        {
            GameObject columnObj = Instantiate(firstColumnObj, Vector3.zero, Quaternion.identity, transform);
            columnObjs[i] = columnObj;
        }
        InitColumnObjsPos();
    }

    private void InitColumnObjsPos()
    {
        float startX = firstColumnObjPosX;
        for (int i = 0; i < maxColumns; i++)
        {
            columnObjs[i].transform.position = new Vector3(startX, GetRandomY(), 0);
            startX += spaceX;
        }
    }

    private float GetRandomY()
    {
        float randomY = UnityEngine.Random.Range(columnMinY, columnMaxY);
        return randomY;
    }

    private void ShiftMove()
    {
        var t = columnObjs[0];
        for (int i = 0; i < maxColumns - 1; i++)
        {
            columnObjs[i] = columnObjs[i + 1];
        }
        columnObjs[maxColumns - 1] = t;
    }

    public void ResetPos()
    {
        InitColumnObjsPos();
    }
}

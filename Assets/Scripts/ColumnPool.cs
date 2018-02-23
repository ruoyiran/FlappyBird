using UnityEngine;

public class ColumnPool : MonoBehaviour {
    public GameObject firstColumnObj;
    public float spaceX = 4.5f;
    public float columnMinY = -4.5f;
    public float columnMaxY = -1.5f;
    public int maxColumns = 6;
    private GameObject[] columnObjs;
    private float halfOfViewWidth;

    private void Start()
    {
        halfOfViewWidth = Camera.main.orthographicSize * Screen.width / (float)Screen.height;
        InitColumnObjs();
    }

    void Update ()
    {
	    if(!GameManager.Instance.IsGameOver && columnObjs[0].transform.position.x < -halfOfViewWidth-0.5f)
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
        Vector3 columnPos = firstColumnObj.transform.position;
        firstColumnObj.transform.position = new Vector3(columnPos.x, GetRandomY(), 0);
        for (int i = 1; i < maxColumns; i++)
        {
            GameObject columnObj = Instantiate(firstColumnObj, Vector3.zero, Quaternion.identity, transform);
            columnPos += new Vector3(spaceX, 0, 0);
            columnObj.transform.position = new Vector3(columnPos.x, GetRandomY(), 0);
            columnObjs[i] = columnObj;
        }
    }

    private float GetRandomY()
    {
        float randomY = Random.Range(columnMinY, columnMaxY);
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
}

using UnityEngine;

namespace FlappyBird
{
    public class ColumnPool : MonoBehaviour
    {
        public GameObject firstColumnObj;
        private const float MAX_COLUMN_GAP_Y = 1.5f;
        private float spaceX = 5f;
        private float columnMinY = -4.5f;
        private float columnMaxY = -1.5f;
        private int maxColumns = 6;
        private GameObject[] columnObjs;
        private float halfOfViewWidth;
        private float firstColumnObjPosX;
        private float _prevColumnY;

        private void Start()
        {
            halfOfViewWidth = Camera.main.orthographicSize * Screen.width / (float)Screen.height;
            firstColumnObjPosX = firstColumnObj.transform.position.x;
            InitColumnObjs();
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing)
                return;
            if (columnObjs[0].transform.position.x < -halfOfViewWidth - 0.5f)
            {
                var lastColumnPos = columnObjs[maxColumns - 1].transform.position;
                columnObjs[0].transform.position = new Vector3(lastColumnPos.x + spaceX, GetRandomY(), 0);
                ShiftMove();
            }
        }

        public void ScrollAllColumns(float scrollSpeed)
        {
            for (int i = 0; i < columnObjs.Length; i++)
            {
                var rb = columnObjs[i].GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = new Vector2(scrollSpeed, 0);
            }
        }

        public void StopScrollAllColumns()
        {
            for (int i = 0; i < columnObjs.Length; i++)
            {
                var rb = columnObjs[i].GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = Vector2.zero;
            }
        }

        private void InitColumnObjs()
        {
            if (firstColumnObj == null)
                return;
            columnObjs = new GameObject[maxColumns];
            columnObjs[0] = firstColumnObj;
            _prevColumnY = columnMaxY;
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
            float randomY = Random.Range(columnMinY, columnMaxY);
            float gapY = Mathf.Abs(_prevColumnY - randomY);
            if (gapY > MAX_COLUMN_GAP_Y)
            {
                gapY = MAX_COLUMN_GAP_Y;
                if (randomY < _prevColumnY)
                    randomY += gapY;
                else
                    randomY -= gapY;
                if (randomY < columnMinY)
                    randomY = columnMinY;
                else if (randomY > columnMaxY)
                    randomY = columnMaxY;
                _prevColumnY = randomY;
            }
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
}
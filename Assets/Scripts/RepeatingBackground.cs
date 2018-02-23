using UnityEngine;

public class RepeatingBackground : MonoBehaviour {
    private BoxCollider2D _boxCollider2d;
    private float _backgroundHorizontalLength;

	void Start () {
        _boxCollider2d = GetComponent<BoxCollider2D>();
        _backgroundHorizontalLength = _boxCollider2d.size.x;
    }
	
	void Update () {
        if (transform.position.x < -_backgroundHorizontalLength)
            RepositionBackground();
    }

    private void RepositionBackground()
    {
        Vector3 groundOffset = new Vector3(_backgroundHorizontalLength * 2.0f, 0);
        transform.position += groundOffset;
    }
}

using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    [SerializeField] float tileSizeY;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        ScrollBackground();
    }

    private void ScrollBackground()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeY);
        transform.position = startPosition + Vector3.down * newPosition;
    }
}

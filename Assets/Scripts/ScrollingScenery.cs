using UnityEngine;

public class ScrollingScenery : MonoBehaviour
{
    public Vector3 direction = new Vector3(0f, 0f, -1f);
    public float speed = 15f;
    public float loopDistance = 20f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector3 dir = direction.normalized;
        transform.position += dir * speed * Time.deltaTime;

        float moved = Vector3.Dot(transform.position - startPosition, dir);
        if (moved >= loopDistance)
        {
            transform.position -= dir * loopDistance;
        }
    }
}
using UnityEngine;

public class TrainShake : MonoBehaviour
{
    public float amount = 0.03f;
    public float speed = 6f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float t = Time.time * speed;
        float x = (Mathf.PerlinNoise(t, 0f) - 0.5f) * amount;
        float y = (Mathf.PerlinNoise(0f, t) - 0.5f) * amount;
        transform.localPosition = startPosition + new Vector3(x, y, 0f);
    }
}
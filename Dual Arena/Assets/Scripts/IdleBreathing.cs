using UnityEngine;

public class IdleBreathing : MonoBehaviour
{
    public float speed = 2f;
    public float scaleAmount = 0.05f;
    public float floatAmount = 0.1f;

    Vector3 originalScale;
    Vector3 originalPos;

    void Start()
    {
        originalScale = transform.localScale;
        originalPos = transform.position;
    }

    void Update()
    {
        float sin = Mathf.Sin(Time.time * speed);

        // breathing scale
        transform.localScale = originalScale * (1 + sin * scaleAmount);

        // floating up/down
        transform.position = originalPos + new Vector3(0, sin * floatAmount, 0);
    }
}
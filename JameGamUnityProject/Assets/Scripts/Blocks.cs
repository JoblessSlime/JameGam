using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public float moveDistance = 1f; 
    public float moveSpeed = 2f; 
    public bool startMovingUp = true; 

    private Vector3 initialPosition; 
    private int direction = 1;

    void Start()
    {
        initialPosition = transform.position;

        direction = startMovingUp ? 1 : -1;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2) - moveDistance;
        Vector3 newPosition = initialPosition + Vector3.up * offset;

        transform.position = newPosition;
    }
}

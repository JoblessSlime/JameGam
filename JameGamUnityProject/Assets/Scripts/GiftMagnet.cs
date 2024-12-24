using UnityEngine;

public class GiftMagnet : MonoBehaviour
{
    public float pullSpeed = 5f;
    public Transform magnet;
    public Transform spawnPoint;
    public GameObject giftPrefab;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if the object is tagged "Gift"
        if (other.CompareTag("Gift"))
        {
            // Get the current position of the gift and the magnet
            Vector3 giftPosition = other.transform.position;
            Vector3 magnetPosition = magnet.position;

            // Move the gift horizontally or vertically based on the triggering zone
            if (gameObject.CompareTag("HorizontalBox"))
            {
                // Move only in the horizontal direction
                float step = pullSpeed * Time.deltaTime;
                giftPosition.x = Mathf.MoveTowards(giftPosition.x, magnetPosition.x, step);
            }
            else if (gameObject.CompareTag("VerticalBox"))
            {
                // Move only in the vertical direction
                float step = pullSpeed * Time.deltaTime;
                giftPosition.y = Mathf.MoveTowards(giftPosition.y, magnetPosition.y, step);
            }

            // Apply the updated position to the gift
            other.transform.position = giftPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object is tagged "Gift"
        if (other.CompareTag("Gift"))
        {
            // Destroy the gift
            Destroy(other.gameObject);

            // Spawn a new gift at the spawn point
            if (spawnPoint != null && giftPrefab != null)
            {
                Instantiate(giftPrefab, spawnPoint.position, Quaternion.identity);
            }   
        }
    }
}
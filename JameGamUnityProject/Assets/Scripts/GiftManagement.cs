using UnityEngine;

public class GiftManagement : MonoBehaviour
{
    public Transform spawnPoint; // The point where the player carries the gift
    public GameObject giftPrefab; // Prefab of the gift
    private GameObject carriedGift; // The currently carried gift
    public Transform dropPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Replace with your preferred input
        {
            DropGift();
        }
    }

    // Function to make the player carry the gift
    public void CarryGift()
    {
        if (carriedGift == null)
        {
            // Instantiate a new gift and make it a child of the spawn point
            carriedGift = Instantiate(giftPrefab, spawnPoint.position, Quaternion.identity);
            carriedGift.transform.SetParent(spawnPoint);
        }
    }

    // Function to drop the carried gift
    public void DropGift()
    {
        if (carriedGift == null)
        {
            Debug.LogWarning("No gift to drop!");
            return;
        }

        // Detach the gift from the spawnPoint
        carriedGift.transform.SetParent(null);

        // Move the gift to the dropPoint
        carriedGift.transform.position = dropPoint.position;

        // Optional: Add Rigidbody2D for gravity
        Rigidbody2D rb = carriedGift.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Enable physics
        }

        // Clear the reference
        carriedGift = null;
    }
}
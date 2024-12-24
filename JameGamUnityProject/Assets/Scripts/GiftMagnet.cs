using UnityEngine;

public class GiftMagnet : MonoBehaviour
{
    public float pullSpeed = 5f;
    public Transform magnet;
    public Transform spawnPoint;
    public GameObject giftPrefab;
    
    

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Gift"))
        {
            Vector3 giftPosition = other.transform.position;
            Vector3 magnetPosition = magnet.position;

            if (gameObject.CompareTag("HorizontalBox"))
            {
                float step = pullSpeed * Time.deltaTime;
                giftPosition.x = Mathf.MoveTowards(giftPosition.x, magnetPosition.x, step);
            }
            else if (gameObject.CompareTag("VerticalBox"))
            {
                float step = pullSpeed * Time.deltaTime;
                giftPosition.y = Mathf.MoveTowards(giftPosition.y, magnetPosition.y, step);
            }

            other.transform.position = giftPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gift"))
        {
            Destroy(other.gameObject);

            if (spawnPoint != null && giftPrefab != null)
            {
                GameObject instance = Instantiate(giftPrefab);

                instance.transform.SetParent(spawnPoint, false);
                
                instance.transform.localPosition = Vector3.zero; 
                instance.transform.localRotation = Quaternion.identity;     
                instance.transform.localScale = Vector3.one;

            }
        }
    }
}
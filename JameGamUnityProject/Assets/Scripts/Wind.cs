using UnityEngine;

public class ConstantWindZone : MonoBehaviour
{
    //public ParticleSystem windVFX; // Reference to the wind VFX
    public float windForce = 10f; // Force of the wind
    public Vector2 windDirection = Vector2.right; // Direction of the wind (right or left)

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                Debug.Log("force activated  ");
                Vector2 force = windDirection.normalized * windForce;
                playerRigidbody.AddForce(force);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ) //&& windVFX != null
        {
            Debug.Log("is playing vfx");
            //windVFX.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") ) //&& windVFX != null
        {
            Debug.Log("is not playing vfx anymore");
            //windVFX.Stop();
        }
    }
}
using UnityEngine;

public class ConstantWindZone : MonoBehaviour
{
    //public ParticleSystem windVFX; // Reference to the wind VFX
    public Vector2 windDirection = Vector2.right; // Direction of the wind (right or left)
    public float minWindForce = 5f; // Minimum wind force
    public float maxWindForce = 15f; // Maximum wind force

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                // Generate a random force magnitude within the range
                float randomForce = Random.Range(minWindForce, maxWindForce);

                // Apply the random force
                Vector2 force = windDirection.normalized * randomForce;
                playerRigidbody.AddForce(force);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ) //&& windVFX != null
        {
            //windVFX.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") ) //&& windVFX != null
        {
            //windVFX.Stop();
        }
    }
}
using UnityEngine;

public class ConstantWindZone : MonoBehaviour
{
    //public ParticleSystem windVFX;
    public float windForce = 10f; 
    public Vector2 windDirection = Vector2.right; 

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                Vector2 force = windDirection.normalized * windForce;
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
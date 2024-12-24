using UnityEngine;

public class SlidingOnIce : MonoBehaviour
{
    public string TagName = "Glace";
    [SerializeField] Smooth2DMovements smooth2DMovements;
    [Range(0.25f, 25f)] public float groundAcceleration_OnIce;
    [Range(0.25f, 25f)] public float groundDeceleration_OnIce;
    public float maxSlidingSpeed;

    private float groundAcceleration_OffIce;
    private float groundDeceleration_OffIce;
    private float maxWalkSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundAcceleration_OffIce = smooth2DMovements.groundAcceleration;
        groundDeceleration_OffIce = smooth2DMovements.groundDeceleration;
        maxWalkSpeed = smooth2DMovements.maxWalkSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagName))
        {
            smooth2DMovements.groundDeceleration = groundDeceleration_OnIce;
            smooth2DMovements.groundAcceleration = groundAcceleration_OnIce;
            smooth2DMovements.maxWalkSpeed = maxSlidingSpeed;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagName))
        {
            smooth2DMovements.groundDeceleration = groundDeceleration_OffIce;
            smooth2DMovements.groundAcceleration = groundAcceleration_OffIce;
            smooth2DMovements.maxWalkSpeed = maxWalkSpeed;
        }
    }
}

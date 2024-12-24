using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAndRespawn : MonoBehaviour
{
    public bool useCheckpointMethod;

    [Header("Death")]
    public string killZoneTag;
    public GameObject player;

    [Header("CheckpointMethod")]
    public string checkpointTag;
    private Transform lastCheckpoint;

    [Header("TimeMethod")]
    public float revertTimeWhenDying = 3.0f;
    public float timeBetweenUpdatePos = 0.5f;
    [SerializeField] Smooth2DMovements smooth2DMovements;
    private List<Vector3> oldPos = new List<Vector3>();
    private int listIndex = -1;
    private float time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (float i = 0f; i < revertTimeWhenDying; i += timeBetweenUpdatePos)
        {
            oldPos.Add(player.transform.position);
            listIndex++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!useCheckpointMethod)
        {
            time += Time.deltaTime;
            if (time >= timeBetweenUpdatePos)
            {
                if (smooth2DMovements.isGrounded)
                {
                    Debug.Log("worked perfectly");
                    oldPos.Remove(oldPos[0]);
                    Vector3 thisPosition = player.transform.position;
                    oldPos.Add(thisPosition);
                    //ChangeIndex();
                    time = 0f;
                }
            }
        }
    }

    private void CheckpointMethod()
    {
        player.transform.position = lastCheckpoint.position;
        Debug.Log("Done");
        Debug.Log(lastCheckpoint.position);
    }

    private void TimeMethod()
    {
        player.transform.position = oldPos[0];
        Debug.Log("Done");
        for (int i = 1; i < listIndex; i++)
        {
            Debug.Log(oldPos[i]);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(killZoneTag)) 
        {
            if (useCheckpointMethod)
            {
                CheckpointMethod();
                return;
            }
            else
            {
                TimeMethod();
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(checkpointTag))
        {
            lastCheckpoint = collision.gameObject.GetComponent<Transform>();
        }
    }
}

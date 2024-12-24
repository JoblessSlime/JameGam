using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    public GameObject player;
    public GameObject KdoPrefab;
    public Transform playerStartPos;

    [Header("UI")]
    public GameObject WinWindow;
    public GameObject LooseWindow;
    public TextMeshProUGUI timerInGame;
    public TextMeshProUGUI timerWinWindow;

    public int timer;

    private float time = 0f;
    private bool endGame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WinWindow.SetActive(false);
        LooseWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (!endGame)
            timerInGame.text = ((int) (timer - time)).ToString();
        if ( time >= timer)
        {
            LooseWindow.SetActive (true);
            timerInGame.text = "";
            endGame = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.CompareTag("Gift"))
        {
            WinWindow.SetActive(true);
            endGame = true;
            timerInGame.text = "";
            timerWinWindow.text = ((int)time).ToString();
            player.SetActive(false);
        }
    }

    public void restart()
    {
        /*
        time = 0f;
        WinWindow.SetActive(false);
        LooseWindow.SetActive(false);
        endGame = false;
        player.transform.position = playerStartPos.position;
        KdoPrefab.transform.position = playerStartPos.position;
        */
        SceneManager.LoadScene("Scene Salim");
    }
}

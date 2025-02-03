using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    public GameObject player;

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
        Debug.Log("end game is " + endGame);
        if (!endGame)
        {
            timerInGame.text = ((int) (timer - time)).ToString();
            if (time >= timer)
            {
                LooseWindow.SetActive(true);
                timerInGame.text = "";
                endGame = true;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if (!endGame)
        {
            if (collision.gameObject.CompareTag("Gift"))
            {
                WinWindow.SetActive(true);
                endGame = true;
                timerInGame.text = "";
                timerWinWindow.text = ((int)time).ToString();
                player.SetActive(false);
            }
        }
    }

    public void restart()
    {
        SceneManager.LoadScene("Scene Salim");
    }
}

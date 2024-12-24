using TMPro;
using UnityEngine;

public class WinGame : MonoBehaviour
{
    public string WinZoneTag;
    public GameObject player;
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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(WinZoneTag))
        {
            WinWindow.SetActive(true);
            endGame = true;
            timerInGame.text = "";
            timerWinWindow.text = ((int) time).ToString();
        }
    }

    public void restart()
    {
        time = 0f;
        WinWindow.SetActive(false);
        LooseWindow.SetActive(false);
        endGame = false;
        player.transform.position = playerStartPos.position;
        this.gameObject.transform.position = playerStartPos.position;
    }
}

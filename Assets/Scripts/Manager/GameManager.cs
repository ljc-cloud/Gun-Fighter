using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text LevelTarget;
    public TMP_Text LevelText;
    public Panel LosePanel;
    public Panel WinPanel;
    public Panel EscPanel;

    public Transform PlayerTransform;

    private float fadeTimer;

    private bool escShow;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            LosePanel.Hide();
            WinPanel.Hide();
        }
        EscPanel.Hide();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
            return;
        Level.Instance.OnTargetComplete += GameManager_OnTargetComplete;
        PlayerTransform.GetComponent<Health>().OnPlayerDie += GameManager_OnPlayerDie;
    }

    private void GameManager_OnTargetComplete()
    {
        ProcessWin();
    }
    private void GameManager_OnPlayerDie()
    {
        ProcessLose();
    }

    private void ProcessWin()
    {
        Cursor.lockState = CursorLockMode.Confined;
        WinPanel.Show();
        PlayerTransform.GetComponent<PlayerMovement>().enabled = false;
        PlayerTransform.GetComponent<MouseLook>().enabled = false;
        PlayerTransform.GetComponent<WeaponControl>().enabled = false;
        PlayerTransform.GetComponentInChildren<WeaponAbstract>().enabled = false;

    }
    private void ProcessLose()
    {
        Cursor.lockState = CursorLockMode.Confined;
        LosePanel.Show();
        Camera.main.transform.SetParent(null);
        Camera.main.transform.position = PlayerTransform.position;
        foreach (var item in Camera.main.transform.GetComponentsInChildren<Transform>())
        {
            if (!item.CompareTag("MainCamera"))
                Destroy(item.gameObject);
        }
    }

    private void Update()
    {
        TimerTick();
        UIFade();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escShow)
            {
                Cursor.lockState = CursorLockMode.Locked;
                EscPanel.Hide();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                EscPanel.Show();
            }
            escShow = !escShow;
        }
    }

    private void TimerTick()
    {
        fadeTimer += Time.deltaTime;
    }
    private void UIFade()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
            return;
        if (fadeTimer > 3f)
        {
            LevelTarget.text = "";
            LevelText.text = "";
        }
    }

    public void PlayAgain()
    {
        //SceneManager.Scene
        string currentLevel = Level.Instance.CurrentLevel;
        SceneManager.LoadScene(currentLevel);
    }

    public void PlayNextLevel()
    {
        string nextLevel = Level.Instance.NextLevel;
        SceneManager.LoadScene(nextLevel);
    }

    public void Replay()
    {
        SceneManager.LoadScene("Level01");
    }

    public void QuitGame() => Application.Quit();
}

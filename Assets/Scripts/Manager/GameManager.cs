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

    public Transform PlayerTransform;

    private float fadeTimer;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;
        LosePanel.Hide();
        WinPanel.Hide();
    }

    private void Start()
    {
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
    }

    private void TimerTick()
    {
        fadeTimer += Time.deltaTime;
    }
    private void UIFade()
    {
        if (fadeTimer > 3f)
        {
            LevelTarget.text = "";
            LevelText.text = "";
        }
    }

    public void PlayAgain()
    {
        string currentLevel = Level.Instance.CurrentLevel;
        SceneManager.LoadScene(currentLevel);
    }

}

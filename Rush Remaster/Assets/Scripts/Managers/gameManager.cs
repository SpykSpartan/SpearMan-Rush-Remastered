using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("References")]
    public FastTravelManager FastTravelManager;
    public PlayerMovement PlayerMovement;
    public PlayerStat PlayerStatics;
    public RespawnManager RespawnManger;
    public GameObject DeathUI;
    public GameObject WinUI;
    public GameObject popupUI;
    private bool isPaused = false;

    [Header("Boss Settings")]
    private int totalBossCount = 1;
    private int bossesDefeated = 0;

    public GameObject pauseMenuUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LockCursor(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        PlayerMovement = FindObjectOfType<PlayerMovement>();
        PlayerStatics = FindObjectOfType<PlayerStat>();
        RespawnManger = FindObjectOfType<RespawnManager>();
        FastTravelManager = FindObjectOfType<FastTravelManager>();

        DeathUI = GameObject.Find("Death UI");
        WinUI = GameObject.Find("Win UI");
        pauseMenuUI = GameObject.Find("PauseMenuUI");

        LockCursor(true);
    }

    public void ReportBossDefeated(string bossName)
    {
        bossesDefeated++;
        Debug.Log($"Boss defeated: {bossName} ({bossesDefeated}/{totalBossCount})");

        if (FastTravelManager != null)
        {
            FastTravelManager.ActivatePoint(bossName);
        }

        if (bossesDefeated >= totalBossCount)
        {
            WinGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void WinGame()
    {
        Debug.Log("PLAYER WINS!");

        if (WinUI != null)
            WinUI.SetActive(true);
        else
            Debug.LogWarning("WinUI is not assigned in the GameManager!");

        LockCursor(false);

        Time.timeScale = 0f;
    }

    public void WinReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");

        Time.timeScale = 1f;

        if (zoneManager.Instance != null)
            Destroy(zoneManager.Instance.gameObject);

        Destroy(this.gameObject);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ResetProgress()
    {
        bossesDefeated = 0;
        Debug.Log("Progress reset.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P pressed — attempting to load save.");
            SaveSystem.Load();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        LockCursor(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        LockCursor(true);
    }

    public void SaveGame()
    {
        SaveSystem.Save();
    }

    public void LoadGame()
    {
        SaveSystem.Load();
    }

    public void ReturnToMainMenu()
    {
        if (zoneManager.Instance != null)
            Destroy(zoneManager.Instance.gameObject);

        Destroy(this.gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    public void GoToBoss()
    {
        if (zoneManager.Instance != null)
            Destroy(zoneManager.Instance.gameObject);

        Destroy(this.gameObject);

        SceneManager.LoadScene("AsianBoss");
    }

    private void LockCursor(bool shouldLock)
    {
        Cursor.visible = !shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void TriggerPopup(GameObject customUI = null)
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (customUI != null)
        {
            customUI.SetActive(true);
        }
        else if (popupUI != null)
        {
            popupUI.SetActive(true);
        }

        LockCursor(false);
    }

    public void ClosePopup(GameObject customUI = null)
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (customUI != null)
        {
            customUI.SetActive(false);
        }
        else if (popupUI != null)
        {
            popupUI.SetActive(false);
        }

        LockCursor(true);
    }
}

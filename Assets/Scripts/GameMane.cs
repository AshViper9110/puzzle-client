using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMane : MonoBehaviour
{
    [SerializeField] GameObject posePanel;
    [SerializeField] TextMeshProUGUI panelText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject startText;
    [SerializeField] GameObject backText;
    [SerializeField] GameObject nextText;

    private float timer = 0f;
    private bool isPaused = false;
    private bool hasStarted = false;

    public static int CurrentStageID = 1;  // 静的に管理してもいいし、PlayerPrefsに保存も可能

    void Start()
    {
        posePanel.SetActive(false);
        timer = 0f;
        hasStarted = false;
    }

    public void LoadNextStage()
    {
        CurrentStageID++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowPanel(string text)
    {
        panelText.text = text;
        if (text == "Pause")
        {
            nextText.SetActive(false);
            backText.SetActive(true);
        }
        else if (text == "Clear")
        {
            nextText.SetActive(true);
            backText.SetActive(false);
        }
        posePanel.SetActive(true);
        isPaused = true;
    }

    public void HidePanel()
    {
        posePanel.SetActive(false);
        isPaused = false;
    }

    public void StartTimer()
    {
        hasStarted = true;
        startText.SetActive(false);
    }

    void Update()
    {
        if (!isPaused && hasStarted)
        {
            timer += Time.deltaTime;
            timeText.text = timer.ToString("F1") + " s";
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}

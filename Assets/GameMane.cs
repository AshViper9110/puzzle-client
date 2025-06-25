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

    private float timer = 0f;
    private bool isPaused = false;
    private bool hasStarted = false;

    void Start()
    {
        posePanel.SetActive(false);
        timer = 0f;
        hasStarted = false;
    }

    void Update()
    {
        if (!isPaused && hasStarted)
        {
            timer += Time.deltaTime;
            timeText.text = timer.ToString("F1") + " s";
        }
    }

    public void ShowPanel(string text)
    {
        Debug.Log(timer + "s");
        panelText.text = text;
        posePanel.SetActive(true);
        isPaused = true;
    }

    public void HidePanel()
    {
        posePanel.SetActive(false);
        isPaused = false;
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartTimer()
    {
        hasStarted = true;
        startText.SetActive(false);
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}

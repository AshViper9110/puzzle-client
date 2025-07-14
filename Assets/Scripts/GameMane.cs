using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMane : MonoBehaviour
{
    // --- UIオブジェクト ---
    [SerializeField] GameObject posePanel;         // 一時停止/クリア表示用パネル
    [SerializeField] TextMeshProUGUI panelText;    // パネル内テキスト（"Pause"や"Clear"など）
    [SerializeField] TextMeshProUGUI timeText;     // タイマー表示用テキスト
    [SerializeField] GameObject startText;         // スタート時のテキスト（例："Tap to Start"）
    [SerializeField] GameObject backText;          // "戻る"ボタン（Pause用）
    [SerializeField] GameObject nextText;          // "次へ"ボタン（Clear用）

    private float timer = 0f;       // 経過時間
    private bool isPaused = false; // 一時停止中かどうか
    private bool hasStarted = false; // ゲームが開始されたかどうか

    // 現在のステージID（全体で共有するためstaticにしている）
    public static int CurrentStageID = 1;

    void Start()
    {
        // 前の画面で選択されたステージIDを読み取る
        int stageId = PlayerPrefs.GetInt("SelectedPanelId", -1);
        Debug.Log($"[GameScene] 受け取ったステージID: {stageId}");

        if (stageId == -1)
        {
            Debug.LogError("ステージIDが渡されていません！");
        }
        else
        {
            GameMane.CurrentStageID = stageId;
        }

        // 初期状態の設定
        posePanel.SetActive(false);
        timer = 0f;
        hasStarted = false;
    }

    // --- ステージ操作関係 ---

    // 次のステージを読み込む（IDを1つ増やす）
    public void LoadNextStage()
    {
        CurrentStageID++;
        PlayerPrefs.SetInt("SelectedPanelId", CurrentStageID);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンを再読み込み
    }

    // 現在のステージを再読み込み
    public void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ステージセレクト画面に戻る
    public void Exit()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    // --- パネルの表示制御 ---

    // パネル表示と内容制御（"Pause" または "Clear"）
    public void ShowPanel(string text)
    {
        panelText.text = text;

        if (text == "Pause")
        {
            nextText.SetActive(false);  // 次へボタンを非表示
            backText.SetActive(true);   // 戻るボタンを表示
        }
        else if (text == "Clear")
        {
            nextText.SetActive(true);   // 次へボタンを表示
            backText.SetActive(false);  // 戻るボタンを非表示
        }

        posePanel.SetActive(true);  // パネルを表示
        isPaused = true;            // 一時停止状態にする
    }

    // パネルを非表示にしてゲームを再開
    public void HidePanel()
    {
        posePanel.SetActive(false);
        isPaused = false;
    }

    // ゲームスタート時に呼ばれる（スタートテキストを非表示）
    public void StartTimer()
    {
        hasStarted = true;
        startText.SetActive(false);
    }

    void Update()
    {
        // 一時停止していない & ゲームが開始されていれば時間を加算
        if (!isPaused && hasStarted)
        {
            timer += Time.deltaTime;
            timeText.text = timer.ToString("F1") + " s";  // 小数1桁で表示
        }
    }

    // 一時停止中かどうか取得（他スクリプトから確認用）
    public bool IsPaused()
    {
        return isPaused;
    }
}

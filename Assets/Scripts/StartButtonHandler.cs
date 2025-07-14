using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スタートボタンが押された時の処理を行うクラス
/// 選択されたステージIDを取得し、次のシーンに遷移する
/// </summary>
public class StartButtonHandler : MonoBehaviour
{
    // ステージ選択パネル管理スクリプト（インスペクターで指定または自動取得）
    public SelectMane selectMane;

    void Awake()
    {
        // selectMane が未指定ならシーン内から自動取得
        if (selectMane == null)
        {
            selectMane = FindObjectOfType<SelectMane>();
            if (selectMane == null)
            {
                Debug.LogError("SelectMane がシーンに見つかりません！");
            }
        }
    }

    /// <summary>
    /// スタートボタンが押されたときに呼ばれるメソッド
    /// 選択されたステージIDを保存し、GameScene に遷移する
    /// </summary>
    public void OnStartButtonPressed()
    {
        // 現在選択されているパネルの ID を取得
        int selectedId = selectMane.GetCurrentPanelId();
        Debug.Log("[StartButton] 選択ID: " + selectedId);

        // PlayerPrefs に保存（次のシーンでステージIDを使うため）
        PlayerPrefs.SetInt("SelectedPanelId", selectedId);
        PlayerPrefs.Save();
        Debug.Log("[StartButton] PlayerPrefsに保存完了。シーン遷移開始。");

        // ゲームシーンへ遷移（シーン名は正確に設定されている必要あり）
        SceneManager.LoadScene("GameScene");
    }
}

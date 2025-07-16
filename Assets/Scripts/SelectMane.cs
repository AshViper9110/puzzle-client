using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// ステージ選択用パネルを生成し、左右にスライドして選択できるようにするクラス。
/// Laravel API からステージの数を取得し、それに応じたパネルを生成する。
/// </summary>
public class SelectMane : MonoBehaviour
{
    [Header("パネル関連")]
    public GameObject panelPrefab;     // ステージパネルのプレハブ
    public float spacing = 300f;       // パネル間の間隔（x軸方向）
    public float centerScale = 1.0f;   // 中央パネルのスケール
    public float sideScale = 0.5f;     // 両サイドのパネルの横スケール
    public float heightScale = 0.25f;  // 両サイドのパネルの縦スケール
    public float moveSpeed = 10f;      // スライド移動の速度

    private List<RectTransform> panels = new List<RectTransform>(); // パネルのリスト
    private int currentIndex = 0; // 現在選択中のインデックス

    void Start()
    {
        // 起動時にステージ数をAPIから取得してパネル生成を開始
        StartCoroutine(FetchStageCountAndCreatePanels());
    }

    /// <summary>
    /// APIからステージ数を取得してパネルを生成するコルーチン
    /// </summary>
    IEnumerator FetchStageCountAndCreatePanels()
    {
        string url = "http://localhost:8000/api/stages";  // Laravel 側のステージ一覧エンドポイント

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("ステージ一覧取得失敗: " + request.error);
                yield break;
            }

            // JSONからステージ数を取得
            string json = request.downloadHandler.text;
            StageCountResponse response = JsonUtility.FromJson<StageCountResponse>(json);
            int stageCount = response.count;

            if (stageCount <= 0)
            {
                Debug.LogWarning("ステージ数が0以下です。パネルを作成しません。");
                yield break;
            }

            // ステージ数に応じてパネルを生成
            CreatePanels(stageCount);
        }
    }

    /// <summary>
    /// 指定された数だけステージ選択パネルを生成
    /// </summary>
    void CreatePanels(int count)
    {
        // 既存パネルがあれば削除
        foreach (var panel in panels)
        {
            Destroy(panel.gameObject);
        }
        panels.Clear();

        for (int i = 0; i < count; i++)
        {
            // プレハブからパネル生成
            GameObject panelObj = Instantiate(panelPrefab, transform);

            RectTransform rt = panelObj.GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError("PanelPrefabにRectTransformが必要です");
                continue;
            }

            // PanelData（任意のID保持用）を設定
            PanelData panelData = panelObj.GetComponent<PanelData>();
            if (panelData != null)
            {
                panelData.id = i + 1;
                panelData.SetId(i + 1);
            }

            // パネルクリック時の処理
            Button btn = panelObj.GetComponent<Button>();
            if (btn != null)
            {
                int capturedId = i + 1;
                btn.onClick.AddListener(() =>
                {
                    Debug.Log($"パネルID {capturedId} がクリックされました");
                    // 必要であれば、ここで選択処理や画面遷移処理を書く
                });
            }

            // パネルをリストに追加
            panels.Add(rt);
        }
    }

    void Update()
    {
        if (panels.Count == 0) return;
        UpdatePanelPositions(); // 毎フレーム位置とスケールを更新してアニメーションを実現
    }

    /// <summary>
    /// 各パネルの位置・スケール・透明度を更新
    /// </summary>
    void UpdatePanelPositions()
    {
        if (panels.Count == 0) return;

        // 現在のインデックスから左右のパネルのインデックスを計算
        int leftIndex = (currentIndex - 1 + panels.Count) % panels.Count;
        int rightIndex = (currentIndex + 1) % panels.Count;
        int farLeftIndex = (currentIndex - 2 + panels.Count) % panels.Count;
        int farRightIndex = (currentIndex + 2) % panels.Count;

        float lerpFactor = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime); // 滑らかに移動させる係数
        float nonSelectedScaleMultiplier = 0.8f; // 選択されていないパネルのスケール倍率
        float nonSelectedAlpha = 0.5f;           // 選択されていないパネルの透明度

        for (int i = 0; i < panels.Count; i++)
        {
            RectTransform rt = panels[i];
            GameObject go = rt.gameObject;

            // 透明度制御用のCanvasGroupがなければ追加
            CanvasGroup cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();

            Vector2 targetPos;
            Vector3 selectedScale = new Vector3(sideScale, heightScale, 1f);
            Vector3 targetScale;
            float targetAlpha;

            if (i == currentIndex)
            {
                targetPos = Vector2.zero;
                targetScale = selectedScale * centerScale;
                targetAlpha = 1f;
                go.SetActive(true);
            }
            else if (i == leftIndex)
            {
                targetPos = new Vector2(-spacing, 0);
                targetScale = selectedScale * nonSelectedScaleMultiplier;
                targetAlpha = nonSelectedAlpha;
                go.SetActive(true);
            }
            else if (i == rightIndex)
            {
                targetPos = new Vector2(spacing, 0);
                targetScale = selectedScale * nonSelectedScaleMultiplier;
                targetAlpha = nonSelectedAlpha;
                go.SetActive(true);
            }
            else if (i == farLeftIndex)
            {
                targetPos = new Vector2(-spacing * 2, 0);
                targetScale = selectedScale * nonSelectedScaleMultiplier * 0.8f;
                targetAlpha = nonSelectedAlpha * 0.7f;
                go.SetActive(true);
            }
            else if (i == farRightIndex)
            {
                targetPos = new Vector2(spacing * 2, 0);
                targetScale = selectedScale * nonSelectedScaleMultiplier * 0.8f;
                targetAlpha = nonSelectedAlpha * 0.7f;
                go.SetActive(true);
            }
            else
            {
                // それ以外のパネルは非表示
                go.SetActive(false);
                continue;
            }

            // 補間で位置・スケール・透明度を滑らかに反映
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, lerpFactor);
            rt.localScale = Vector3.Lerp(rt.localScale, targetScale, lerpFactor);
            cg.alpha = Mathf.Lerp(cg.alpha, targetAlpha, lerpFactor);
            cg.interactable = (i == currentIndex);    // 中央だけ操作可能に
            cg.blocksRaycasts = (i == currentIndex);  // 中央だけクリック判定有効
        }
    }

    /// <summary>
    /// 左にスライド
    /// </summary>
    public void SlideLeft()
    {
        if (panels.Count == 0) return;
        currentIndex = (currentIndex - 1 + panels.Count) % panels.Count;
    }

    /// <summary>
    /// 右にスライド
    /// </summary>
    public void SlideRight()
    {
        if (panels.Count == 0) return;
        currentIndex = (currentIndex + 1) % panels.Count;
    }

    /// <summary>
    /// 現在選択されているパネルのIDを取得
    /// </summary>
    public int GetCurrentPanelId()
    {
        if (currentIndex < 0 || currentIndex >= panels.Count) return -1;

        PanelData data = panels[currentIndex].GetComponent<PanelData>();
        return data != null ? data.id : -1;
    }

    // APIレスポンスの形式に合わせたデシリアライズ用クラス
    [System.Serializable]
    public class StageCountResponse
    {
        public int count;
    }
}

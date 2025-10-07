using System.Collections;
using UnityEngine;

public class BoxMove : MonoBehaviour
{
    public float moveDuration = 0.2f; // 移動にかける時間（アニメーション速度）
    public float cellSize = 1f;       // 1マスの大きさ（Tilemapなら1でOK）
    public LayerMask wallLayer;       // 壁のレイヤー（Inspectorで設定）
    public LayerMask boxLayer;        // ボックスのレイヤー（Inspectorで設定）

    private bool isMoving = false;

    /// <summary>
    /// プレイヤーから呼び出される移動メソッド
    /// </summary>
    public bool Move(Vector2 direction)
    {
        if (!isMoving)
        {
            // 移動先に壁があるかチェック
            if (CanMove(direction))
            {
                // 連鎖的にボックスを押す
                StartCoroutine(MoveWithPush(direction));
                return true; // 移動成功
            }
        }
        return false; // 移動失敗
    }

    /// <summary>
    /// ボックスを押してから移動するコルーチン
    /// </summary>
    private IEnumerator MoveWithPush(Vector2 direction)
    {
        isMoving = true;

        // 移動先に他のボックスがあるかチェック（自分自身を除外）
        RaycastHit2D[] boxHits = Physics2D.BoxCastAll(
            transform.position,
            Vector2.one * 0.8f,
            0f,
            direction,
            cellSize,
            boxLayer
        );

        // 自分以外のボックスを探す
        foreach (RaycastHit2D hit in boxHits)
        {
            // 自分自身はスキップ
            if (hit.collider.gameObject == gameObject)
                continue;

            BoxMove nextBox = hit.collider.GetComponent<BoxMove>();
            if (nextBox != null)
            {
                // 次のボックスを移動させる
                nextBox.Move(direction);

                // 次のボックスの移動が完了するまで待つ
                yield return new WaitUntil(() => !nextBox.isMoving);
                break; // 最初に見つかったボックスだけ処理
            }
        }

        // 自分の移動アニメーション開始
        yield return StartCoroutine(MoveAnimation(direction));

        isMoving = false;
    }

    /// <summary>
    /// 次のボックスを押す処理（削除）
    /// </summary>
    private void PushNextBox(Vector2 direction)
    {
        // 移動先に他のボックスがあるかチェック
        RaycastHit2D boxHit = Physics2D.BoxCast(
            transform.position,
            Vector2.one * 0.8f,
            0f,
            direction,
            cellSize,
            boxLayer
        );

        if (boxHit.collider != null)
        {
            BoxMove nextBox = boxHit.collider.GetComponent<BoxMove>();
            if (nextBox != null)
            {
                // 次のボックスを移動させる
                nextBox.Move(direction);
            }
        }
    }

    /// <summary>
    /// 指定方向に移動可能かチェック
    /// </summary>
    private bool CanMove(Vector2 direction)
    {
        Vector2 targetPos = (Vector2)transform.position + direction * cellSize;

        // 壁があるかチェック
        RaycastHit2D wallHit = Physics2D.BoxCast(
            transform.position,
            Vector2.one * 0.8f,
            0f,
            direction,
            cellSize,
            wallLayer
        );

        // 壁にぶつかる場合は移動不可
        if (wallHit.collider != null)
        {
            return false;
        }

        // 他のボックスがあるかチェック（自分自身を除外）
        RaycastHit2D[] boxHits = Physics2D.BoxCastAll(
            transform.position,
            Vector2.one * 0.8f,
            0f,
            direction,
            cellSize,
            boxLayer
        );

        // 自分以外のボックスをチェック
        foreach (RaycastHit2D hit in boxHits)
        {
            // 自分自身はスキップ
            if (hit.collider.gameObject == gameObject)
                continue;

            BoxMove nextBox = hit.collider.GetComponent<BoxMove>();
            if (nextBox != null)
            {
                // 次のボックスが移動できるかチェック（再帰的）
                return nextBox.CanMove(direction);
            }
            return false;
        }

        // 衝突がなければ移動可能
        return true;
    }

    /// <summary>
    /// 実際の移動アニメーション
    /// </summary>
    private IEnumerator MoveAnimation(Vector2 direction)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction * cellSize;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector2.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // 最終位置補正
    }
}
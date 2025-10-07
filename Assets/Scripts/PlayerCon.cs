using System.Collections;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveDuration = 0.2f; // 移動にかける時間
    public float cellSize = 1f;       // 1マスの大きさ
    public LayerMask wallLayer;       // 壁のレイヤー
    public LayerMask boxLayer;        // ボックスのレイヤー
    public Joystick joystick;         // ジョイスティック

    private Rigidbody2D rb;
    private GameMane gameMane;
    private bool hasMoved = false;
    private bool isMoving = false;

    // 入力の閾値（ジョイスティックの遊び）
    private float inputThreshold = 0.5f;
    private Vector2 lastDirection = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematicに変更

        gameMane = FindObjectOfType<GameMane>();

        if (joystick == null)
        {
            joystick = FindObjectOfType<FixedJoystick>();
        }
    }

    private void Update()
    {
        if (gameMane != null && gameMane.IsPaused()) return;
        if (isMoving) return; // 移動中は入力を受け付けない

        Vector2 direction = GetInputDirection();

        // 方向入力があれば移動を試みる
        if (direction != Vector2.zero)
        {
            TryMove(direction);
        }
    }

    /// <summary>
    /// 入力から移動方向を取得（4方向のみ）
    /// </summary>
    private Vector2 GetInputDirection()
    {
        float moveX = 0f;
        float moveY = 0f;

        // ジョイスティック入力
        if (joystick != null)
        {
            moveX = joystick.Horizontal;
            moveY = joystick.Vertical;
        }

        // PC入力（ジョイスティック入力がない場合）
        if (Mathf.Abs(moveX) < inputThreshold && Mathf.Abs(moveY) < inputThreshold)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }

        // 4方向に制限（上下左右のみ）
        Vector2 direction = Vector2.zero;

        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
        {
            // 横方向優先
            if (Mathf.Abs(moveX) > inputThreshold)
            {
                direction = moveX > 0 ? Vector2.right : Vector2.left;
            }
        }
        else
        {
            // 縦方向優先
            if (Mathf.Abs(moveY) > inputThreshold)
            {
                direction = moveY > 0 ? Vector2.up : Vector2.down;
            }
        }

        // 同じ方向の連続入力を防ぐ
        if (direction == lastDirection)
        {
            return Vector2.zero;
        }

        lastDirection = direction;
        return direction;
    }

    /// <summary>
    /// 移動を試みる
    /// </summary>
    private void TryMove(Vector2 direction)
    {
        Vector2 targetPos = (Vector2)transform.position + direction * cellSize;

        // 移動先に壁があるかチェック
        if (CheckCollision(transform.position, direction, wallLayer))
        {
            return; // 壁があるので移動しない
        }

        // 移動先にボックスがあるかチェック
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
            // ボックスを押す
            BoxMove box = boxHit.collider.GetComponent<BoxMove>();
            if (box != null)
            {
                // ボックスが移動できたらプレイヤーも移動
                if (box.Move(direction))
                {
                    StartCoroutine(MoveCoroutine(direction));
                }
            }
        }
        else
        {
            // 何もない場合は普通に移動
            StartCoroutine(MoveCoroutine(direction));
        }
    }

    /// <summary>
    /// 衝突判定
    /// </summary>
    private bool CheckCollision(Vector2 origin, Vector2 direction, LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            Vector2.one * 0.8f,
            0f,
            direction,
            cellSize,
            layer
        );

        return hit.collider != null;
    }

    /// <summary>
    /// プレイヤーの移動アニメーション
    /// </summary>
    private IEnumerator MoveCoroutine(Vector2 direction)
    {
        isMoving = true;
        Vector2 start = transform.position;
        Vector2 end = start + direction * cellSize;
        float elapsed = 0f;

        // 初回移動でタイマー開始
        if (!hasMoved)
        {
            hasMoved = true;
            gameMane?.StartTimer();
        }

        while (elapsed < moveDuration)
        {
            transform.position = Vector2.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // 最終位置補正
        isMoving = false;
        lastDirection = Vector2.zero; // 移動完了後、方向をリセット
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Clear");
            gameMane?.ShowPanel("Clear");
        }
    }
}
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick; // 🎮 インスペクタでアタッチ

    private Rigidbody2D rb;
    private GameMane gameMane;
    private bool hasMoved = false;
    public float speed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameMane = FindObjectOfType<GameMane>();
        joystick = FindObjectOfType<FixedJoystick>();
    }

    private void FixedUpdate()
    {
        if (gameMane != null && gameMane.IsPaused()) return;

        float moveX = 0f;
        float moveY = 0f;

        // 🎮 モバイル入力（ジョイスティック）
        if (joystick != null)
        {
            moveX = joystick.Horizontal;
            moveY = joystick.Vertical;
        }

        // 🎮 PC入力（WASD / 矢印キー）
        //   ※ジョイスティック入力がゼロのときのみPC入力を反映
        if (Mathf.Approximately(moveX, 0f) && Mathf.Approximately(moveY, 0f))
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
        }

        // 入力ベクトルを正規化
        Vector2 move = new Vector2(moveX, moveY).normalized;

        // Rigidbody移動
        rb.velocity = move * moveSpeed;

        // 初回移動でタイマー開始
        if (!hasMoved && move.sqrMagnitude > 0f)
        {
            hasMoved = true;
            gameMane?.StartTimer();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Clear");
            gameMane.ShowPanel("Clear");
        }
    }
}

using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick; // 🎮 インスペクタでアタッチ

    private Rigidbody2D rb;
    private GameMane gameMane;
    private bool hasMoved = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameMane = FindObjectOfType<GameMane>();
    }

    private void FixedUpdate()
    {
        if (gameMane != null && gameMane.IsPaused()) return;

        // 🎮 Joystick入力
        float moveX = joystick.Horizontal;
        float moveY = joystick.Vertical;

        Vector2 move = new Vector2(moveX, moveY).normalized;

        rb.velocity = move * moveSpeed;

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

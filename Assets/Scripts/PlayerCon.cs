using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveSpeed = 5f;     // 移動速度
    public float jumpForce = 5f;     // ジャンプ力

    private Rigidbody2D rb;
    private bool isGrounded;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;

    private GameMane gameMane;
    private bool hasMoved = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameMane = FindObjectOfType<GameMane>();
    }

    private void Update()
    {
        // ✅ Rayで真下に接地判定（レイヤー「Ground」に限定）
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 0.6f; // プレイヤーの足元の高さに合わせて調整
        isGrounded = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask("Ground"));

        // ジャンプ（キーボード）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !gameMane.IsPaused())
        {
            JumpMove();
        }

        // Debug（Sceneビューで足元のRay表示）
        //Debug.DrawRay(origin, direction * distance, Color.red);
    }

    void FixedUpdate()
    {
        if (!gameMane.IsPaused())
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            if (isLeftPressed) moveX = -1;
            if (isRightPressed) moveX = 1;

            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

            // 🔸 動き出したらタイマー開始（1回だけ）
            if (!hasMoved && (moveX != 0 || rb.velocity.y != 0))
            {
                hasMoved = true;
                gameMane?.StartTimer();
            }
        }
    }

    // UIボタン用（押しっぱなし対応）
    public void OnLeftDown() => isLeftPressed = true;
    public void OnLeftUp() => isLeftPressed = false;
    public void OnRightDown() => isRightPressed = true;
    public void OnRightUp() => isRightPressed = false;

    // ジャンプ共通処理
    public void JumpMove()
    {
        if (isGrounded && !gameMane.IsPaused())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // ゴール判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Clear");
            gameMane.ShowPanel("Clear");
        }
    }
}

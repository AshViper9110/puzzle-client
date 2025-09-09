using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMove : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ���擾
    }

    void Update()
    {
        Vector2 velocity = rb.velocity;

        // �΂߂ɓ����Ă�����␳
        if (Mathf.Abs(velocity.x) > 0 && Mathf.Abs(velocity.y) > 0)
        {
            // X�������傫�����X��D��
            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
                velocity.y = 0;
            else
                velocity.x = 0;
        }

        rb.velocity = velocity;
    }
}

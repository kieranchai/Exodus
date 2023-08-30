using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Player { get; private set; }

    public Rigidbody2D rb;

    public float speed;

    private void Awake()
    {
        if (Player != null && Player != this)
        {
            Destroy(gameObject);
            return;
        }
        Player = this;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        LookAtMouse();
        MovePlayer();
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    public void MovePlayer()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = input.normalized * speed;
    }

}

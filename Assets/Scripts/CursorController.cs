using UnityEngine;

public class CursorController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.PAUSED ||
            GameController.instance.currentState == GameController.GAME_STATE.DEAD ||
            GameController.instance.currentState == GameController.GAME_STATE.WIN ||
            GameController.instance.currentState == GameController.GAME_STATE.PANNING ||
            GameController.instance.isOverUI)
        {
            Cursor.visible = true;
            spriteRenderer.enabled = false;
        }
        else
        {
            Cursor.visible = false;
            spriteRenderer.enabled = true;
            Vector2 mouseCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mouseCursorPos;
        }
    }
}

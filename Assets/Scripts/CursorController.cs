using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursor_normal;
    public Vector2 normalCursorHotspot;

    public Texture2D cursor_Swap;
    public Vector2 swapCursorHotspot;

    public Texture2D cursor_Buy;
    public Vector2 buyCursorHotspot;

    public Texture2D cursor_Sell;
    public Vector2 sellCursorHotspot;

    public void OnSwapCursorEnter()
    {
        Cursor.SetCursor(cursor_Swap, swapCursorHotspot, CursorMode.Auto);
    }

    public void OnShopCursorEnter()
    {
        if (this.gameObject.GetComponent<ShopWeapon>() != null)
        {
            if (this.gameObject.GetComponent<ShopWeapon>().type == "buy") Cursor.SetCursor(cursor_Buy, buyCursorHotspot, CursorMode.Auto);
            if (this.gameObject.GetComponent<ShopWeapon>().type == "sell") Cursor.SetCursor(cursor_Sell, sellCursorHotspot, CursorMode.Auto);
            return;
        }

        Cursor.SetCursor(cursor_Buy, buyCursorHotspot, CursorMode.Auto);

    }

    public void OnCursorExit()
    {
        Cursor.SetCursor(cursor_normal, normalCursorHotspot, CursorMode.Auto); ;
    }
}

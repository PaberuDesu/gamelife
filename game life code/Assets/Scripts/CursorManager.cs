using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Paint paint;

    [SerializeField] private Texture2D brushTexture;
    [SerializeField] private Texture2D fillTexture;

    private CursorAppearance defaultCursor;
    private CursorAppearance brushCursor;
    private CursorAppearance fillCursor;

    private void Awake() {
        defaultCursor = new CursorAppearance(null, Vector2.zero);
        brushCursor = new CursorAppearance(brushTexture, Vector2.up * brushTexture.height);
        fillCursor = new CursorAppearance(fillTexture, Vector2.up * fillTexture.height);
    }

    public void SetCursor(bool setUp) {
        if (setUp) {
            if (paint.brush == Paint.Brush.Fill) fillCursor.SetUp();
            else brushCursor.SetUp();
        }
        else defaultCursor.SetUp();
    }
}

internal class CursorAppearance
{
    private Texture2D texture;
    private Vector2 hotspot;

    internal CursorAppearance(Texture2D texture, Vector2 hotspot) {
        this.texture = texture;
        this.hotspot = hotspot;
    }

    internal void SetUp() {Cursor.SetCursor(texture, hotspot, CursorMode.Auto);}
}
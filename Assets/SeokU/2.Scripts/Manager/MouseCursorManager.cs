using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : Singleton<MouseCursorManager>
{
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D attackCursor;

    private void Awake()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    public void DefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    public void AttackCursor()
    {
        Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
}

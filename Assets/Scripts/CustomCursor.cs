using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture; // reference to texture
    public CursorMode cursorMode = CursorMode.ForceSoftware; // allows to render hardware cursor

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

}

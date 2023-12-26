using UnityEngine;
using UnityEngine.UI;

public class PanelStabilizer : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layout;
    private Vector2 screenScale = Vector2.zero;

    private void Update() {
        if (screenScale.x != Screen.width || screenScale.y != Screen.height) {
            screenScale = new Vector2(Screen.width, Screen.height);
            if (screenScale.x/screenScale.y < 1.5f)
                layout.transform.localScale = Vector3.one * screenScale.x/screenScale.y/1.5f;
            layout.childForceExpandWidth = false;
            Invoke("MakeTrue",0.001f);
        }
    }

    private void MakeTrue() {
        layout.childForceExpandWidth = true;
    }
}
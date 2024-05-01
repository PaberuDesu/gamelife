using UnityEngine;

public class EscClosePanel : MonoBehaviour
{
    private void Update() {if (Input.GetKeyDown(KeyCode.Escape)) {gameObject.SetActive(false);}}
}
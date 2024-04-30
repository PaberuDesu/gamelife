using UnityEngine;

public class escape : MonoBehaviour
{
    [SerializeField] private GameObject escape_menu;
    [SerializeField] private GameObject settings_menu;
    [SerializeField] private GameObject saveload_menu;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (settings_menu.activeSelf) settings_menu.SetActive(false);
            else if (saveload_menu.activeSelf) saveload_menu.SetActive(false);
            escape_menu.SetActive(!escape_menu.activeSelf);
        }
    }
}
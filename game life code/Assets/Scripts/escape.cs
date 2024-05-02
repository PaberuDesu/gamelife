using UnityEngine;
using UnityEngine.UI;

public class escape : MonoBehaviour
{
    [SerializeField] private GameObject escape_menu;
    [SerializeField] private GameObject settings_menu;
    [SerializeField] private GameObject saveload_menu;
    [SerializeField] private Activator activator;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (settings_menu.activeSelf) settings_menu.SetActive(false);
            else if (saveload_menu.activeSelf) saveload_menu.SetActive(false);
            Escape();
        }
    }

    public void Escape() {
        activator.Activate(escape_menu.activeSelf);
        escape_menu.SetActive(!escape_menu.activeSelf);
    }

    public void EscapeSaveMenu() {
        saveload_menu.SetActive(false);
        activator.Activate(true);
    }
}
using UnityEngine;

public class escape : MonoBehaviour
{
    [SerializeField] private GameObject escape_menu;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            escape_menu.SetActive(!escape_menu.activeSelf);
        }
    }
}
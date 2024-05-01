using UnityEngine;

public class DropdownCloser : MonoBehaviour
{
    [SerializeField] private GameObject butOpen;
    [SerializeField] private GameObject butClose;
    [SerializeField] private GameObject dropdown;
    private void OnDisable() {
        butOpen.SetActive(true);
        butClose.SetActive(false);
        dropdown.SetActive(false);
    }
}
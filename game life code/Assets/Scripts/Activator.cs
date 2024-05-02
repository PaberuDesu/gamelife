using UnityEngine;
using UnityEngine.UI;

public class Activator : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private void OnEnable() {
        slider.interactable = true;
    }

    public void Activate(bool value) {
        slider.interactable = value;
    }
}
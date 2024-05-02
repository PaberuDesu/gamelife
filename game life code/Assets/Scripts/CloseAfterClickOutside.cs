using UnityEngine;
using UnityEngine.EventSystems;

public class CloseAfterClickOutside : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool is_over;
    private static bool is_clicked_on_functional;

    private void OnEnable() {
        is_over = false;
        is_clicked_on_functional = false;
    }

    private void OnDisable() {gameObject.SetActive(false);}
    
    public static void setOver() {is_clicked_on_functional = true;}

    private void Update() {
        if (is_clicked_on_functional) {
            is_over = false;
            is_clicked_on_functional = false;
            gameObject.SetActive(false);
            return;
        }
        if (Input.GetMouseButtonUp(0) && !is_over) gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {is_over = true;}

    public void OnPointerExit(PointerEventData eventData) {is_over = false;}
}
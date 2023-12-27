using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SavedPaletteViewer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject palette;

    public void OnPointerEnter(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(Activate());
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(Deactivate());
    }

    private IEnumerator Activate() {
        palette.SetActive(true);
        for (float height = 0f; height < 1; height += 0.1f) {
            palette.transform.localScale = new Vector3(1, height, 1);
            yield return new WaitForSeconds(.01f);
        }
    }
    private IEnumerator Deactivate() {
        for (float height = 1f; height > 0; height -= 0.1f) {
            palette.transform.localScale = new Vector3(1, height, 1);
            yield return new WaitForSeconds(.01f);
        }
        palette.SetActive(false);
    }
}
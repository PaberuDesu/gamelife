using UnityEngine;
using UnityEngine.UI;

public class MessageCenter : MonoBehaviour
{
    public GameObject SuccessMessage;
    public GameObject ObjectExistsMessage;
    public GameObject WrongCoordinatesMessage;

    private void OnDisable() {foreach (Transform child in transform) Destroy(child.gameObject);}

    public void MessageCellChanged(bool isCellAlive, int x = 0, int y = 0, int z = 0) {
        Instantiate(SuccessMessage, transform).transform.GetChild(0).gameObject.GetComponent<Text>().text =
            isCellAlive
            ? $"Клетка успешно создана в точке ({x}, {y}, {z})"
            : $"Клетка успешно удалена из точки ({x}, {y}, {z})";
    }

    public void MessageCellExists() {Instantiate(ObjectExistsMessage, transform);}

    public void MessageWrongCoordinates() {Instantiate(WrongCoordinatesMessage, transform);}
}

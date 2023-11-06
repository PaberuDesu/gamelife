using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageCenter : MonoBehaviour
{
    public GameObject SuccessMessage;
    public GameObject ObjectExistsMessage;
    public GameObject WrongCoordinates;

    public void Message(byte MessageCode, int x, int y, int z)
    {
        GameObject message;
        switch (MessageCode)
        {
            case 0:
                message = Instantiate(SuccessMessage, transform);
                message.name = $"success({x}, {y}, {z})";
                message.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"Клетка успешно создана в точке ({x}, {y}, {z})";
                break;
            case 1:
                message = Instantiate(ObjectExistsMessage, transform);
                message.name = $"failure({x}, {y}, {z})";
                break;
            case 2:
                message = Instantiate(WrongCoordinates, transform);
                message.name = $"failure: uncorrect coordinates";
                break;
            case 3:
                message = Instantiate(SuccessMessage, transform);
                message.name = $"success({x}, {y}, {z})";
                message.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"Клетка успешно удалена из точки ({x}, {y}, {z})";
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputCleaner : MonoBehaviour
{
    private InputField field;
    [SerializeField] private string text;
    private void OnEnable() {
        if (field is null) field = gameObject.GetComponent<InputField>();
        field.text = text;
    }
}
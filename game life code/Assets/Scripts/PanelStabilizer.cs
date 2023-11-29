using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelStabilizer : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layout;

    private void Start() {
        layout.childForceExpandWidth = false;
        Invoke("MakeTrue",0.01f);
    }

    private void MakeTrue() {
        layout.childForceExpandWidth = true;
    }
}

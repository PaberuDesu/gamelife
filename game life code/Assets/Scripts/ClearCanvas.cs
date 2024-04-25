using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearCanvas : MonoBehaviour
{
    [SerializeField] private Paint canvas;
    [SerializeField] private Image progressbar;
    private bool _holding = false;
    private float timeHold = 0.3f;

    public void setHolding(bool value) {_holding = value;}

    void Update() {
        if (_holding && progressbar.fillAmount < 1) {
            progressbar.fillAmount += 1.0f/timeHold * Time.deltaTime;
            return;
        }
        if (!_holding && progressbar.fillAmount > 0) {
            progressbar.fillAmount -= 2.0f/timeHold * Time.deltaTime;
            return;
        }
        if (progressbar.fillAmount == 1) {
            _holding = false;
            canvas.Clear();
        }
    }
}
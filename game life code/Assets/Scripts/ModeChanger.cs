using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChanger : MonoBehaviour
{
    [SerializeField] private SupportTypeSelecting script;
    private const short step = 1300;
    private int remain_of_step;
    private int MaximumAbs;
    private const byte FPS = 10;

    private void Awake() {
        MaximumAbs = step * (transform.childCount - 1) / 2;
        remain_of_step = MaximumAbs % step;
    }

    public void ChangeModeByButton(int MoveMultiplier) {
        if (-MoveMultiplier * transform.localPosition.x != MaximumAbs && Mathf.Abs(transform.localPosition.x % (step)) == remain_of_step) {
            script.SelectedCellType += MoveMultiplier;
            if (script is Settings settingsScript) settingsScript.ChangeButtonsColors();
            StartCoroutine(ChangeMode(MoveMultiplier));
        }
    }

    private IEnumerator ChangeMode(int MoveMultiplier) {
        for (byte i = 0; i < FPS; i++) {
            transform.localPosition += (step * MoveMultiplier / FPS) * Vector3.left;
            yield return new WaitForSeconds(0.1f / FPS);
        }
    }
}
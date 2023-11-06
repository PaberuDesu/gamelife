using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChanger : MonoBehaviour
{
    [SerializeField] pregameLogic pregame;
    short step = 1300;
    int remain_of_step;
    int MaximumAbs;
    byte FPS = 10;

    void Awake()
    {
        MaximumAbs = step * (transform.childCount - 1) / 2;
        remain_of_step = MaximumAbs % step;
    }

    public void ChangeModeByButton(int MoveMultiplier)
    {
        StartCoroutine(ChangeMode(Convert.ToSByte(MoveMultiplier)));
    }

    IEnumerator ChangeMode(sbyte MoveMultiplier)
    {
        if (-MoveMultiplier * transform.localPosition.x != MaximumAbs && Mathf.Abs(transform.localPosition.x % (step)) == remain_of_step)
        {
            for (byte i = 0; i < FPS; i++)
            {
                transform.localPosition += (step * MoveMultiplier / FPS) * Vector3.left;
                yield return new WaitForSeconds(0.1f / FPS);
            }
            pregame.SelectedCellType += MoveMultiplier;
        }
    }
}
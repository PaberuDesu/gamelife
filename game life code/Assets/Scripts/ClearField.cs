using UnityEngine;
using UnityEngine.UI;

public class ClearField : MonoBehaviour
{
    [SerializeField] private Field field;
    [SerializeField] private Image progressbar;
    private bool _holding = false;
    private const float inreaseSpeedMultiplier = 3;
    private const float decreaseSpeedMultiplier = -6;

    public void setHolding(bool value) {_holding = value;}

    void Update() {
        if (progressbar.fillAmount == 1 && _holding) {
            _holding = false;
            field.Clear();
            field.AddAction();
        }
        else if (progressbar.fillAmount > 0 || _holding)
            progressbar.fillAmount += Time.deltaTime * (_holding ? inreaseSpeedMultiplier : decreaseSpeedMultiplier);
    }
}
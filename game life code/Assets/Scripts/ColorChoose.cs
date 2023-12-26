using UnityEngine;
using UnityEngine.UI;

public class ColorChoose : MonoBehaviour
{
    [SerializeField] private Paint palette;
    [SerializeField] private int _colorNumber;
    [SerializeField] private Image ColorVisualiser;
    [SerializeField] private Slider[] _values;

    private Color CurrentColor = Color.white;

    private const float maxValue = 255.0f;

    private void Start() {
        CurrentColor = palette._colors[_colorNumber];
        _values[0].value = CurrentColor.r * maxValue;
        _values[1].value = CurrentColor.g * maxValue;
        _values[2].value = CurrentColor.b * maxValue;
    }

    public void ChangeValue(int color) {
        ChooseColor(color, Mathf.RoundToInt(_values[color].value));
    }

    private void ChooseColor(int color, int value) {
        if (color == 0)
            CurrentColor = new Color(value/maxValue, CurrentColor.g, CurrentColor.b);
        else if (color == 1)
            CurrentColor = new Color(CurrentColor.r, value/maxValue, CurrentColor.b);
        else
            CurrentColor = new Color(CurrentColor.r, CurrentColor.g, value/maxValue);
    }

    public void PickColor() {
        ColorVisualiser.color = CurrentColor;
        palette._colors[_colorNumber] = CurrentColor;
        palette.recolor(_colorNumber);
    }
}
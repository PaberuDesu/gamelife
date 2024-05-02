using UnityEngine;
using UnityEngine.UI;

public class ColorChoose : MonoBehaviour
{
    [SerializeField] private Paint palette;
    [SerializeField] private int _colorNumber;
    [SerializeField] private Image ColorVisualiser;
    [SerializeField] private Slider[] _values;

    private Color CurrentColor = Color.white;
    private Color FactColor = Color.white;

    private const float maxValue = 255.0f;

    private void OnEnable() {
        FactColor = palette._colors[_colorNumber];
        CurrentColor = FactColor;
        for (int i = 0; i<3; i++)
            _values[i].value = FactColor[i] * maxValue;
    }

    public void ChangeValue(int rgb) {CurrentColor[rgb] = Mathf.RoundToInt(_values[rgb].value)/maxValue;}

    public void PickColor() {
        ColorVisualiser.color = CurrentColor;
        palette._colors[_colorNumber] = CurrentColor;
        palette.recolor(_colorNumber);
        FactColor = CurrentColor;
    }
}
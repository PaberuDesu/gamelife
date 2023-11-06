using UnityEngine;
using UnityEngine.UI;

public class ChooseBut : MonoBehaviour
{
    [SerializeField] Image _currentBrightButton;
    private Color Bright = Color.white;
    private Color Dim = new Color(0.6f, 0.6f, 0.6f, 1);
    private Vector3 big = Vector3.one;
    private Vector3 little = new Vector3(0.9f,1,1);

    public void ChangeCurrentButton(Image _nextBrightButton){
        _currentBrightButton.color = Dim;
        _currentBrightButton.gameObject.transform.localScale = little;
        _currentBrightButton = _nextBrightButton;
        _currentBrightButton.color = Bright;
        _currentBrightButton.gameObject.transform.localScale = big;
    }
}

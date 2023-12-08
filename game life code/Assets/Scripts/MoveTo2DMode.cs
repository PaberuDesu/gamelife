using UnityEngine;
using UnityEngine.UI;

public class MoveTo2DMode : MonoBehaviour {
    [SerializeField] GameObject _canvas3D;
    [SerializeField] GameObject _canvas2D;
    [SerializeField] InputField _coordField;

    private void Awake() {
        if (MainMenuLogic._isChosen2D)
            Activate2DMode();
    }

    public void Activate2DMode() {
        _canvas3D.SetActive(false);
        _canvas2D.SetActive(true);
        _coordField.text = "";
    }
}
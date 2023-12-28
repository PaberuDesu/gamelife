using UnityEngine;
using UnityEngine.UI;

public class MoveTo2DMode : MonoBehaviour {
    [SerializeField] private GameObject _canvas3D;
    [SerializeField] private GameObject _canvas2D;
    [SerializeField] private pregameLogic _3dPregame;

    private void Awake() {
        if (MainMenuLogic._isChosen2D)
            _canvas2D.SetActive(true);
        else _canvas3D.SetActive(true);
    }
    private void Start() {
        if (MainMenuLogic._isChosen2D)
            _3dPregame.enabled = false;
    }
}
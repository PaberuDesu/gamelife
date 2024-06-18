using UnityEngine;

public class MoveTo2DMode : MonoBehaviour {
    [SerializeField] private GameObject canvas3D;
    [SerializeField] private GameObject field3D;

    private void Awake() {
        if (MainMenuLogic._isChosen2D) field3D.SetActive(false);
        else canvas3D.SetActive(true);
    }
}
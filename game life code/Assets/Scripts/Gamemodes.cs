using UnityEngine;

public class Gamemodes : MonoBehaviour
{
    [SerializeField] private moveCharacter Camera_on;
    [SerializeField] private GameObject PregameUI_on;

    public bool cameraEnabled = false;
    public bool gameStarted = false;

    void Update() {
        if ((Input.GetKeyDown(KeyCode.E) && !gameStarted) || (Input.GetKeyDown(KeyCode.Escape) && cameraEnabled))
            ChangeGamemodeCameraAndPregame();
    }

    void ChangeGamemodeCameraAndPregame() {
        cameraEnabled = !cameraEnabled;
        Camera_on.enabled = cameraEnabled;
        PregameUI_on.SetActive(!cameraEnabled);
    }
}
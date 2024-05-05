using UnityEngine;

public class Gamemodes : MonoBehaviour
{
    public moveCharacter Camera_on;
    public GameObject PregameUI_on;

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

    public void ChangeGamemodePregameAndGame(bool value) {gameStarted = value;}
}
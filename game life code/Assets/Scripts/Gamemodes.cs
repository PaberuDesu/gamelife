using UnityEngine;

public class Gamemodes : MonoBehaviour
{
    public moveCharacter Camera_on;
    public GameObject PregameUI_on;

    public int gamemode = 0;
    int GamemodeAmount = 2;

    void Update() {
        if ((Input.GetKeyDown(KeyCode.E) && gamemode < 2) || (Input.GetKeyDown(KeyCode.Escape) && gamemode == 1))
            ChangeGamemodeCameraAndPregame();
    }

    void ChangeGamemodeCameraAndPregame() {
        gamemode = (gamemode + 1) % GamemodeAmount;
        Camera_on.enabled = gamemode == 1;
        PregameUI_on.SetActive(gamemode == 0);
    }

    public void ChangeGamemodePregameAndGame(bool _is_game_starting) {gamemode = _is_game_starting ? 2 : 0;}
}
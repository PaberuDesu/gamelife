using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemodes : MonoBehaviour
{
    public moveCharacter Camera_on;
    public GameObject PregameUI_on;

    public int gamemode = 0;
    int GamemodeAmount = 2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && gamemode < 2)
            ChangeGamemodeCameraAndPregame();
    }

    void ChangeGamemodeCameraAndPregame()
    {
        gamemode = (gamemode + 1) % GamemodeAmount;
        if (gamemode == 0)
        {
            Camera_on.enabled = false;
            PregameUI_on.SetActive(true);
        }
        else if (gamemode == 1)
        {
            PregameUI_on.SetActive(false);
            Camera_on.enabled = true;
        }
    }

    public void ChangeGamemodePregameAndGame(bool _is_game_starting)
    {gamemode = _is_game_starting ? 2 : 1;}
}
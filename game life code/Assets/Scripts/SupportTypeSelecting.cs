using UnityEngine;

public abstract class SupportTypeSelecting : MonoBehaviour {public int SelectedCellType = 1;}

public abstract class Pregame : SupportTypeSelecting {
    [SerializeField] protected gameLogic game;
    protected Roster actions;
    protected bool hotkeyUndo {get {return Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z);}}
    protected bool hotkeyRedo {get {return Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y);}}
    private void OnDisable() {actions = null;}
    private void FixedUpdate() {if (game.playingOneFrame) game.StartGame();}
    public abstract void CutField();
    public abstract void Clear();
    public abstract void AddAction();
}

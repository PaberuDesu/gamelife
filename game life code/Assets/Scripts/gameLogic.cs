using System.Collections;
using UnityEngine;

abstract public class gameLogic : MonoBehaviour {
    private bool continuing;
    protected bool doStopIfStable = true;
    public byte counter = 0;
    public GameObject GameOver;
    [SerializeField] protected GameObject pregameUI;
    [SerializeField] protected GameObject gameUI;
    [SerializeField] protected Settings settings;
    [SerializeField] private Pregame field;
    public bool playingOneFrame{get {return Input.GetKey(KeyCode.P);}}

    private void OnDisable() {field.AddAction();}

    private void Update() {if (Input.GetKeyDown(KeyCode.Escape)) Stop();}

    public void Stop() {continuing = false;}

    public void StartGame() {
        continuing = true;
        if (!playingOneFrame) {
            pregameUI.SetActive(false);
            gameUI.SetActive(true);
        }
        counter = 0;
        SetStart();
        StartCoroutine(GameCycle());
    }

    protected abstract void SetStart();

    public void EndGame() {
        pregameUI.SetActive(true);
        gameUI.SetActive(false);
        doStopIfStable = true;
    }

    protected abstract void SetEnd(bool isContinuing);

    public void Continue() {
        doStopIfStable = false;
        SetEnd(true);
        StartCoroutine(GameCycle());
    }

    protected abstract IEnumerator GameCycle();

    protected void EndIteration(bool isStable) {
        if (!playingOneFrame) StartCoroutine(DesizeContinueOrStop(isStable));
        else if (this is gameLogic2D) ((gameLogic2D) this)._paint._texture.Apply();
    }

    private IEnumerator DesizeContinueOrStop(bool isStable) {
        if (isStable && continuing) {
            SetEnd(false);
            GameOver.SetActive(true);
        } else if (continuing) {
            counter++;
            if (counter == 100) {
                counter = 0;
                if (this is gameLogic2D) ((gameLogic2D)this).RememberedAllCells = GameStatusData.All2DCells;
                else ((gameLogic3D)this).RememberedAllCells = GameStatusData.All3DCells;
            }
            yield return new WaitForSeconds(0.05f / settings.simulationSpeed);
            StartCoroutine(GameCycle());
        } else {
            SetEnd(false);
            EndGame();
        }
    }
}
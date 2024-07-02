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
    protected abstract int dimensions{get;}

    private void OnDisable() {field.AddAction();}

    private void Update() {if (Input.GetKeyDown(KeyCode.Escape)) Stop();}

    public void Stop() {continuing = false;}

    public void StartGame() {
        continuing = true;
        pregameUI.SetActive(false);
        gameUI.SetActive(true);
        counter = 0;
        SetStart();
        GameCycle();
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
        GameCycle();
    }

    protected abstract void GameCycle();

    protected IEnumerator DesizeContinueOrStop(bool isStable) {
        if (isStable && doStopIfStable && continuing) {
            SetEnd(false);
            GameOver.SetActive(true);
        } else if (continuing) {
            counter++;
            if (counter == 100) {
                counter = 0;
                if (dimensions == 2) ((gameLogic2D)this).RememberedAllCells = GameStatusData.All2DCells;
                else ((gameLogic3D)this).RememberedAllCells = GameStatusData.All3DCells;
            }
            yield return new WaitForSeconds(0.1f / settings.simulationSpeed);
            GameCycle();
        } else {
            SetEnd(false);
            EndGame();
        }
    }
}
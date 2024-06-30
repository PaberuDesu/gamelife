using System.Collections;
using UnityEngine;

abstract public class gameLogic : MonoBehaviour {
    protected bool continuing;
    public byte counter = 0;
    public GameObject GameOver;
    [SerializeField] protected GameObject pregameUI;
    [SerializeField] protected GameObject gameUI;
    [SerializeField] protected Settings settings;
    [SerializeField] private Field field;

    private void OnDisable() {field.AddAction();}

    public void Stop() {continuing = false;}
    protected abstract void Update();
    public abstract void StartGame();
    protected abstract IEnumerator GameCycle();
}
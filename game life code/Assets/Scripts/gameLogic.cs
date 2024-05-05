using System.Collections;
using UnityEngine;
using Settings;

namespace game_logic {
    abstract public class gameLogic : MonoBehaviour {
        protected bool continuing;
        public byte counter = 0;
        public GameObject GameOver;
        [SerializeField] protected GameObject pregameUI;
        [SerializeField] protected GameObject gameUI;
        [SerializeField] protected SettingsClass Settings;

        public void Stop() {continuing = false;}
        protected abstract void Update();
        public abstract void StartGame();
        protected abstract IEnumerator GameCycle();
    }
}
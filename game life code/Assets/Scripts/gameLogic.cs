using System.Collections;
using UnityEngine;

namespace game_logic {
    abstract public class gameLogic : MonoBehaviour {
        protected bool continuing;
        public byte counter = 0;
        public GameObject GameOver;
        [SerializeField] protected GameObject pregameUI;
        [SerializeField] protected GameObject gameUI;
        [SerializeField] protected Settings settings;

        public void Stop() {continuing = false;}
        protected abstract void Update();
        public abstract void StartGame();
        protected abstract IEnumerator GameCycle();
    }
}
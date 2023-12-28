using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Settings {
    abstract public class SettingsClass : MonoBehaviour {
        public bool[] BornCondition;
        public bool[] SurviveCondition;
        public bool[] ParasitismCondition;
        public bool[] ParasiteSurviveCondition;
        public bool[] MushroomBornCondition;
        public bool[] MushroomSurviveCondition;
        public bool[] ImitatorBornCondition;
        public bool[] ImitatorSurviveCondition;

        protected abstract Transform _bornConditionChanger{get;}
        protected abstract Transform _surviveConditionChanger{get;}
        protected abstract Image _borderExistsIndicator{get;}
        protected abstract GameObject _settingsPanel{get;}
        protected abstract Slider _speedSlider{get;}

        public bool _isBorderExists = false;
        public float MinimumSimulationSpeed = 0.1f;
        public float SimulationSpeed = 0.1f;

        public int SelectedCellType = 1;
        protected const short step = 1300;
        protected int remain_of_step;
        protected int MaximumAbs;
        protected const byte FPS = 10;

        protected abstract int dimensions{get;}
        private int neighboursCount;

        private void Awake() {
            neighboursCount = Mathf.RoundToInt(Mathf.Pow(3, dimensions));
            BornCondition = new bool[neighboursCount];
            SurviveCondition = new bool[neighboursCount];
            ParasitismCondition = new bool[neighboursCount];
            ParasiteSurviveCondition = new bool[neighboursCount];
            MushroomBornCondition = new bool[neighboursCount];
            MushroomSurviveCondition = new bool[neighboursCount];
            ImitatorBornCondition = new bool[neighboursCount];
            ImitatorSurviveCondition = new bool[neighboursCount];

            MaximumAbs = step * (transform.childCount - 1) / 2;
            remain_of_step = MaximumAbs % step;
            _settingsPanel.SetActive(false);
        }

        public void ChangeModeByButton(int MoveMultiplier) {
            StartCoroutine(ChangeMode(MoveMultiplier));
        }

        private IEnumerator ChangeMode(int MoveMultiplier) {
            if (-MoveMultiplier * transform.localPosition.x != MaximumAbs && Mathf.Abs(transform.localPosition.x % (step)) == remain_of_step) {
                for (byte i = 0; i < FPS; i++) {
                    transform.localPosition += (step * MoveMultiplier / FPS) * Vector3.left;
                    yield return new WaitForSeconds(0.1f / FPS);
                }
                SelectedCellType += MoveMultiplier;
            }
            switch (SelectedCellType) {
                case 1:
                    change_buttons_colors(BornCondition, SurviveCondition);
                    break;
                case 2:
                    change_buttons_colors(ParasitismCondition, ParasiteSurviveCondition);
                    break;
                case 3:
                    change_buttons_colors(MushroomBornCondition, MushroomSurviveCondition);
                    break;
                case 4:
                    change_buttons_colors(ImitatorBornCondition, ImitatorSurviveCondition);
                    break;
            }
        }

        public void change_buttons_colors(bool[] AppearCondition, bool[] DisappearCondition) {
            for (byte i = 0; i < neighboursCount; i++) {
                Image button = _bornConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
                button.color = AppearCondition[i] ? Color.green : Color.red;
            }
            for (byte i = 0; i < neighboursCount; i++) {
                Image button = _surviveConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
                button.color = DisappearCondition[i] ? Color.green : Color.red;
            }
        }

        public void SetBorder() {
            _isBorderExists = !_isBorderExists;
            _borderExistsIndicator.color = _isBorderExists ? Color.green : Color.red;
        }

        public void SetBornCondition(int neighbour_count) {
            Image button = _bornConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
            switch (SelectedCellType) {
                case 1:
                    BornCondition[neighbour_count] = !BornCondition[neighbour_count];
                    button.color = BornCondition[neighbour_count] ? Color.green : Color.red;
                    break;
                case 2:
                    ParasitismCondition[neighbour_count] = !ParasitismCondition[neighbour_count];
                    button.color = ParasitismCondition[neighbour_count] ? Color.green : Color.red;
                    break;
                case 3:
                    MushroomBornCondition[neighbour_count] = !MushroomBornCondition[neighbour_count];
                    button.color = MushroomBornCondition[neighbour_count] ? Color.green : Color.red;
                    break;
                case 4:
                    ImitatorBornCondition[neighbour_count] = !ImitatorBornCondition[neighbour_count];
                    button.color = ImitatorBornCondition[neighbour_count] ? Color.green : Color.red;
                    break;
            }
        }
        public void SetSurviveCondition(int neighbour_count) {
            Image button = _surviveConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
            switch (SelectedCellType) {
                case 1:
                    SurviveCondition[neighbour_count] = !SurviveCondition[neighbour_count];
                    button.color = SurviveCondition[neighbour_count] ? Color.green : Color.red;
                    break;
                case 2:
                    ParasiteSurviveCondition[neighbour_count] = !ParasiteSurviveCondition[neighbour_count];
                    button.color = ParasiteSurviveCondition[neighbour_count] ? Color.green : Color.red;
                    break;
                case 3:
                    MushroomSurviveCondition[neighbour_count] = !MushroomSurviveCondition[neighbour_count];
                    button.color = MushroomSurviveCondition[neighbour_count] ? Color.green : Color.red;
                    break;
                case 4:
                    ImitatorSurviveCondition[neighbour_count] = !ImitatorSurviveCondition[neighbour_count];
                    button.color = ImitatorSurviveCondition[neighbour_count] ? Color.green : Color.red;
                    break;
            }
        }

        public abstract void ChangeScale();

        public void LogScale(Text logger) {
            logger.text = GameStatusData.WrittenSize(dimensions);
        }

        public void SetSpeed() {SimulationSpeed = MinimumSimulationSpeed + (_speedSlider.value * 10);}
    }
}
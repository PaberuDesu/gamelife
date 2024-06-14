using System.Collections;
using UnityEngine;
using UnityEngine.UI;

abstract public class Settings : SupportTypeSelecting {
    public bool[][] BornConditions;
    public bool[][] SurviveConditions;

    [SerializeField] protected Text[] ScaleChangers;

    public Image BorderExistsIndicator;
    public Transform BornConditionChanger;
    public Transform SurviveConditionChanger;
    public Slider SpeedSlider;
    [SerializeField] protected GameObject SettingsPanel;

    public bool _isBorderExists = false;
    public float MinimumSimulationSpeed = 1f;
    public float SimulationSpeed = 1f;
    private const int simulationSpeedModifier = 10;


    protected abstract int dimensions{get;}
    private int neighboursCount;

    private void Awake() {
        neighboursCount = (int) Mathf.Pow(3, dimensions);
        int cellTypeCount = 4;
        BornConditions = new bool[cellTypeCount][];
        for (int i = 0; i < cellTypeCount; i++)
            BornConditions[i] = new bool[neighboursCount];
        SurviveConditions = new bool[cellTypeCount][];
        for (int i = 0; i < cellTypeCount; i++)
            SurviveConditions[i] = new bool[neighboursCount];
        SettingsPanel.SetActive(false);
    }

    public void ChangeButtonsColors() {ChangeButtonsColors(BornConditions[SelectedCellType-1], SurviveConditions[SelectedCellType-1]);}

    public void ChangeButtonsColors(bool[] AppearCondition, bool[] DisappearCondition) {
        for (byte i = 0; i < neighboursCount; i++) {
            Image button = BornConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
            button.color = AppearCondition[i] ? Color.green : Color.red;
        }
        for (byte i = 0; i < neighboursCount; i++) {
            Image button = SurviveConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
            button.color = DisappearCondition[i] ? Color.green : Color.red;
        }
    }

    public void SetBorder() {SetBorder(!_isBorderExists);}

    public void SetBorder(bool borderExistance) {
        _isBorderExists = borderExistance;
        BorderExistsIndicator.color = borderExistance ? Color.green : Color.red;
    }

    public void SetBornCondition(int neighbour_count) {
        Image button = BornConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
        BornConditions[SelectedCellType-1][neighbour_count] = !BornConditions[SelectedCellType-1][neighbour_count];
        button.color = BornConditions[SelectedCellType-1][neighbour_count] ? Color.green : Color.red;
    }

    public void SetSurviveCondition(int neighbour_count) {
        Image button = SurviveConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
        SurviveConditions[SelectedCellType-1][neighbour_count] = !SurviveConditions[SelectedCellType-1][neighbour_count];
        button.color = SurviveConditions[SelectedCellType-1][neighbour_count] ? Color.green : Color.red;
    }

    public abstract void ChangeScale();

    public void LogScale(Text logger) {logger.text = GameStatusData.WrittenSize(dimensions);}

    public void SetSpeed() {SimulationSpeed = MinimumSimulationSpeed + (SpeedSlider.value * simulationSpeedModifier);}

    public void SetSpeed(float simulationSpeed) {
        SimulationSpeed = simulationSpeed;
        SpeedSlider.value = (simulationSpeed - MinimumSimulationSpeed)/simulationSpeedModifier;
    }
}
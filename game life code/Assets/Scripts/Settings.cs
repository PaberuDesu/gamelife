using System.Collections;
using UnityEngine;
using UnityEngine.UI;

abstract public class Settings : SupportTypeSelecting {
    public bool[][] bornConditions;
    public bool[][] surviveConditions;
    public bool _isBorderExists = false;
    public float simulationSpeed = 1f;

    [SerializeField] private Text[] scaleChangers;
    [SerializeField] private Image borderExistsIndicator;
    [SerializeField] private Transform bornConditionChanger;
    [SerializeField] private Transform surviveConditionChanger;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Field field;

    private const float minimumSimulationSpeed = 1f;
    private const int simulationSpeedModifier = 10;


    protected abstract int dimensions{get;}
    protected abstract int[] fieldSize{get;set;}
    private int neighboursCount;

    private void Awake() {
        neighboursCount = (int) Mathf.Pow(3, dimensions);
        int cellTypeCount = 4;
        bornConditions = new bool[cellTypeCount][];
        for (int i = 0; i < cellTypeCount; i++)
            bornConditions[i] = new bool[neighboursCount];
        surviveConditions = new bool[cellTypeCount][];
        for (int i = 0; i < cellTypeCount; i++)
            surviveConditions[i] = new bool[neighboursCount];
        settingsPanel.SetActive(false);
    }

    public void ChangeButtonsColors() {ChangeButtonsColors(bornConditions[SelectedCellType-1], surviveConditions[SelectedCellType-1]);}

    public void ChangeButtonsColors(bool[] AppearCondition, bool[] DisappearCondition) {
        for (byte i = 0; i < neighboursCount; i++) {
            Image button = bornConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
            button.color = AppearCondition[i] ? Color.green : Color.red;
        }
        for (byte i = 0; i < neighboursCount; i++) {
            Image button = surviveConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
            button.color = DisappearCondition[i] ? Color.green : Color.red;
        }
    }

    public void SetBorder() {SetBorder(!_isBorderExists);}

    public void SetBorder(bool borderExistance) {
        _isBorderExists = borderExistance;
        borderExistsIndicator.color = borderExistance ? Color.green : Color.red;
    }

    public void SetBornCondition(int neighbour_count) {
        Image button = bornConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
        bornConditions[SelectedCellType-1][neighbour_count] = !bornConditions[SelectedCellType-1][neighbour_count];
        button.color = bornConditions[SelectedCellType-1][neighbour_count] ? Color.green : Color.red;
    }

    public void SetSurviveCondition(int neighbour_count) {
        Image button = surviveConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
        surviveConditions[SelectedCellType-1][neighbour_count] = !surviveConditions[SelectedCellType-1][neighbour_count];
        button.color = surviveConditions[SelectedCellType-1][neighbour_count] ? Color.green : Color.red;
    }

    public void ChangeScale() {
        int[] sizes = new int[dimensions];
        for (int i = 0; i < dimensions; i++) {
            try {
                sizes[i] = int.Parse(scaleChangers[i].text);
                if (sizes[i] == 0) sizes[i] = fieldSize[i];
            }
            catch {sizes[i] = fieldSize[i];}
        }
        bool _changed = false;
        for (int i = 0; i < dimensions; i++) {
            if (sizes[i] != fieldSize[i]) {
                _changed = true;
                break;
            }
        }
        if (_changed && AllowedSize(sizes)) {
            fieldSize = sizes;
            GameStatusData.CutField(dimensions);
            field.CutField();
        }
    }

    protected abstract bool AllowedSize(int[] sizes);

    public void LogScale(Text logger) {logger.text = GameStatusData.WrittenSize(dimensions);}

    public void SetSpeed() {simulationSpeed = minimumSimulationSpeed + (speedSlider.value * simulationSpeedModifier);}

    public void SetSpeed(float simulationSpeed) {
        this.simulationSpeed = simulationSpeed;
        speedSlider.value = (simulationSpeed - minimumSimulationSpeed)/simulationSpeedModifier;
    }
}
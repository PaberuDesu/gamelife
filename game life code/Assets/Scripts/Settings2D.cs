using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settings2D : MonoBehaviour {
    public bool[] BornCondition = new bool[9];
    public bool[] SurviveCondition = new bool[9];
    public bool[] ParasitismCondition = new bool[9];
    public bool[] ParasiteSurviveCondition = new bool[9];
    public bool[] MushroomBornCondition = new bool[9];
    public bool[] MushroomSurviveCondition = new bool[9];
    public bool[] ImitatorBornCondition = new bool[9];
    public bool[] ImitatorSurviveCondition = new bool[9];

    public bool _isBorderExists = false;
    public Image BorderExistsIndicator;
    public Transform BornConditionChanger;
    public Transform SurviveConditionChanger;
    [SerializeField] private Text XScaleChanger;
    [SerializeField] private Text YScaleChanger;
    public Slider SpeedSlider;

    [SerializeField] private Paint paint;

    public float MinimumSimulationSpeed = 0.1f;
    public float SimulationSpeed = 0.1f;

    public int SelectedCellType = 1;
    const short step = 1300;
    int remain_of_step;
    int MaximumAbs;
    const byte FPS = 10;

    void Awake() {
        MaximumAbs = step * (transform.childCount - 1) / 2;
        remain_of_step = MaximumAbs % step;
    }

    public void ChangeModeByButton(int MoveMultiplier) {
        StartCoroutine(ChangeMode(MoveMultiplier));
    }
    
    IEnumerator ChangeMode(int MoveMultiplier) {
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
        for (byte i = 0; i < 9; i++) {
            Image button = BornConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
            button.color = AppearCondition[i] ? Color.green : Color.red;
        }
        for (byte i = 0; i < 9; i++) {
            Image button = SurviveConditionChanger.GetChild(i).gameObject.GetComponent<Image>();
            button.color = DisappearCondition[i] ? Color.green : Color.red;
        }
    }

    public void SetBorder() {
        _isBorderExists = !_isBorderExists;
        BorderExistsIndicator.color = _isBorderExists ? Color.green : Color.red;
    }

    public void SetBornCondition(int neighbour_count) {
        Image button = BornConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
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
        Image button = SurviveConditionChanger.GetChild(neighbour_count).gameObject.GetComponent<Image>();
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

    public void ChangeScale() {
        int X, Y;
        try {X = int.Parse(XScaleChanger.text);}
        catch {X = GameStatusData.X_size2D;}
        try {Y = int.Parse(YScaleChanger.text);}
        catch {Y = GameStatusData.Y_size2D;}
        if (X != GameStatusData.X_size2D || Y != GameStatusData.Y_size2D) {
            if (X > 0)
                GameStatusData.X_size2D = X;
            if (Y > 0)
                GameStatusData.Y_size2D = Y;
            paint.CutField();
            byte[,] All2DCells = new byte[GameStatusData.X_size2D,GameStatusData.Y_size2D];
            for (byte x = 0; x < GameStatusData.X_size2D; x++) {
                for (byte y = 0; y < GameStatusData.Y_size2D; y++) {
                    try{All2DCells[x,y] = GameStatusData.All2DCells[x,y];}
                    catch{All2DCells[x,y] = 0;}
                }
            }
            GameStatusData.All2DCells = All2DCells;
        }
    }

    public void LogScale(Text logger) {
        logger.text = $"({GameStatusData.X_size2D}; {GameStatusData.Y_size2D})";
    }

    public void SetSpeed() {SimulationSpeed = MinimumSimulationSpeed + (SpeedSlider.value * 10);}
}
using UnityEngine;
using UnityEngine.UI;
using Settings;

public class Settings2D : SettingsClass {
    public Image BorderExistsIndicator;
    public Transform BornConditionChanger;
    public Transform SurviveConditionChanger;
    [SerializeField] private Text[] ScaleChangers;
    public Slider SpeedSlider;

    [SerializeField] private Paint paint;
    [SerializeField] private GameObject SettingsPanel;

    protected override int dimensions {get {
        return 2;}}

    protected override Transform _bornConditionChanger {get {
        return BornConditionChanger;}}

    protected override Transform _surviveConditionChanger {get {
        return SurviveConditionChanger;}}

    protected override Image _borderExistsIndicator {get {
        return BorderExistsIndicator;}}

    protected override GameObject _settingsPanel {get {
        return SettingsPanel;}}

    protected override Slider _speedSlider {get {
            return SpeedSlider;}}

    public override void ChangeScale() {
        int[] sizes = new int[dimensions];
        for (int i = 0; i < dimensions; i++) {
            try {sizes[i] = int.Parse(ScaleChangers[i].text);}
            catch {sizes[i] = GameStatusData.size2D[i];}
        }
        bool _changed = false;
        for (int i = 0; i < dimensions; i++) {
            if (sizes[i] != GameStatusData.size2D[i]) {
                _changed = true;
                break;
            }
        }
        if (_changed) {
            for (int i = 0; i < dimensions; i++) {
                if (sizes[i] > 0)
                    GameStatusData.size2D[i] = sizes[i];
            }
            paint.CutField();
            byte[,] All2DCells = new byte[GameStatusData.size2D[0],GameStatusData.size2D[1]];
            for (byte x = 0; x < GameStatusData.size2D[0]; x++) {
                for (byte y = 0; y < GameStatusData.size2D[1]; y++) {
                    try{All2DCells[x,y] = GameStatusData.All2DCells[x,y];}
                    catch{All2DCells[x,y] = 0;}
                }
            }
            GameStatusData.All2DCells = All2DCells;
        }
    }
}
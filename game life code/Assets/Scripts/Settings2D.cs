using UnityEngine;
using UnityEngine.UI;
using Settings;

public class Settings2D : SettingsClass {
    [SerializeField] private Paint paint;

    protected override int dimensions {get {return 2;}}

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
            byte[,] All2DCells = new byte[GameStatusData.size2D[0],GameStatusData.size2D[1]];
            for (byte x = 0; x < GameStatusData.size2D[0]; x++) {
                for (byte y = 0; y < GameStatusData.size2D[1]; y++) {
                    try{All2DCells[x,y] = GameStatusData.All2DCells[x,y];}
                    catch{All2DCells[x,y] = 0;}
                }
            }
            GameStatusData.All2DCells = All2DCells;
            paint.CutField();
        }
    }
}
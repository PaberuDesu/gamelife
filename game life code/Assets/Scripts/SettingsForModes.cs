using UnityEngine;
using UnityEngine.UI;
using Settings;

public class SettingsForModes : SettingsClass {
    private const int maxArea = 3000;

    protected override int dimensions {get {return 3;}}

    public override void ChangeScale() {
        int[] sizes = new int[dimensions];
        for (int i = 0; i < dimensions; i++) {
            try {sizes[i] = int.Parse(ScaleChangers[i].text);}
            catch {sizes[i] = GameStatusData.size3D[i];}
        }
        bool _changed = false;
        for (int i = 0; i < dimensions; i++) {
            if (sizes[i] != GameStatusData.size3D[i]) {
                _changed = true;
                break;
            }
        }
        if (_changed && sizes[0]*sizes[1]*sizes[2] <= maxArea) {
            for (int i = 0; i < dimensions; i++) {
                if (sizes[i] > 0)
                    GameStatusData.size3D[i] = sizes[i];
            }
            pregameLogic.CutField();
            byte[,,] AllCells = new byte[GameStatusData.size3D[0],GameStatusData.size3D[1],GameStatusData.size3D[2]];
            for (byte x = 0; x < GameStatusData.size3D[0]; x++) {
                for (byte y = 0; y < GameStatusData.size3D[1]; y++) {
                    for (byte z = 0; z < GameStatusData.size3D[2]; z++) {
                        try{AllCells[x,y,z] = GameStatusData.AllCells[x,y,z];}
                        catch{AllCells[x,y,z] = 0;}
                    }
                }
            }
            GameStatusData.AllCells = AllCells;
        }
    }
}
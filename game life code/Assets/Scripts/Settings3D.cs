using UnityEngine;

public class Settings3D : Settings {
    protected override int dimensions {get {return 3;}}
    protected override int[] fieldSize{get {return GameStatusData.size3D;} set {GameStatusData.size3D = value;}}
    protected override bool AllowedSize(int[] sizes) {return sizes[0]*sizes[1]*sizes[2] < 3000;}
}
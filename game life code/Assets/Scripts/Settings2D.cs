using UnityEngine;
using UnityEngine.UI;

public class Settings2D : Settings {
    protected override int dimensions {get {return 2;}}
    protected override int[] fieldSize{get {return GameStatusData.size2D;}set {GameStatusData.size2D = value;}}
    protected override bool AllowedSize(int[] sizes) {return true;}
}
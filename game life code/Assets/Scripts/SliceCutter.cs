using UnityEngine;
using UnityEngine.UI;

public class SliceCutter : MonoBehaviour {
    public Text Slice;
    public static int AxisNumber = 0;
    public static string AxisName = "X";
    private static string[] Axises = {"X", "Y", "Z"};
    public static int Coordinate = 0;
    [SerializeField] private Transform visualizer = null;

    public void OnEnable() {
        AxisNumber = 0;
        Coordinate = 0;
        ChangeAxis();
    }

    private void ReplaceText() {
        Slice.text = $"Координата среза: {AxisName+Coordinate}";

        if (visualizer == null) return;
        Vector3 scale = new Vector3(GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]);
        scale[AxisNumber] = 1;
        visualizer.localScale = scale;
        Vector3 position = (scale - Vector3.one)/2;
        position[AxisNumber] = Coordinate;
        visualizer.position = position;
    }

    public void ChangeAxis(int axis_number) {
        if (AxisNumber != axis_number) {
            AxisNumber = axis_number;
            ChangeAxis();
            ReplaceText();
            try {
                int size = AxisNumber == 0 ? GameStatusData.size3D[0] : (AxisNumber == 1 ? GameStatusData.size3D[1] : GameStatusData.size3D[2]);
                if (!(Coordinate < size && Coordinate >=0)) {
                    Coordinate = 0;
                    ReplaceText();
                }
            }
            catch{;}
        }
    }

    public void ChangeAxis() {
        for (int _axis = 0; _axis < 3; _axis++)
            transform.GetChild(_axis).gameObject.GetComponent<Image>().color = _axis == AxisNumber ? Color.white : Color.grey;
        AxisName = Axises[AxisNumber];
        ReplaceText();
    }

    public void ChangeCoordinate(string text_coordinate) {
        try {
            int size = AxisNumber == 0 ? GameStatusData.size3D[0] : (AxisNumber == 1 ? GameStatusData.size3D[1] : GameStatusData.size3D[2]);
            int UncheckedCoord = int.Parse(text_coordinate);
            if (UncheckedCoord < size && UncheckedCoord >=0) {
                Coordinate = UncheckedCoord;
                ReplaceText();
            }
        }
        catch{;}
    }
}
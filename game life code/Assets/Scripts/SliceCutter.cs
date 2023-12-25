using UnityEngine;
using UnityEngine.UI;

public class SliceCutter : MonoBehaviour {
    public Text Slice;
    public static int AxisNumber = 0;
    public static string AxisName = "X";
    private static string[] Axises = {"X", "Y", "Z"};
    public static int Coordinate = 0;

    private void Start() {ChangeAxis();}

    private void ReplaceText() {
        Slice.text = $"Координата среза: {AxisName+Coordinate}";
    }

    public void ChangeAxis(int axis_number) {
        if (AxisNumber != axis_number) {
            AxisNumber = axis_number;
            ChangeAxis();
            ReplaceText();
            try {
                int size = AxisNumber == 0 ? GameStatusData.X_size : (AxisNumber == 1 ? GameStatusData.Y_size : GameStatusData.Z_size);
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
            transform.GetChild(_axis).gameObject.GetComponent<Image>().color = Color.grey;
        transform.GetChild(AxisNumber).gameObject.GetComponent<Image>().color = Color.white;
        AxisName = Axises[AxisNumber];
        ReplaceText();
    }

    public void ChangeCoordinate(string text_coordinate) {
        try {
            int size = AxisNumber == 0 ? GameStatusData.X_size : (AxisNumber == 1 ? GameStatusData.Y_size : GameStatusData.Z_size);
            int UncheckedCoord = int.Parse(text_coordinate);
            if (UncheckedCoord < size && UncheckedCoord >=0) {
                Coordinate = UncheckedCoord;
                ReplaceText();
            }
        }
        catch{;}
    }

    public void ChangeText(Text currentSlice) {
        currentSlice.text = $"Координата текущего среза: {AxisName+Coordinate}";
    }
}
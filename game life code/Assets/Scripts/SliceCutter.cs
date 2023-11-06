using UnityEngine;
using UnityEngine.UI;

public class SliceCutter : MonoBehaviour {
    public Text Slice;
    public static int AxisNumber = 0;
    public static string AxisName = "X";
    public static int Coordinate = 0;

    public void ChangeAxis(int axis_number) {
        if (AxisNumber != axis_number) {
            transform.GetChild(AxisNumber).gameObject.GetComponent<Image>().color = Color.grey;
            transform.GetChild(axis_number).gameObject.GetComponent<Image>().color = Color.white;
            AxisNumber = axis_number;
            AxisName = AxisNumber == 0 ? "X" : (AxisNumber == 1 ? "Y" : "Z");
            Slice.text = $"{AxisName+Coordinate}";
            try {
                int size = AxisNumber == 0 ? GameStatusData.X_size : (AxisNumber == 1 ? GameStatusData.Y_size : GameStatusData.Z_size);
                if (!(Coordinate < size && Coordinate >=0)) {
                    Coordinate = 0;
                    Slice.text = $"{AxisName+Coordinate}";
                }
            }
            catch{;}
        }
    }

    public void ChangeAxis() {
        transform.GetChild(AxisNumber).gameObject.GetComponent<Image>().color = Color.white;
        AxisName = AxisNumber == 0 ? "X" : (AxisNumber == 1 ? "Y" : "Z");
        Slice.text = $"{AxisName+Coordinate}";
    }

    public void ChangeCoordinate(string text_coordinate) {
        try {
            int size = AxisNumber == 0 ? GameStatusData.X_size : (AxisNumber == 1 ? GameStatusData.Y_size : GameStatusData.Z_size);
            int UncheckedCoord = int.Parse(text_coordinate);
            if (UncheckedCoord < size && UncheckedCoord >=0) {
                Coordinate = UncheckedCoord;
                Slice.text = $"{AxisName+Coordinate}";
            }
        }
        catch{;}
    }

    public void ChangeText(Text currentSlice) {
        currentSlice.text = $"{AxisName+Coordinate}";
    }

    public void LightOffAxisMark() {
        transform.GetChild(AxisNumber).gameObject.GetComponent<Image>().color = Color.grey;
    }
}
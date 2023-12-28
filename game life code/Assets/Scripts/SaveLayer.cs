using UnityEngine;

public class SaveLayer : MonoBehaviour {
    [SerializeField] Paint paint;

    public void Save() {
        int axis = SliceCutter.AxisNumber;
        int coord = SliceCutter.Coordinate;
        for (int width = 0; width < paint._textureScale[0]; width++) {
            for (int height = 0; height < paint._textureScale[1]; height++) {
                int PixelID = GetID(paint._texture.GetPixel(width, height));
                if (axis == 0 && coord < GameStatusData.size3D[0] && height < GameStatusData.size3D[1] && width < GameStatusData.size3D[2])
                    pregameLogic.Create(coord, height, width, PixelID);
                else if (axis == 1 && width < GameStatusData.size3D[0] && coord < GameStatusData.size3D[1] && height < GameStatusData.size3D[2])
                    pregameLogic.Create(width, coord, height, PixelID);
                else if (axis == 2 && width < GameStatusData.size3D[0] && height < GameStatusData.size3D[1] && coord < GameStatusData.size3D[2])
                    pregameLogic.Create(width, height, coord, PixelID);
            }
        }
    }

    private int GetID(Color _pixelColor) {
        Color _roundedColor = new Color(Round(_pixelColor.r), Round(_pixelColor.g), Round(_pixelColor.b));
        for (int ID = 0; ID < 5; ID++) {
            if (_roundedColor == paint._colors[ID])
                return ID;
        }
        return 0;
    }

    private float Round(float number) {return Mathf.Floor(number * 10)/ 10;}
}
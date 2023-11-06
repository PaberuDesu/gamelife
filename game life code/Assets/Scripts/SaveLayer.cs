using UnityEngine;

public class SaveLayer : MonoBehaviour {
    [SerializeField] Paint paint;

    public void Save() {
        int axis = SliceCutter.AxisNumber;
        int coord = SliceCutter.Coordinate;
        for (int width = 0; width < paint._textureX; width++) {
            for (int height = 0; height < paint._textureY; height++) {
                int PixelID = GetID(paint._texture.GetPixel(width, height));
                if (axis == 0 && coord < GameStatusData.X_size && height < GameStatusData.Y_size && width < GameStatusData.Z_size)
                    pregameLogic.Create(coord, height, width, PixelID);
                else if (axis == 1 && width < GameStatusData.X_size && coord < GameStatusData.Y_size && height < GameStatusData.Z_size)
                    pregameLogic.Create(width, coord, height, PixelID);
                else if (axis == 2 && width < GameStatusData.X_size && height < GameStatusData.Y_size && coord < GameStatusData.Z_size)
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
using UnityEngine;

public class SaveLayer : MonoBehaviour {
    [SerializeField] Paint paint;

    public void Save() {
        int axis = SliceCutter.AxisNumber;
        int coord = SliceCutter.Coordinate;
        for (int x = 0; x < paint._textureScale[0]; x++) {
            for (int y = 0; y < paint._textureScale[1]; y++) {
                int PixelID = GetID(paint._texture.GetPixel(x, y));
                if (axis == 0 && coord < GameStatusData.size3D[0] && y < GameStatusData.size3D[1] && x < GameStatusData.size3D[2])
                    pregameLogic.Create(coord, y, x, PixelID);
                else if (axis == 1 && x < GameStatusData.size3D[0] && coord < GameStatusData.size3D[1] && y < GameStatusData.size3D[2])
                    pregameLogic.Create(x, coord, y, PixelID);
                else if (axis == 2 && x < GameStatusData.size3D[0] && y < GameStatusData.size3D[1] && coord < GameStatusData.size3D[2])
                    pregameLogic.Create(x, y, coord, PixelID);
            }
        }
    }

    private int GetID(Color _pixelColor) {
        float minColorDifference = 0.0035f;
        for (int ID = 1; ID < 5; ID++) {
            if (Vector4.Distance(_pixelColor, paint._colors[ID]) < minColorDifference) return ID;
        }
        return 0;
    }
}
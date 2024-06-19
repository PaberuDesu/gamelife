using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ReloadPhotos : MonoBehaviour {
    [SerializeField] private Image[] Save2DSlots;
    [SerializeField] private Image[] Save3DSlots;
    [SerializeField] private Text[] DateVisualizers;
    [SerializeField] private Image[] Palettes;
    private const int saveSlots = 4;

    private void Start() {ReloadAllPhotos();}

    public void ReloadAllPhotos() {
        for (int slotNumber = 0; slotNumber < saveSlots; slotNumber++) {
            ReloadPhoto3D(slotNumber);
            ReloadPhoto2D(slotNumber);
        }
    }
    
    public void ReloadPhoto2D(int slotNumber) {
        Sprite sprite = ImageSprite(2, slotNumber);
        sprite.texture.filterMode = FilterMode.Point;
        Save2DSlots[slotNumber].sprite = sprite;
        //load pic
        Texture2D texPalette = new Texture2D(5, 1);
        texPalette.LoadImage(File.ReadAllBytes(Application.dataPath + $"/Resources/Palette{slotNumber}.png"));
        sprite = Sprite.Create(texPalette, new Rect(0, 0, texPalette.width, texPalette.height),Vector2.zero);
        sprite.texture.filterMode = FilterMode.Point;
        Palettes[slotNumber].sprite = sprite;
        //load palette
    }

    public void ReloadPhoto3D(int slotNumber) {Save3DSlots[slotNumber].sprite = ImageSprite(3, slotNumber);}

    private Sprite ImageSprite(int dimensions, int slotNumber) {
        Texture2D tex = new Texture2D(10, 10);
        string path = Application.dataPath + $"/Resources/Image{slotNumber}of{dimensions}D.png";
        int dateSlot = slotNumber + (dimensions == 2 ? 0 : saveSlots);
        DateVisualizers[dateSlot].text = (new FileInfo(path)).LastWriteTime.ToString("g");
        tex.LoadImage(File.ReadAllBytes(path));
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),Vector2.zero);
    }
}
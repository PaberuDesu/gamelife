using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveData : MonoBehaviour {
    public Settings3D settings3D;
    public Settings2D settings2D;
    [SerializeField] private Paint pregame2D;
    [SerializeField] private pregameLogic pregame3D;
    [SerializeField] private TakeAPhoto take_a_photo;
    [SerializeField] private ReloadPhotos _photoReloader;


    private void Start() {
        if (MainMenuLogic._isChosen2D) {
            if (MainMenuLogic.data_slot_to_load >= 0)
                Load2DField(MainMenuLogic.data_slot_to_load);
            else GameStatusData.All2DCells = new byte[GameStatusData.size2D[0], GameStatusData.size2D[1]];
        }
        else {
            if (MainMenuLogic.data_slot_to_load >= 0)
                Load3DField(MainMenuLogic.data_slot_to_load);
            else GameStatusData.AllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
        }
    }

    public void Save3DField(int SlotNumber) {
        Field3DData CellData = new Field3DData(new SettingsData(settings3D));
        File.WriteAllText(Application.streamingAssetsPath + $"/SavedData/SavedGameNumber{SlotNumber}.json", JsonUtility.ToJson(CellData));
        SavePhoto(SlotNumber);
    }

    public void Load3DField(int SlotNumber) {
        Field3DData CellData = JsonUtility.FromJson<Field3DData>(File.ReadAllText(Application.streamingAssetsPath + $"/SavedData/SavedGameNumber{SlotNumber}.json"));
        CellData.Apply(settings3D, pregame3D);
        CellData.settings.Apply(settings3D);
    }

    public void Save2DField(int SlotNumber) {
        Field2DData CellData = new Field2DData(new SettingsData(settings2D),  pregame2D);
        File.WriteAllText(Application.streamingAssetsPath + $"/SavedData/Saved2DGameNumber{SlotNumber}.json", JsonUtility.ToJson(CellData));
        SavePhoto2D(SlotNumber);
    }

    public void Load2DField(int SlotNumber) {
        Field2DData CellData = JsonUtility.FromJson<Field2DData>(File.ReadAllText(Application.streamingAssetsPath + $"/SavedData/Saved2DGameNumber{SlotNumber}.json"));
        CellData.Apply(pregame2D);
        CellData.settings.Apply(settings2D);
    }

    public void SavePhoto(int SlotNumber) {
        Texture2D photo = take_a_photo.CamPhoto();
        File.WriteAllBytes(Application.dataPath + $"/Resources/Image{SlotNumber}.png", photo.EncodeToPNG());
        _photoReloader.ReloadPhoto3D(SlotNumber);
    }
    
    public void SavePhoto2D(int SlotNumber) {
        File.WriteAllBytes(Application.dataPath + $"/Resources/Image{SlotNumber}of2D.png", pregame2D._texture.EncodeToPNG());

        int colorNum = 5;
        Texture2D texPalette = new Texture2D(colorNum, 1);
        for (int i=0; i<colorNum; i++)
            texPalette.SetPixel(i, 1, pregame2D._colors[i]);
        File.WriteAllBytes(Application.dataPath + $"/Resources/Palette{SlotNumber}.png", texPalette.EncodeToPNG());
        
        _photoReloader.ReloadPhoto2D(SlotNumber);
    }
}

[System.Serializable] public class Field3DData {
    public List<Cell> AllCells = new List<Cell>();
    public int X_size, Y_size, Z_size;
    public SettingsData settings;
    
    public Field3DData(SettingsData settings_data) {
        settings = settings_data;
        X_size = GameStatusData.size3D[0];
        Y_size = GameStatusData.size3D[1];
        Z_size = GameStatusData.size3D[2];
        for (byte x = 0; x < X_size; x++) {
            for (byte y = 0; y < Y_size; y++) {
                for (byte z = 0; z < Z_size; z++) {
                    if (GameStatusData.AllCells[x,y,z] > 0)
                        AllCells.Add(new Cell(GameStatusData.AllCells[x,y,z], x, y, z));
                }
            }
        }
    }

    public void Apply(Settings3D settings, pregameLogic field) {
        GameStatusData.size3D[0] = X_size;
        GameStatusData.size3D[1] = Y_size;
        GameStatusData.size3D[2] = Z_size;
        settings.FixCamera(X_size, Y_size, Z_size);
        field.Clear();
        foreach (Cell cell in AllCells)
            field.Create(cell.x, cell.y, cell.z, cell.ID);
    }
}

[System.Serializable] public class Field2DData {
    public List<int> AllCells = new List<int>();
    public int X_size, Y_size;
    public SettingsData settings;
    public Color[] palette;
    
    public Field2DData(SettingsData settings_data, Paint field) {
        settings = settings_data;
        X_size = GameStatusData.size2D[0];
        Y_size = GameStatusData.size2D[1];
        palette = field._colors;
        for (byte x = 0; x < X_size; x++) {
            for (byte y = 0; y < Y_size; y++)
                AllCells.Add(GameStatusData.All2DCells[x,y]);
        }
    }

    public void Apply(Paint field) {
        field._colors = palette;
        field.recolorVisualisers();
        GameStatusData.size2D[0] = X_size;
        GameStatusData.size2D[1] = Y_size;
        GameStatusData.All2DCells = new byte[X_size, Y_size];
        field.GetNewSize();
        for (int i = 0; i < X_size * Y_size; i++) {
            int x = (int) i / Y_size, y = (int) i % Y_size;
            GameStatusData.All2DCells[x,y] = Convert.ToByte(AllCells[i]);
            field.PaintToPlay(x, y, GameStatusData.All2DCells[x,y]);
        }
        field._texture.Apply();
    }
}

[System.Serializable] public class SettingsData {
    public List<CellType> CellTypes = new List<CellType>();
    public bool BorderExistance;
    public float SimulationSpeed;

    public SettingsData(Settings settings) {
        for (int i = 0; i < 4; i++)
            CellTypes.Add(new CellType(i+1, GameStatusData.CellNames[i], settings.BornConditions[i], settings.SurviveConditions[i]));
        BorderExistance = settings._isBorderExists;
        SimulationSpeed = settings.SimulationSpeed;
    }

    public void Apply(Settings settings) {
        foreach (CellType cell in CellTypes) {
            for (int i = 0; i < 4; i++) {
                if (cell.name == GameStatusData.CellNames[i]) {
                    settings.BornConditions[i] = cell.BornConditions;
                    settings.SurviveConditions[i] = cell.SurviveConditions;
                    if (settings.SelectedCellType == i+1)
                        settings.ChangeButtonsColors(cell.BornConditions, cell.SurviveConditions);
                }
            }
        }
        settings.SetBorder(BorderExistance);
        settings.SetSpeed(SimulationSpeed);
    }
}

[System.Serializable] public class CellType {
    public int ID;
    public string name;
    public bool[] BornConditions, SurviveConditions;

    public CellType(int ID, string name, bool[] BornConditions, bool[] SurviveConditions) {
        this.ID = ID;
        this.name = name;
        this.BornConditions = BornConditions;
        this.SurviveConditions = SurviveConditions;
    }
}

[System.Serializable] public class Cell {
    public int ID;
    public byte x, y, z;

    public Cell(int ID, byte x, byte y, byte z) {
        this.ID = ID;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
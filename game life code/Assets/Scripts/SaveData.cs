using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveData : MonoBehaviour {
    public SettingsForModes Settings;
    public Settings2D _settings2D;
    [SerializeField] Paint _paint;
    [SerializeField] TakeAPhoto take_a_photo;
    [SerializeField] ReloadPhotos _photoReloader;


    private void Start() {
        if (MainMenuLogic._isChosen2D) {
            if (MainMenuLogic.data_slot_to_load >= 0)
                Load2DField(MainMenuLogic.data_slot_to_load);
            else
                GameStatusData.All2DCells = new byte[GameStatusData.size2D[0], GameStatusData.size2D[1]];
        }
        else {
            if (MainMenuLogic.data_slot_to_load >= 0)
                LoadField(MainMenuLogic.data_slot_to_load);
            else
                GameStatusData.AllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
        }
    }

    public void SaveField(int SlotNumber) {
        FieldData CellData = new FieldData(new SettingsData(Settings));
        File.WriteAllText(Application.streamingAssetsPath + $"/SavedData/SavedGameNumber{SlotNumber}.json", JsonUtility.ToJson(CellData));
        SavePhoto(SlotNumber);
    }

    public void LoadField(int SlotNumber) {
        FieldData CellData = JsonUtility.FromJson<FieldData>(File.ReadAllText(Application.streamingAssetsPath + $"/SavedData/SavedGameNumber{SlotNumber}.json"));
        CellData.Apply(Settings);
        CellData.Settings.Apply(Settings);
    }

    public void Save2DField(int SlotNumber) {
        Field2DData CellData = new Field2DData(new Settings2DData(_settings2D),  _paint);
        File.WriteAllText(Application.streamingAssetsPath + $"/SavedData/Saved2DGameNumber{SlotNumber}.json", JsonUtility.ToJson(CellData));
        SavePhoto2D(SlotNumber);
    }

    public void Load2DField(int SlotNumber) {
        Field2DData CellData = JsonUtility.FromJson<Field2DData>(File.ReadAllText(Application.streamingAssetsPath + $"/SavedData/Saved2DGameNumber{SlotNumber}.json"));
        CellData.Apply(_paint);
        CellData.Settings.Apply(_settings2D);
    }

    public void SavePhoto(int SlotNumber) {
        Texture2D photo = take_a_photo.CamPhoto();
        File.WriteAllBytes(Application.dataPath + $"/Resources/Image{SlotNumber}.png", photo.EncodeToPNG());
        _photoReloader.ReloadPhoto3D(SlotNumber);
    }
    
    public void SavePhoto2D(int SlotNumber) {
        File.WriteAllBytes(Application.dataPath + $"/Resources/Image{SlotNumber}of2D.png", _paint._texture.EncodeToPNG());

        int colorNum = 5;
        Texture2D texPalette = new Texture2D(colorNum, 1);
        for (int i=0; i<colorNum; i++)
            texPalette.SetPixel(i, 1, _paint._colors[i]);
        File.WriteAllBytes(Application.dataPath + $"/Resources/Palette{SlotNumber}.png", texPalette.EncodeToPNG());
        
        _photoReloader.ReloadPhoto2D(SlotNumber);
    }
}

[System.Serializable] public class FieldData {
    public List<Cell> AllCells = new List<Cell>();
    public int X_size, Y_size, Z_size;
    public SettingsData Settings;
    
    public FieldData(SettingsData settings_data) {
        Settings = settings_data;
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

    public void Apply(SettingsForModes settings) {
        GameStatusData.size3D[0] = X_size;
        GameStatusData.size3D[1] = Y_size;
        GameStatusData.size3D[2] = Z_size;
        settings.FixCamera(X_size, Y_size, Z_size);
        pregameLogic.ClearField();
        foreach (Cell cell in AllCells) {
            pregameLogic.Create(cell.x, cell.y, cell.z, cell.ID);
        }
    }
}

[System.Serializable] public class Field2DData {
    public List<int> AllCells = new List<int>();
    public int X_size, Y_size;
    public Settings2DData Settings;
    public Color[] palette;
    
    public Field2DData(Settings2DData settings_data, Paint _paint) {
        Settings = settings_data;
        palette = _paint._colors;
        X_size = GameStatusData.size2D[0];
        Y_size = GameStatusData.size2D[1];
        for (byte x = 0; x < X_size; x++) {
            for (byte y = 0; y < Y_size; y++) {
                AllCells.Add(GameStatusData.All2DCells[x,y]);
            }
        }
    }

    public void Apply(Paint _paint) {
        _paint._colors = palette;
        _paint.recolorVisualisers();
        GameStatusData.size2D[0] = X_size;
        GameStatusData.size2D[1] = Y_size;
        GameStatusData.All2DCells = new byte[X_size, Y_size];
        _paint.GetNewSize();
        for (int i = 0; i < X_size * Y_size; i++) {
            int x = (int) i / Y_size, y = (int) i % Y_size;
            GameStatusData.All2DCells[x,y] = Convert.ToByte(AllCells[i]);
            _paint.PaintToPlay(x, y, GameStatusData.All2DCells[x,y]);
        }
        _paint._texture.Apply();
    }
}

[System.Serializable] public class SettingsData {
    //cell types
    public List<CellType> CellTypes = new List<CellType>();
    //border existance
    public bool BorderExistance;
    //speed of game
    public float SimulationSpeed;

    public SettingsData(SettingsForModes Settings) {
        for (int i = 0; i < 4; i++)
            CellTypes.Add(new CellType(i+1, GameStatusData.CellNames[i], Settings.BornConditions[i], Settings.SurviveConditions[i]));
        BorderExistance = Settings._isBorderExists;
        SimulationSpeed = Settings.SimulationSpeed;
    }

    public void Apply(SettingsForModes Settings) {
        foreach (CellType cell in CellTypes) {
            for (int i = 0; i < 4; i++) {
                if (cell.name == GameStatusData.CellNames[i]) {
                    Settings.BornConditions[i] = cell.BornConditions;
                    Settings.SurviveConditions[i] = cell.SurviveConditions;
                    if (Settings.SelectedCellType == i+1)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                }
            }
        }

        Settings._isBorderExists = BorderExistance;
        Settings.BorderExistsIndicator.color = BorderExistance ? Color.green : Color.red;

        Settings.SimulationSpeed = SimulationSpeed;
        Settings.SpeedSlider.value = (SimulationSpeed - Settings.MinimumSimulationSpeed)/10;
    }
}

[System.Serializable] public class Settings2DData {
    public List<CellType> CellTypes = new List<CellType>();
    public bool BorderExistance;
    public float SimulationSpeed;

    public Settings2DData(Settings2D Settings) {
        for (int i = 0; i < 4; i++)
            CellTypes.Add(new CellType(i+1, GameStatusData.CellNames[i], Settings.BornConditions[i], Settings.SurviveConditions[i]));
        BorderExistance = Settings._isBorderExists;
        SimulationSpeed = Settings.SimulationSpeed;
    }

    public void Apply(Settings2D Settings) {
        foreach (CellType cell in CellTypes) {
            for (int i = 0; i < 4; i++) {
                if (cell.name == GameStatusData.CellNames[i]) {
                    Settings.BornConditions[i] = cell.BornConditions;
                    Settings.SurviveConditions[i] = cell.SurviveConditions;
                    if (Settings.SelectedCellType == i+1)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                }
            }
        }

        Settings._isBorderExists = BorderExistance;
        Settings.BorderExistsIndicator.color = BorderExistance ? Color.green : Color.red;

        Settings.SimulationSpeed = SimulationSpeed;
        Settings.SpeedSlider.value = (SimulationSpeed - Settings.MinimumSimulationSpeed)/10;
    }
}

[System.Serializable] public class CellType {
    public int ID;
    public string name;
    public bool[] BornConditions;
    public bool[] SurviveConditions;

    public CellType(int ID, string name, bool[] BornConditions, bool[] SurviveConditions) {
        this.ID = ID;
        this.name = name;
        this.BornConditions = BornConditions;
        this.SurviveConditions = SurviveConditions;
    }
}

[System.Serializable] public class Cell {
    public int ID;
    public byte x;
    public byte y;
    public byte z;

    public Cell(int ID, byte x, byte y, byte z) {
        this.ID = ID;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
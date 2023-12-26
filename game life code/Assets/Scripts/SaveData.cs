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
                GameStatusData.All2DCells = new byte[GameStatusData.X_size2D, GameStatusData.Y_size2D];
        }
        else {
            if (MainMenuLogic.data_slot_to_load >= 0)
                LoadField(MainMenuLogic.data_slot_to_load);
            else
                GameStatusData.AllCells = new byte[GameStatusData.X_size, GameStatusData.Y_size, GameStatusData.Z_size];
        }
    }

    public void SaveField(int SlotNumber) {
        FieldData CellData = new FieldData(new SettingsData(Settings));
        File.WriteAllText(Application.streamingAssetsPath + $"/SavedData/SavedGameNumber{SlotNumber}.json", JsonUtility.ToJson(CellData));
        SavePhoto(SlotNumber);
    }

    public void LoadField(int SlotNumber) {
        FieldData CellData = JsonUtility.FromJson<FieldData>(File.ReadAllText(Application.streamingAssetsPath + $"/SavedData/SavedGameNumber{SlotNumber}.json"));
        CellData.Apply();
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
        _photoReloader.ReloadPhoto2D(SlotNumber);
    }
}

[System.Serializable] public class FieldData {
    public List<Cell> AllCells = new List<Cell>();
    public int X_size, Y_size, Z_size;
    public SettingsData Settings;
    
    public FieldData(SettingsData settings_data) {
        Settings = settings_data;
        X_size = GameStatusData.X_size;
        Y_size = GameStatusData.Y_size;
        Z_size = GameStatusData.Z_size;
        for (byte x = 0; x < X_size; x++) {
            for (byte y = 0; y < Y_size; y++) {
                for (byte z = 0; z < Z_size; z++) {
                    if (GameStatusData.AllCells[x,y,z] > 0)
                        AllCells.Add(new Cell(GameStatusData.AllCells[x,y,z], x, y, z));
                }
            }
        }
    }

    public void Apply() {
        GameStatusData.X_size = X_size;
        GameStatusData.Y_size = Y_size;
        GameStatusData.Z_size = Z_size;
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
        X_size = GameStatusData.X_size2D;
        Y_size = GameStatusData.Y_size2D;
        for (byte x = 0; x < X_size; x++) {
            for (byte y = 0; y < Y_size; y++) {
                AllCells.Add(GameStatusData.All2DCells[x,y]);
            }
        }
    }

    public void Apply(Paint _paint) {
        _paint._colors = palette;
        _paint.recolorVisualisers();
        GameStatusData.X_size2D = X_size;
        GameStatusData.Y_size2D = Y_size;
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
        //cell types, its born and survive conditions
        CellTypes.Add(new CellType(1, "Common Cell", Settings.BornCondition, Settings.SurviveCondition));
        CellTypes.Add(new CellType(2, "Parasite Cell", Settings.ParasitismCondition, Settings.ParasiteSurviveCondition));
        CellTypes.Add(new CellType(3, "Mushroom Cell", Settings.MushroomBornCondition, Settings.MushroomSurviveCondition));
        CellTypes.Add(new CellType(4, "Imitator Cell", Settings.ImitatorBornCondition, Settings.ImitatorSurviveCondition));
        //border existance
        BorderExistance = Settings._isBorderExists;
        //speed of game
        SimulationSpeed = Settings.SimulationSpeed;
    }

    public void Apply(SettingsForModes Settings) {
        foreach (CellType cell in CellTypes) {
            switch (cell.name) {
                case "Common Cell":
                    Settings.BornCondition = cell.BornConditions;
                    Settings.SurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 1)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
                case "Parasite Cell":
                    Settings.ParasitismCondition = cell.BornConditions;
                    Settings.ParasiteSurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 2)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
                case "Mushroom Cell":
                    Settings.MushroomBornCondition = cell.BornConditions;
                    Settings.MushroomSurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 3)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
                case "Imitator Cell":
                    Settings.ImitatorBornCondition = cell.BornConditions;
                    Settings.ImitatorSurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 4)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
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
    public float SimulationSpeed;//speed of game

    public Settings2DData(Settings2D Settings) {
        //cell types, its born and survive conditions
        CellTypes.Add(new CellType(1, "Common Cell", Settings.BornCondition, Settings.SurviveCondition));
        CellTypes.Add(new CellType(2, "Parasite Cell", Settings.ParasitismCondition, Settings.ParasiteSurviveCondition));
        CellTypes.Add(new CellType(3, "Mushroom Cell", Settings.MushroomBornCondition, Settings.MushroomSurviveCondition));
        CellTypes.Add(new CellType(4, "Imitator Cell", Settings.ImitatorBornCondition, Settings.ImitatorSurviveCondition));
        BorderExistance = Settings._isBorderExists;
        SimulationSpeed = Settings.SimulationSpeed;//speed of game
    }

    public void Apply(Settings2D Settings) {
        foreach (CellType cell in CellTypes) {
            switch (cell.name) {
                case "Common Cell":
                    Settings.BornCondition = cell.BornConditions;
                    Settings.SurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 1)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
                case "Parasite Cell":
                    Settings.ParasitismCondition = cell.BornConditions;
                    Settings.ParasiteSurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 2)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
                case "Mushroom Cell":
                    Settings.MushroomBornCondition = cell.BornConditions;
                    Settings.MushroomSurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 3)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
                case "Imitator Cell":
                    Settings.ImitatorBornCondition = cell.BornConditions;
                    Settings.ImitatorSurviveCondition = cell.SurviveConditions;
                    if (Settings.SelectedCellType == 4)
                        Settings.change_buttons_colors(cell.BornConditions, cell.SurviveConditions);
                    break;
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
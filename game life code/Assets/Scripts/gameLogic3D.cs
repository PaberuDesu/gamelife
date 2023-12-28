using System.Collections;
using UnityEngine;

public class gameLogic3D : MonoBehaviour {
    [SerializeField] SettingsForModes Settings;
    byte[,,] AllCells;
    byte[,,] RememberedAllCells;
    public byte counter = 0;

    public GameObject GameOver;

    public void StartGame() {
        RememberedAllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
        StartCoroutine(GameCycle());
    }

    IEnumerator GameCycle() {
        AllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];

        for (byte x = 0; x < GameStatusData.size3D[0]; x++) {
            for (byte y = 0; y < GameStatusData.size3D[1]; y++) {
                for (byte z = 0; z < GameStatusData.size3D[2]; z++) {
                    AllCells[x,y,z] = GameStatusData.AllCells[x,y,z];
                }
            }
        }
        
        for (byte x = 0; x < GameStatusData.size3D[0]; x++) {
            for (byte y = 0; y < GameStatusData.size3D[1]; y++) {
                for (byte z = 0; z < GameStatusData.size3D[2]; z++) {
                    GameObject New_cell;
                    byte[] neighbour_counter = checkNeighbours(x, y, z);
                    //logic of count neighbours for each cell type
                    int CellsNeighbours = neighbour_counter[1] - neighbour_counter[2] + neighbour_counter[4];
                    int ParasitesNeighbours = neighbour_counter[1] + neighbour_counter[3] + neighbour_counter[4];
                    int MushroomsNeighbours = neighbour_counter[0] + neighbour_counter[3];
                    int ImitatorsNeighbours = neighbour_counter[1] + neighbour_counter[2] + neighbour_counter[3] + neighbour_counter[4];

                    switch (GameStatusData.AllCells[x,y,z]) {
                        case 4:
                            if (!(Settings.ImitatorSurviveCondition[ImitatorsNeighbours])) {
                                Destroy(GameObject.Find($"imitator({x}, {y}, {z})"));
                                if (CellsNeighbours < 0)
                                    CellsNeighbours = 0;
                                if (!(Settings.SurviveCondition[CellsNeighbours])) {
                                    if (!(Settings.ParasiteSurviveCondition[ParasitesNeighbours])) {
                                        if (!(Settings.MushroomSurviveCondition[MushroomsNeighbours]))
                                            AllCells[x,y,z] = 0;
                                        else {
                                            AllCells[x,y,z] = 3;
                                            New_cell = Instantiate(GameStatusData.mushroom, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                            New_cell.name = $"mushroom({x}, {y}, {z})";
                                        }
                                    }
                                    else {
                                        AllCells[x,y,z] = 2;
                                        New_cell = Instantiate(GameStatusData.parasite, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                        New_cell.name = $"parasite({x}, {y}, {z})";
                                    }
                                }
                                else {
                                    AllCells[x,y,z] = 1;
                                    New_cell = Instantiate(GameStatusData.cell, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                    New_cell.name = $"cell({x}, {y}, {z})";
                                }
                            }
                            break;
                        case 3:
                            if (!(Settings.MushroomSurviveCondition[MushroomsNeighbours])) {
                                AllCells[x,y,z] = 0;
                                Destroy(GameObject.Find($"mushroom({x}, {y}, {z})"));
                            }
                            break;
                        case 2:
                            if (!(Settings.ParasiteSurviveCondition[ParasitesNeighbours])) {
                                AllCells[x,y,z] = 0;
                                Destroy(GameObject.Find($"parasite({x}, {y}, {z})"));
                            }
                            break;
                        case 1:
                            if (CellsNeighbours < 0)
                                CellsNeighbours = 0;
                            if (Settings.ParasitismCondition[ParasitesNeighbours]) {
                                AllCells[x,y,z] = 2;
                                Destroy(GameObject.Find($"cell({x}, {y}, {z})"));
                                New_cell = Instantiate(GameStatusData.parasite, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                New_cell.name = $"parasite({x}, {y}, {z})";
                            }
                            else if (!(Settings.SurviveCondition[CellsNeighbours])) {
                                AllCells[x,y,z] = 0;
                                Destroy(GameObject.Find($"cell({x}, {y}, {z})"));
                            }
                            break;
                        case 0:
                            if (CellsNeighbours < 0)
                                CellsNeighbours = 0;
                            if (Settings.BornCondition[CellsNeighbours]) {
                                AllCells[x,y,z] = 1;
                                New_cell = Instantiate(GameStatusData.cell, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                New_cell.name = $"cell({x}, {y}, {z})";
                            }
                            else if (Settings.MushroomBornCondition[MushroomsNeighbours]) {
                                AllCells[x,y,z] = 3;
                                New_cell = Instantiate(GameStatusData.mushroom, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                New_cell.name = $"mushroom({x}, {y}, {z})";
                            }
                            else if (Settings.ImitatorBornCondition[ImitatorsNeighbours]) {
                                AllCells[x,y,z] = 4;
                                New_cell = Instantiate(GameStatusData.imitator, new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
                                New_cell.name = $"imitator({x}, {y}, {z})";
                            }
                            break;
                    }
                }
            }
        }

        bool EqualityShort = true, EqualityLong = true;
        try {
            for (byte x = 0; x < GameStatusData.size3D[0]; x++) {
                for (byte y = 0; y < GameStatusData.size3D[1]; y++) {
                    for (byte z = 0; z < GameStatusData.size3D[2]; z++) {
                        if (!(GameStatusData.AllCells[x,y,z] == AllCells[x,y,z]))
                            EqualityShort = false;
                        if (!(RememberedAllCells[x,y,z] == AllCells[x,y,z]))
                            EqualityLong = false;
                        if (!EqualityLong && !EqualityShort)
                            throw new System.Exception();
                    }
                } 
            }
            GameOver.SetActive(true);
            StopCoroutine(GameCycle());
        }
        catch{;}
        GameStatusData.AllCells = AllCells;
        counter++;
        if (counter == 100) {
            counter = 0;
            RememberedAllCells = AllCells;
        }
        yield return new WaitForSeconds(0.1f / Settings.SimulationSpeed);
        StartCoroutine(GameCycle());
    }

    byte[] checkNeighbours(byte x, byte y, byte z) {
        byte[] neighbour_counter = new byte[5];
        for (sbyte x_neigbourhood = -1; x_neigbourhood <= 1; x_neigbourhood++) {
            for (sbyte y_neigbourhood = -1; y_neigbourhood <= 1; y_neigbourhood++) {
                for (sbyte z_neigbourhood = -1; z_neigbourhood <= 1; z_neigbourhood++) {
                    if (!(x_neigbourhood == 0 && y_neigbourhood == 0 && z_neigbourhood == 0)) {
                        if (Settings._isBorderExists) {
                            try {neighbour_counter[GameStatusData.AllCells[x + x_neigbourhood, y + y_neigbourhood, z + z_neigbourhood]]++;}
                            catch{};
                        }
                        else
                            neighbour_counter[GameStatusData.AllCells[(x + x_neigbourhood + GameStatusData.size3D[0])%GameStatusData.size3D[0], (y + y_neigbourhood + GameStatusData.size3D[1])%GameStatusData.size3D[1], (z + z_neigbourhood + GameStatusData.size3D[2])%GameStatusData.size3D[2]]]++;
                    }
                }
            }
        }
        return neighbour_counter;
    }
}
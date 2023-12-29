using System.Collections;
using UnityEngine;

public class gameLogic3D : MonoBehaviour {
    [SerializeField] private SettingsForModes Settings;
    byte[,,] AllCells;
    byte[,,] RememberedAllCells;
    public byte counter = 0;

    public GameObject GameOver;

    public void StartGame() {
        RememberedAllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
        StartCoroutine(GameCycle());
    }

    private IEnumerator GameCycle() {
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
                    byte[] neighbour_counter = checkNeighbours(x, y, z);
                    //logic of count neighbours for each cell type
                    int[] CountNeighbours = new int[5];
                    CountNeighbours[0] = 0;
                    CountNeighbours[1] = neighbour_counter[1] - neighbour_counter[2] + neighbour_counter[4];
                    CountNeighbours[2] = neighbour_counter[1] + neighbour_counter[3] + neighbour_counter[4];
                    CountNeighbours[3] = neighbour_counter[0] + neighbour_counter[3];
                    CountNeighbours[4] = neighbour_counter[1] + neighbour_counter[2] + neighbour_counter[3] + neighbour_counter[4];

                    switch (GameStatusData.AllCells[x,y,z]) {
                        case 4:
                            if (Settings.ImitatorSurviveCondition[CountNeighbours[4]]) break;

                            Destroy(GameObject.Find($"{GameStatusData.CellNames[3]}({x}, {y}, {z})"));
                            if (CountNeighbours[1] < 0)
                                CountNeighbours[1] = 0;

                            if (Settings.SurviveCondition[CountNeighbours[1]]) {
                                CreateCell(x,y,z,1);
                                break;
                            }
                            if (Settings.ParasiteSurviveCondition[CountNeighbours[2]]) {
                                CreateCell(x,y,z,2);
                                break;
                            }
                            if (Settings.MushroomSurviveCondition[CountNeighbours[3]]) {
                                CreateCell(x,y,z,3);
                                break;
                            }
                            AllCells[x,y,z] = 0;
                            break;
                        case 3:
                            if (!(Settings.MushroomSurviveCondition[CountNeighbours[3]]))
                                DeleteCell(x,y,z,3);
                            break;
                        case 2:
                            if (!(Settings.ParasiteSurviveCondition[CountNeighbours[2]]))
                                DeleteCell(x,y,z,2);
                            break;
                        case 1:
                            if (CountNeighbours[1] < 0)
                                CountNeighbours[1] = 0;
                            if (Settings.ParasitismCondition[CountNeighbours[2]]) {
                                Destroy(GameObject.Find($"{GameStatusData.CellNames[0]}({x}, {y}, {z})"));
                                CreateCell(x,y,z,2);
                                break;
                            }
                            if (!(Settings.SurviveCondition[CountNeighbours[1]]))
                                DeleteCell(x,y,z,1);
                            break;
                        case 0:
                            if (CountNeighbours[1] < 0)
                                CountNeighbours[1] = 0;
                            if (Settings.BornCondition[CountNeighbours[1]]) {
                                CreateCell(x,y,z,1);
                                break;
                            }
                            if (Settings.MushroomBornCondition[CountNeighbours[3]]) {
                                CreateCell(x,y,z,3);
                                break;
                            }
                            if (Settings.ImitatorBornCondition[CountNeighbours[4]])
                                CreateCell(x,y,z,4);
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

    private byte[] checkNeighbours(byte x, byte y, byte z) {
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

    private void CreateCell(byte x, byte y, byte z, byte type) {
        AllCells[x,y,z] = type;
        GameObject New_cell = Instantiate(GameStatusData.cellTypes[type-1], new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
        New_cell.name = $"{GameStatusData.CellNames[type-1]}({x}, {y}, {z})";
    }

    private void DeleteCell(byte x, byte y, byte z, byte type) {
        AllCells[x,y,z] = 0;
        Destroy(GameObject.Find($"{GameStatusData.CellNames[type-1]}({x}, {y}, {z})"));
    }
}
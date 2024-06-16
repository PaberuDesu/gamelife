using System.Collections;
using UnityEngine;
using game_logic;

public class gameLogic3D : gameLogic {
    private byte[,,] AllCells;
    private byte[,,] RememberedAllCells;

    [SerializeField] private moveCharacter _move;
    [SerializeField] private Gamemodes gamemodes;

    protected override void Update() {if (Input.GetKeyDown(KeyCode.Escape)) {continuing = false;}}

    public override void StartGame() {
        continuing = true;
        pregameUI.SetActive(false);
        gameUI.SetActive(true);
        _move.enabled = true;
        gamemodes.gameStarted = true;
        counter = 0;
        RememberedAllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
        StartCoroutine(GameCycle());
    }

    protected override IEnumerator GameCycle() {
        AllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];

        for (byte x = 0; x < GameStatusData.size3D[0]; x++) {
            for (byte y = 0; y < GameStatusData.size3D[1]; y++) {
                for (byte z = 0; z < GameStatusData.size3D[2]; z++) {
                    AllCells[x,y,z] = GameStatusData.All3DCells[x,y,z];
                }
            }
        }
        
        for (byte x = 0; x < GameStatusData.size3D[0]; x++) {
            for (byte y = 0; y < GameStatusData.size3D[1]; y++) {
                for (byte z = 0; z < GameStatusData.size3D[2]; z++) {
                    byte[] neighbour_counter = checkNeighbours(x, y, z);
                    //logic of count neighbours for each cell type
                    int[] CountNeighbours = new int[4];
                    CountNeighbours[0] = neighbour_counter[1] - neighbour_counter[2] + neighbour_counter[4];
                    CountNeighbours[1] = neighbour_counter[1] + neighbour_counter[3] + neighbour_counter[4];
                    CountNeighbours[2] = neighbour_counter[0] + neighbour_counter[3];
                    CountNeighbours[3] = neighbour_counter[1] + neighbour_counter[2] + neighbour_counter[3] + neighbour_counter[4];

                    switch (GameStatusData.All3DCells[x,y,z]) {
                        case 4:
                            if (settings.SurviveConditions[3][CountNeighbours[3]]) break;

                            Destroy(GameObject.Find($"{GameStatusData.CellNames[3]}({x}, {y}, {z})"));
                            if (CountNeighbours[0] < 0)
                                CountNeighbours[0] = 0;

                            bool created = false;
                            for (byte i = 1; i < 4; i++) {
                                if (settings.SurviveConditions[i-1][CountNeighbours[i-1]]) {
                                    CreateCell(x,y,z,i);
                                    created = true;
                                    break;
                                }
                            }
                            if (!created) AllCells[x,y,z] = 0;
                            break;
                        case 3:
                            if (!(settings.SurviveConditions[2][CountNeighbours[2]]))
                                DeleteCell(x,y,z,3);
                            break;
                        case 2:
                            if (!(settings.SurviveConditions[1][CountNeighbours[1]]))
                                DeleteCell(x,y,z,2);
                            break;
                        case 1:
                            if (CountNeighbours[0] < 0)
                                CountNeighbours[0] = 0;

                            if (settings.BornConditions[1][CountNeighbours[1]]) {
                                Destroy(GameObject.Find($"{GameStatusData.CellNames[0]}({x}, {y}, {z})"));
                                CreateCell(x,y,z,2);
                                break;
                            }
                            if (!(settings.SurviveConditions[0][CountNeighbours[0]]))
                                DeleteCell(x,y,z,1);
                            break;
                        case 0:
                            if (CountNeighbours[0] < 0)
                                CountNeighbours[0] = 0;

                            byte[] bornableTypes = {1,3,4};
                            foreach (byte i in bornableTypes) {
                                if (settings.BornConditions[i-1][CountNeighbours[i-1]]) {
                                    CreateCell(x,y,z,i);
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        bool EqualityShort = true, EqualityLong = true, flag = true;
        for (byte x = 0; flag && x < GameStatusData.size3D[0]; x++) {
            for (byte y = 0; flag && y < GameStatusData.size3D[1]; y++) {
                for (byte z = 0; flag && z < GameStatusData.size3D[2]; z++) {
                    if (!(GameStatusData.All3DCells[x,y,z] == AllCells[x,y,z]))
                        EqualityShort = false;
                    if (!(RememberedAllCells[x,y,z] == AllCells[x,y,z]))
                        EqualityLong = false;
                    if (!EqualityLong && !EqualityShort)
                        flag = false;
                }
            } 
        }
        GameStatusData.All3DCells = AllCells;

        if (flag) {
            _move.enabled = false;
            GameOver.SetActive(true);
        } else if (continuing) {
            counter++;
            if (counter == 100) {
                counter = 0;
                RememberedAllCells = AllCells;
            }
            yield return new WaitForSeconds(0.1f / settings.SimulationSpeed);
            StartCoroutine(GameCycle());
        } else {
            _move.enabled = false;
            pregameUI.SetActive(true);
            gameUI.SetActive(false);
            gamemodes.gameStarted = false;
        }
    }

    private byte[] checkNeighbours(byte x, byte y, byte z) {
        byte[] neighbour_counter = new byte[5];
        for (sbyte x_neigbourhood = -1; x_neigbourhood <= 1; x_neigbourhood++) {
            for (sbyte y_neigbourhood = -1; y_neigbourhood <= 1; y_neigbourhood++) {
                for (sbyte z_neigbourhood = -1; z_neigbourhood <= 1; z_neigbourhood++) {
                    if (!(x_neigbourhood == 0 && y_neigbourhood == 0 && z_neigbourhood == 0)) {
                        if (settings._isBorderExists) {
                            try {neighbour_counter[GameStatusData.All3DCells[x + x_neigbourhood, y + y_neigbourhood, z + z_neigbourhood]]++;}
                            catch{};
                        }
                        else
                            neighbour_counter[GameStatusData.All3DCells[(x + x_neigbourhood + GameStatusData.size3D[0])%GameStatusData.size3D[0], (y + y_neigbourhood + GameStatusData.size3D[1])%GameStatusData.size3D[1], (z + z_neigbourhood + GameStatusData.size3D[2])%GameStatusData.size3D[2]]]++;
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
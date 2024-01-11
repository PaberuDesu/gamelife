using System.Collections;
using UnityEngine;

public class gameLogic2D : MonoBehaviour {
    [SerializeField] private Settings2D Settings;
    [SerializeField] private Paint _paint;
    private byte[,] AllCells;
    private byte[,] RememberedAllCells;

    public byte counter = 0;
    public GameObject GameOver;

    public void StartGame() {
        RememberedAllCells = new byte[GameStatusData.size2D[0], GameStatusData.size2D[1]];
        StartCoroutine(GameCycle());
    }

    private IEnumerator GameCycle() {
        AllCells = new byte[GameStatusData.size2D[0], GameStatusData.size2D[1]];

        for (byte x = 0; x < GameStatusData.size2D[0]; x++) {
            for (byte y = 0; y < GameStatusData.size2D[1]; y++) {
                AllCells[x,y] = GameStatusData.All2DCells[x,y];
            }
        }
        
        for (byte x = 0; x < GameStatusData.size2D[0]; x++) {
            for (byte y = 0; y < GameStatusData.size2D[1]; y++) {
                byte[] neighbour_counter = checkNeighbours(x, y);
                //logic of count neighbours for each cell type
                int[] CountNeighbours = new int[5];
                CountNeighbours[0] = 0;
                CountNeighbours[1] = neighbour_counter[1] - neighbour_counter[2] + neighbour_counter[4];
                CountNeighbours[2] = neighbour_counter[1] + neighbour_counter[3] + neighbour_counter[4];
                CountNeighbours[3] = neighbour_counter[0] + neighbour_counter[3];
                CountNeighbours[4] = neighbour_counter[1] + neighbour_counter[2] + neighbour_counter[3] + neighbour_counter[4];
                switch (GameStatusData.All2DCells[x,y]) {
                    case 4:
                        if (Settings.SurviveConditions[3][CountNeighbours[4]]) break;

                        if (CountNeighbours[1] < 0)
                            CountNeighbours[1] = 0;

                        bool created = false;
                        for (byte i = 1; i < 4; i++) {
                            if (Settings.SurviveConditions[i-1][CountNeighbours[i]]) {
                                CreateCell(x,y,i);
                                created = true;
                                break;
                            }
                        }
                        if (!created) CreateCell(x,y,0);
                        break;
                    case 3:
                        if (!(Settings.SurviveConditions[2][CountNeighbours[3]]))
                            CreateCell(x,y,0);
                        break;
                    case 2:
                        if (!(Settings.SurviveConditions[1][CountNeighbours[2]]))
                            CreateCell(x,y,0);
                        break;
                    case 1:
                        if (CountNeighbours[1] < 0)
                            CountNeighbours[1] = 0;
                        
                        if (Settings.BornConditions[1][CountNeighbours[2]])
                            CreateCell(x,y,2);
                        else if (!(Settings.SurviveConditions[0][CountNeighbours[1]]))
                            CreateCell(x,y,0);
                        break;
                    case 0:
                        if (CountNeighbours[1] < 0)
                            CountNeighbours[1] = 0;
                        
                        byte[] bornableTypes = {1,3,4};
                        foreach (byte i in bornableTypes) {
                            if (Settings.BornConditions[i-1][CountNeighbours[i]]) {
                                CreateCell(x,y,i);
                                break;
                            }
                        }
                        break;
                }
            }
        }

        try {
            bool EqualityShort = true, EqualityLong = true;
            for (byte x = 0; x < GameStatusData.size2D[0]; x++) {
                for (byte y = 0; y < GameStatusData.size2D[1]; y++) {
                    if (!(GameStatusData.All2DCells[x,y] == AllCells[x,y]))
                        EqualityShort = false;
                    if (!(RememberedAllCells[x,y] == AllCells[x,y]))
                        EqualityLong = false;
                    if (!EqualityLong && !EqualityShort)
                        throw new System.Exception();
                } 
            }
            GameOver.SetActive(true);
            StopCoroutine(GameCycle());
        }
        catch{;}
        GameStatusData.All2DCells = AllCells;
        counter++;
        if (counter == 100) {
            counter = 0;
            RememberedAllCells = AllCells;
        }
        _paint._texture.Apply();
        yield return new WaitForSeconds(0.1f / Settings.SimulationSpeed);
        StartCoroutine(GameCycle());
    }

    private byte[] checkNeighbours(byte x, byte y) {
        byte[] neighbour_counter = new byte[5];
        for (sbyte x_neigbourhood = -1; x_neigbourhood <= 1; x_neigbourhood++) {
            for (sbyte y_neigbourhood = -1; y_neigbourhood <= 1; y_neigbourhood++) {
                if (!(x_neigbourhood == 0 && y_neigbourhood == 0)) {
                    if (Settings._isBorderExists) {
                        try {neighbour_counter[GameStatusData.All2DCells[x + x_neigbourhood, y + y_neigbourhood]]++;}
                        catch{;}
                    }
                    else
                        neighbour_counter[GameStatusData.All2DCells[(x + x_neigbourhood + GameStatusData.size2D[0])%GameStatusData.size2D[0], (y + y_neigbourhood + GameStatusData.size2D[1])%GameStatusData.size2D[1]]]++;
                }
            }
        }
        return neighbour_counter;
    }

    private void CreateCell(byte x, byte y, byte type) {
        AllCells[x,y] = type;
        _paint.PaintToPlay(x,y,type);
    }
}
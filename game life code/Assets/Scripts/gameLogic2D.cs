using System.Collections;
using UnityEngine;

public class gameLogic2D : gameLogic {
    [SerializeField] private Paint _paint;
    private byte[,] AllCells;
    private byte[,] RememberedAllCells;

    private bool _isFrameChanged = false;

    protected override void Update() {
        _isFrameChanged = true;
        if (Input.GetKeyDown(KeyCode.Escape)) {continuing = false;}
    }
    
    public override void StartGame() {
        continuing = true;
        pregameUI.SetActive(false);
        gameUI.SetActive(true);
        _paint.SetInGame(true);
        counter = 0;
        RememberedAllCells = new byte[GameStatusData.size2D[0], GameStatusData.size2D[1]];
        StartCoroutine(GameCycle());
    }

    protected override IEnumerator GameCycle() {
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
                int[] CountNeighbours = new int[4];
                CountNeighbours[0] = neighbour_counter[1] - neighbour_counter[2] + neighbour_counter[4];
                CountNeighbours[1] = neighbour_counter[1] + neighbour_counter[3] + neighbour_counter[4];
                CountNeighbours[2] = neighbour_counter[0] + neighbour_counter[3];
                CountNeighbours[3] = neighbour_counter[1] + neighbour_counter[2] + neighbour_counter[3] + neighbour_counter[4];
                switch (GameStatusData.All2DCells[x,y]) {
                    case 4:
                        if (settings.surviveConditions[3][CountNeighbours[3]]) break;

                        if (CountNeighbours[0] < 0)
                            CountNeighbours[0] = 0;

                        bool created = false;
                        for (byte i = 1; i < 4; i++) {
                            if (settings.surviveConditions[i-1][CountNeighbours[i-1]]) {
                                CreateCell(x,y,i);
                                created = true;
                                break;
                            }
                        }
                        if (!created) CreateCell(x,y,0);
                        break;
                    case 3:
                        if (!(settings.surviveConditions[2][CountNeighbours[2]]))
                            CreateCell(x,y,0);
                        break;
                    case 2:
                        if (!(settings.surviveConditions[1][CountNeighbours[1]]))
                            CreateCell(x,y,0);
                        break;
                    case 1:
                        if (CountNeighbours[0] < 0)
                            CountNeighbours[0] = 0;
                        
                        if (settings.bornConditions[1][CountNeighbours[1]])
                            CreateCell(x,y,2);
                        else if (!(settings.surviveConditions[0][CountNeighbours[0]]))
                            CreateCell(x,y,0);
                        break;
                    case 0:
                        if (CountNeighbours[0] < 0)
                            CountNeighbours[0] = 0;
                        
                        byte[] bornableTypes = {1,3,4};
                        foreach (byte i in bornableTypes) {
                            if (settings.bornConditions[i-1][CountNeighbours[i-1]]) {
                                CreateCell(x,y,i);
                                break;
                            }
                        }
                        break;
                }
            }
        }

        if (_isFrameChanged) {
            _paint._texture.Apply();
            _isFrameChanged = false;
        }

        bool EqualityShort = true, EqualityLong = true, flag = true;
        for (byte x = 0; flag && x < GameStatusData.size2D[0]; x++) {
            for (byte y = 0; flag && y < GameStatusData.size2D[1]; y++) {
                if (!(GameStatusData.All2DCells[x,y] == AllCells[x,y]))
                    EqualityShort = false;
                if (!(RememberedAllCells[x,y] == AllCells[x,y]))
                    EqualityLong = false;
                if (!EqualityLong && !EqualityShort)
                    flag = false;
            } 
        }
        GameStatusData.All2DCells = AllCells;
        
        if (flag) {
            _paint._texture.Apply();
            GameOver.SetActive(true);
        } else if (continuing) {
            counter++;
            if (counter == 100) {
                counter = 0;
                RememberedAllCells = AllCells;
            }
            yield return new WaitForSeconds(0.1f / settings.simulationSpeed);
            StartCoroutine(GameCycle());
        } else {
            pregameUI.SetActive(true);
            gameUI.SetActive(false);
            _paint._texture.Apply();
            _paint.SetInGame(false);
        }
    }

    private byte[] checkNeighbours(byte x, byte y) {
        byte[] neighbour_counter = new byte[5];
        for (sbyte x_neigbourhood = -1; x_neigbourhood <= 1; x_neigbourhood++) {
            for (sbyte y_neigbourhood = -1; y_neigbourhood <= 1; y_neigbourhood++) {
                if (!(x_neigbourhood == 0 && y_neigbourhood == 0)) {
                    if (settings._isBorderExists) {
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
using System;
using System.Collections;
using UnityEngine;

public class gameLogic2D : MonoBehaviour {
    [SerializeField] Settings2D Settings;
    [SerializeField] Paint _paint;
    byte[,] All2DCells;
    byte[,] RememberedAll2DCells;
    public byte counter = 0;

    public GameObject GameOver;

    public void StartGame() {
        RememberedAll2DCells = new byte[GameStatusData.X_size2D, GameStatusData.Y_size2D];
        StartCoroutine(GameCycle());
    }

    IEnumerator GameCycle() {
        All2DCells = new byte[GameStatusData.X_size2D, GameStatusData.Y_size2D];

        for (byte x = 0; x < GameStatusData.X_size2D; x++) {
            for (byte y = 0; y < GameStatusData.Y_size2D; y++) {
                All2DCells[x,y] = GameStatusData.All2DCells[x,y];
            }
        }
        
        for (byte x = 0; x < GameStatusData.X_size2D; x++) {
            for (byte y = 0; y < GameStatusData.Y_size2D; y++) {
                byte[] neighbour_counter = checkNeighbours(x, y);
                //logic of count neighbours for each cell type
                int CellsNeighbours = neighbour_counter[1] - neighbour_counter[2] + neighbour_counter[4];
                int ParasitesNeighbours = neighbour_counter[1] + neighbour_counter[3] + neighbour_counter[4];
                int MushroomsNeighbours = neighbour_counter[0] + neighbour_counter[3];
                int ImitatorsNeighbours = neighbour_counter[1] + neighbour_counter[2] + neighbour_counter[3] + neighbour_counter[4];
                switch (GameStatusData.All2DCells[x,y]) {
                    case 4:
                        if (!(Settings.ImitatorSurviveCondition[ImitatorsNeighbours])) {
                            if (CellsNeighbours < 0)
                                CellsNeighbours = 0;
                            if (!(Settings.SurviveCondition[CellsNeighbours])) {
                                if (!(Settings.ParasiteSurviveCondition[ParasitesNeighbours])) {
                                    if (!(Settings.MushroomSurviveCondition[MushroomsNeighbours])) {
                                        All2DCells[x,y] = 0;
                                        _paint.PaintToPlay(x,y,0);
                                    }
                                    else {
                                        All2DCells[x,y] = 3;
                                        _paint.PaintToPlay(x,y,3);
                                    }
                                }
                                else {
                                    All2DCells[x,y] = 2;
                                    _paint.PaintToPlay(x,y,2);
                                }
                            }
                            else {
                                All2DCells[x,y] = 1;
                                _paint.PaintToPlay(x,y,1);
                            }
                        }
                        break;
                    case 3:
                        if (!(Settings.MushroomSurviveCondition[MushroomsNeighbours])) {
                            All2DCells[x,y] = 0;
                            _paint.PaintToPlay(x,y,0);
                        }
                        break;
                    case 2:
                        if (!(Settings.ParasiteSurviveCondition[ParasitesNeighbours])) {
                            All2DCells[x,y] = 0;
                            _paint.PaintToPlay(x,y,0);
                        }
                        break;
                    case 1:
                        if (CellsNeighbours < 0)
                            CellsNeighbours = 0;
                        if (Settings.ParasitismCondition[ParasitesNeighbours]) {
                            All2DCells[x,y] = 2;
                            _paint.PaintToPlay(x,y,2);
                        }
                        else if (!(Settings.SurviveCondition[CellsNeighbours])) {
                            All2DCells[x,y] = 0;
                            _paint.PaintToPlay(x,y,0);
                        }
                        break;
                    case 0:
                        if (CellsNeighbours < 0)
                            CellsNeighbours = 0;
                        if (Settings.BornCondition[CellsNeighbours]) {
                            All2DCells[x,y] = 1;
                            _paint.PaintToPlay(x,y,1);
                        }
                        else if (Settings.MushroomBornCondition[MushroomsNeighbours]) {
                            All2DCells[x,y] = 3;
                            _paint.PaintToPlay(x,y,3);
                        }
                        else if (Settings.ImitatorBornCondition[ImitatorsNeighbours]) {
                            All2DCells[x,y] = 4;
                            _paint.PaintToPlay(x,y,4);
                        }
                        break;
                }
            }
        }

        try {
            for (byte x = 0; x < GameStatusData.X_size2D; x++) {
                for (byte y = 0; y < GameStatusData.Y_size2D; y++) {
                    bool EqualityShort = GameStatusData.All2DCells[x,y] == All2DCells[x,y];
                    bool EqualityLong = RememberedAll2DCells[x,y] == All2DCells[x,y];
                    if (!EqualityLong && !EqualityShort)
                        throw new System.Exception();
                } 
            }
            GameOver.SetActive(true);
            StopCoroutine(GameCycle());
        }
        catch{;}
        GameStatusData.All2DCells = All2DCells;
        counter++;
        if (counter == 100) {
            counter = 0;
            RememberedAll2DCells = All2DCells;
        }
        _paint._texture.Apply();
        yield return new WaitForSeconds(0.1f / Settings.SimulationSpeed);
        StartCoroutine(GameCycle());
    }

    byte[] checkNeighbours(byte x, byte y) {
        byte[] neighbour_counter = new byte[5];
        for (sbyte x_neigbourhood = -1; x_neigbourhood <= 1; x_neigbourhood++) {
            for (sbyte y_neigbourhood = -1; y_neigbourhood <= 1; y_neigbourhood++) {
                if (!(x_neigbourhood == 0 && y_neigbourhood == 0)) {
                    if (Settings._isBorderExists) {
                        try {neighbour_counter[GameStatusData.All2DCells[x + x_neigbourhood, y + y_neigbourhood]]++;}
                        catch{;}
                    }
                    else
                        neighbour_counter[GameStatusData.All2DCells[(x + x_neigbourhood + GameStatusData.X_size2D)%GameStatusData.X_size2D, (y + y_neigbourhood + GameStatusData.Y_size2D)%GameStatusData.Y_size2D]]++;
                }
            }
        }
        return neighbour_counter;
    }
}
using UnityEngine;

public class GameStatusData {
    public static GameObject cell;
    public static GameObject parasite;
    public static GameObject mushroom;
    public static GameObject imitator;
    public static Transform CellsParent;
    public static int X_size = 10, Y_size = 10, Z_size = 10;
    public static byte[,,] AllCells;
    public static int X_size2D = 10, Y_size2D = 10;
    public static byte[,] All2DCells;
}
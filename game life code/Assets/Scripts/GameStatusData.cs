using UnityEngine;

public class GameStatusData : MonoBehaviour {
    [SerializeField] public GameObject[] cellPreinstances;
    [SerializeField] private Transform CellsParentPreinstance;

    public static GameObject[] cellTypes;
    public static Transform CellsParent;
    public static int[] size3D = new int[] {10, 10, 10};
    public static byte[,,] All3DCells;
    public static int[] size2D = new int[] {10, 10};
    public static byte[,] All2DCells;

    public static string[] CellNames = new string[] {"cell", "parasite", "mushroom", "imitator"};

    public static string WrittenSize(int dimensions) {
        if (dimensions == 2) return $"({size2D[0]}; {size2D[1]})";
        if (dimensions == 3) return $"({size3D[0]}; {size3D[1]}; {size3D[2]})";
        return "";
    }

    public static void CutField(int dimensions) {
        if (dimensions == 2) {
            byte[,] AllCells = new byte[size2D[0],size2D[1]];
            for (byte x = 0; x < size2D[0]; x++) {
                for (byte y = 0; y < size2D[1]; y++) {
                    try{AllCells[x,y] = All2DCells[x,y];}
                    catch{AllCells[x,y] = 0;}
                }
            }
            All2DCells = AllCells;
        }
        else {
            byte[,,] AllCells = new byte[size3D[0],size3D[1],size3D[2]];
            for (byte x = 0; x < size3D[0]; x++) {
                for (byte y = 0; y < size3D[1]; y++) {
                    for (byte z = 0; z < size3D[2]; z++) {
                        try{AllCells[x,y,z] = All3DCells[x,y,z];}
                        catch{AllCells[x,y,z] = 0;}
                    }
                }
            }
            All3DCells = AllCells;
        }
    }

    private void Awake() {
        size2D = new int[] {10,10};
        size3D = new int[] {10,10,10};
        cellTypes = cellPreinstances;
        CellsParent = CellsParentPreinstance;
    }
}
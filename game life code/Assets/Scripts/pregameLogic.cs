using System;
using UnityEngine;

public class pregameLogic : MonoBehaviour {
    [field: SerializeField] public GameObject cellPreinstance{get;private set;}
    [field: SerializeField] public GameObject parasitePreinstance{get;private set;}
    [field: SerializeField] public GameObject mushroomPreinstance{get;private set;}
    [field: SerializeField] public GameObject imitatorPreinstance{get;private set;}

    [SerializeField] private Transform CellsParentPreinstance;

    [SerializeField] private MessageCenter message_center;

    public int SelectedCellType = 1;
    private string X_text, Y_text, Z_text;
    
    private bool _isChangingCellInView = false;

    private void Awake() {
        GameStatusData.size3D = new int[] {10,10,10};
        GameStatusData.cellTypes[0] = cellPreinstance;
        GameStatusData.cellTypes[1] = parasitePreinstance;
        GameStatusData.cellTypes[2] = mushroomPreinstance;
        GameStatusData.cellTypes[3] = imitatorPreinstance;
        GameStatusData.CellsParent = CellsParentPreinstance;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Mouse0))
            ChangeCellInView();
    }

    public void Change_x(string input) {X_text = input;}

    public void Change_y(string input) {Y_text = input;}

    public void Change_z(string input) {Z_text = input;}

    private void ChangeCellInView() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Default");
        if (Physics.Raycast(ray, out hit, 35, mask) && !_isChangingCellInView) {
            _isChangingCellInView = true;
            Vector3 coordinates = hit.collider.transform.position;
            if (GameStatusData.AllCells[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z] == SelectedCellType)
                coordinates += hit.normal;
            if (coordinates.x >= 0 && coordinates.x < GameStatusData.size3D[0] && coordinates.y >= 0 && coordinates.y < GameStatusData.size3D[1] && coordinates.z >= 0 && coordinates.z < GameStatusData.size3D[2])
                Create((int) coordinates.x, (int) coordinates.y, (int) coordinates.z);
            Invoke("WaitForChangeNext", 0.1f);
        }
    }

    private void WaitForChangeNext() {_isChangingCellInView = false;}

    public void Create() {
        byte x = 0, y = 0, z = 0;
        try {
            x = Convert.ToByte(X_text);
            y = Convert.ToByte(Y_text);
            z = Convert.ToByte(Z_text);
            if (GameStatusData.AllCells[x,y,z] != SelectedCellType)
                Create(x,y,z);
            else message_center.Message(1, x, y, z);//message: cell is already exist
        }
        catch {
            message_center.Message(2, 0, 0, 0);//message: wrong coordinates
        }
    }

    public void Create(int x, int y, int z) {
        if (SelectedCellType > 0)
            message_center.Message(0, x, y, z);//message: cell has been created
        else message_center.Message(3, x, y, z);//message: cell has been deleted
        Create(x,y,z, SelectedCellType);
    }

    public static void Create(int x, int y, int z, int cell_type) {
        GameObject New_cell;
        if (GameStatusData.AllCells[x,y,z] > 0)
            DeleteCell(x, y, z);
        if (cell_type > 0) {
            New_cell = Instantiate(GameStatusData.cellTypes[cell_type-1], new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
            New_cell.name = $"{GameStatusData.CellNames[cell_type-1]}({x}, {y}, {z})";
        }
        GameStatusData.AllCells[x,y,z] = Convert.ToByte(cell_type);
    }

    static void DeleteCell(int x, int y, int z) {
        if (GameStatusData.AllCells[x,y,z] > 0)
            Destroy(GameObject.Find($"{GameStatusData.CellNames[GameStatusData.AllCells[x,y,z] - 1]}({x}, {y}, {z})"));
    }

    public static void CutField() {
        foreach(Transform child in GameStatusData.CellsParent) {
            if (child.transform.position.x >= GameStatusData.size3D[0] || child.transform.position.y >= GameStatusData.size3D[1] || child.transform.position.z >= GameStatusData.size3D[2])
                Destroy(child.gameObject);
        }
    }

    public static void ClearField() {
        foreach(Transform child in GameStatusData.CellsParent)
            Destroy(child.gameObject);
        GameStatusData.AllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
    }
}
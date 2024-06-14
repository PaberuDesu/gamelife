using System;
using UnityEngine;

public class pregameLogic : Field {
    [SerializeField] private MessageCenter message_center;
    private string[] coordText = new string[3];
    private bool _isChangingCellInView = false;

    private void Update() {if (Input.GetMouseButton(1)) ChangeCellInView(Input.GetKey(KeyCode.LeftControl));}

    public void Change_x(string input) {coordText[0] = input;}

    public void Change_y(string input) {coordText[1] = input;}

    public void Change_z(string input) {coordText[2] = input;}

    private void ChangeCellInView(bool changeOnlyEmptyCells) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 35, LayerMask.GetMask("Default")) && !_isChangingCellInView) {
            _isChangingCellInView = true;
            Vector3 coordinates = hit.collider.transform.position;
            if (GameStatusData.AllCells[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z] == SelectedCellType || changeOnlyEmptyCells)
                coordinates += hit.normal;
            if (coordinates.x >= 0 && coordinates.x < GameStatusData.size3D[0] && coordinates.y >= 0 && coordinates.y < GameStatusData.size3D[1] && coordinates.z >= 0 && coordinates.z < GameStatusData.size3D[2])
                Create((int) coordinates.x, (int) coordinates.y, (int) coordinates.z);
            Invoke("WaitForChangeNext", 0.1f);
        }
    }

    private void WaitForChangeNext() {_isChangingCellInView = false;}

    public void CreateByCoordinates() {
        byte x, y, z;
        try {
            x = Convert.ToByte(coordText[0]);
            y = Convert.ToByte(coordText[1]);
            z = Convert.ToByte(coordText[2]);
            if (GameStatusData.AllCells[x,y,z] != SelectedCellType) Create(x,y,z);
            else message_center.MessageCellExists();
        } catch {message_center.MessageWrongCoordinates();}
    }

    private void Create(int x, int y, int z) {
        Create(x,y,z, SelectedCellType);
        if (SelectedCellType > 0) message_center.MessageCellChanged(true, x, y, z);
        else message_center.MessageCellChanged(false, x, y, z);
    }

    public void Create(int x, int y, int z, int cell_type) {
        if (GameStatusData.AllCells[x,y,z] > 0) DeleteCell(x, y, z);
        if (cell_type > 0) {
            GameObject New_cell = Instantiate(GameStatusData.cellTypes[cell_type-1], new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
            New_cell.name = $"{GameStatusData.CellNames[cell_type-1]}({x}, {y}, {z})";
        }
        GameStatusData.AllCells[x,y,z] = Convert.ToByte(cell_type);
    }

    private static void DeleteCell(int x, int y, int z) {Destroy(GameObject.Find($"{GameStatusData.CellNames[GameStatusData.AllCells[x,y,z] - 1]}({x}, {y}, {z})"));}

    public static void CutField() {
        foreach(Transform child in GameStatusData.CellsParent) {
            if (child.position.x >= GameStatusData.size3D[0] || child.position.y >= GameStatusData.size3D[1] || child.position.z >= GameStatusData.size3D[2])
                Destroy(child.gameObject);
        }
    }

    public override void Clear() {
        foreach(Transform child in GameStatusData.CellsParent) Destroy(child.gameObject);
        GameStatusData.AllCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
    }

    public override void AddAction() {}
}
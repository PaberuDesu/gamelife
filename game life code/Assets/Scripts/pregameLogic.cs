using System;
using UnityEngine;

public class pregameLogic : Field {
    [SerializeField] private Transform camera_area;
    [SerializeField] private MessageCenter message_center;
    private string[] coordText = new string[3];
    private bool _isChangingCellInView = false;
    private bool _isDrawing = false;
    private float createInViewSpeedPref = 1.0f;
    private const float createInViewCooldown = 0.01f;

    private void OnEnable() {FixCamera();}
    private void OnDisable() {actions = null;}

    private void Start() {createInViewSpeedPref = PlayerPrefs.GetFloat("createSpeed");}

    private void Update() {
        if (!_isChangingCellInView) {
            if (Input.GetMouseButton(1)) ChangeCellInView(Input.GetKey(KeyCode.LeftControl));
            else if (_isDrawing) {
                AddAction();
                _isDrawing = false;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)) {SetField(actions.Undo3D());}
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y)) {SetField(actions.Redo3D());}
    }

    private void SetField(byte[,,] statement) {
        if (statement is null) return;
        for (int x = 0; x < GameStatusData.size3D[0]; x++) {
            for (int y = 0; y < GameStatusData.size3D[1]; y++) {
                for (int z = 0; z < GameStatusData.size3D[2]; z++) {
                    byte type = statement[x,y,z];
                    if (type != GameStatusData.All3DCells[x,y,z])
                        Create(x,y,z,type);
                }
            }
        }
    }

    public void Change_x(string input) {coordText[0] = input;}

    public void Change_y(string input) {coordText[1] = input;}

    public void Change_z(string input) {coordText[2] = input;}

    private void ChangeCellInView(bool changeOnlyEmptyCells) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 35, LayerMask.GetMask("Default"))) {
            _isChangingCellInView = true;
            Vector3 coordinates = hit.collider.transform.position;
            if (GameStatusData.All3DCells[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z] == SelectedCellType || changeOnlyEmptyCells)
                coordinates += hit.normal;
            if (coordinates.x >= 0 && coordinates.x < GameStatusData.size3D[0] && coordinates.y >= 0 && coordinates.y < GameStatusData.size3D[1] && coordinates.z >= 0 && coordinates.z < GameStatusData.size3D[2]) {
                Create((int) coordinates.x, (int) coordinates.y, (int) coordinates.z);
                _isDrawing = true;
            }
            Invoke("WaitForChangeNext", createInViewCooldown / createInViewSpeedPref);
        }
    }

    private void WaitForChangeNext() {_isChangingCellInView = false;}

    public void CreateByCoordinates() {
        byte x, y, z;
        try {
            x = Convert.ToByte(coordText[0]);
            y = Convert.ToByte(coordText[1]);
            z = Convert.ToByte(coordText[2]);
            if (GameStatusData.All3DCells[x,y,z] != SelectedCellType) {
                Create(x,y,z);
                AddAction();
            }
            else message_center.MessageCellExists();
        } catch {message_center.MessageWrongCoordinates();}
    }

    private void Create(int x, int y, int z) {
        Create(x,y,z, SelectedCellType);
        if (SelectedCellType > 0) message_center.MessageCellChanged(true, x, y, z);
        else message_center.MessageCellChanged(false, x, y, z);
    }

    public void Create(int x, int y, int z, int cell_type) {
        if (GameStatusData.All3DCells[x,y,z] > 0) DeleteCell(x, y, z);
        if (cell_type > 0) {
            GameObject New_cell = Instantiate(GameStatusData.cellTypes[cell_type-1], new Vector3(x, y, z), Quaternion.identity, GameStatusData.CellsParent);
            New_cell.name = $"{GameStatusData.CellNames[cell_type-1]}({x}, {y}, {z})";
        }
        GameStatusData.All3DCells[x,y,z] = Convert.ToByte(cell_type);
    }

    private static void DeleteCell(int x, int y, int z) {Destroy(GameObject.Find($"{GameStatusData.CellNames[GameStatusData.All3DCells[x,y,z] - 1]}({x}, {y}, {z})"));}

    public override void CutField() {
        FixCamera();
        foreach(Transform child in GameStatusData.CellsParent) {
            if (child.position.x >= GameStatusData.size3D[0] || child.position.y >= GameStatusData.size3D[1] || child.position.z >= GameStatusData.size3D[2])
                Destroy(child.gameObject);
        }
    }

    public override void Clear() {
        foreach(Transform child in GameStatusData.CellsParent) Destroy(child.gameObject);
        GameStatusData.All3DCells = new byte[GameStatusData.size3D[0], GameStatusData.size3D[1], GameStatusData.size3D[2]];
    }

    public void FixCamera() {
        actions = new Roster(GameStatusData.All3DCells);
        int x = GameStatusData.size3D[0];
        int y = GameStatusData.size3D[1];
        int z = GameStatusData.size3D[2];
        int average = (x+y+z)/3;
        camera_area.localScale = new Vector3(Mathf.Max(x, average), Mathf.Max(y, average), Mathf.Max(z, average));
        Transform cam_transform = camera_area.GetChild(0);
        cam_transform.LookAt(new Vector3(x/2, y/2, z/2));
        cam_transform.GetComponent<Camera>().orthographicSize = 2*((x+y+z)/3);

        cam_transform = camera_area.GetChild(1);
        cam_transform.GetComponent<Camera>().orthographicSize = 2*((x+y+z)/3);
    }

    public override void AddAction() {actions.Add(GameStatusData.All3DCells);}

    public void AddAction(int prevCell, int nextCell, byte x, byte y, byte z) {

    }
}
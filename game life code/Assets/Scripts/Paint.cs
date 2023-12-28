using System;
using UnityEngine;
using UnityEngine.UI;

public class Paint : MonoBehaviour {
    private float Zoom = 1;
    private const int minZoom = 1;
    private const int maxZoom = 12;

    private Vector2 ScaleProportion = Vector2.one;
    private Vector2 canvasSize;
    private Vector2 canvasCenter;

    [SerializeField] Transform MovePicButParent;
    [SerializeField] GameObject[] MovePicButObjectList = new GameObject[4];
    private Vector2 MovePicDirection = Vector2.zero;

    private Vector2 screenScale = Vector2.zero;

    public Texture2D _texture;
    [SerializeField] private Material _material;
    public int[] _textureScale;

    [SerializeField] private Image[] PaletteVisualiser;
    [SerializeField] private Transform ColorActivator;
    [SerializeField] private Transform ColorActivator_parentsList;
    public Color[] _colors = {Color.white, Color.green, Color.red, new Color(0.6f, 0.3f, 0, 1), Color.magenta};
    [SerializeField] private int _activeColorNumber = 1;
    
    [SerializeField] private Text CurrentCoordOut;

    [SerializeField] private RectTransform _rectMask;
    [SerializeField] private RectTransform _rect;

    private bool _isOnCanvas = false;
    private bool _isPaintable = false;

    private void Awake() {
        _textureScale = new int[] {GameStatusData.size3D[2],GameStatusData.size3D[1]};
        _texture = new Texture2D(_textureScale[0], _textureScale[1]);
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.filterMode = FilterMode.Point;
        _material.mainTexture = _texture;
        _texture.Apply();

        if (MainMenuLogic._isChosen2D && MainMenuLogic.data_slot_to_load >= 0) {
            GetNewSize();
            _texture.Apply();
        }
        else DrawSlice();
    }

    public void Resize() {
        Zoom = minZoom;
        int x = SliceCutter.AxisNumber == 0 ? GameStatusData.size3D[2] : GameStatusData.size3D[0];
        int y = SliceCutter.AxisNumber == 1 ? GameStatusData.size3D[2] : GameStatusData.size3D[1];
        _textureScale = new int[] {x,y};
        SetNormalSize();
        _texture.Resize(_textureScale[0], _textureScale[1]);
        GameStatusData.size2D = _textureScale;
        GameStatusData.All2DCells = new byte[_textureScale[0], _textureScale[1]];
    }

    public void GetNewSize() {
        _textureScale = GameStatusData.size2D;
        Zoom = minZoom;
        SetNormalSize();
        _texture.Resize(_textureScale[0], _textureScale[1]);
    }

    public void CutField() {
        Zoom = minZoom;
        int[] OldTexture = _textureScale;
        _textureScale = GameStatusData.size2D;
        SetNormalSize();
        _texture.Resize(_textureScale[0], _textureScale[1]);
        int axis = SliceCutter.AxisNumber;
        byte[,] newAll2DCells = new byte[_textureScale[0], _textureScale[1]];
        for (int width = 0; width < _textureScale[0]; width++) {
            for (int height = 0; height < _textureScale[1]; height++) {
                if (width >= OldTexture[0] || height >= OldTexture[1]) {
                    _texture.SetPixel(width, height, _colors[0]);
                    newAll2DCells[width, height] = 0;
                }
                else if (axis >= 0) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.All2DCells[width, height]]);
                    newAll2DCells[width, height] = GameStatusData.All2DCells[width, height];
                }
            }
        }
        _texture.Apply();
        GameStatusData.All2DCells = newAll2DCells;
    }

    private void Update() {
        if (screenScale.x != Screen.width || screenScale.y != Screen.height) {
            canvasSize = _rectMask.anchorMax - _rectMask.anchorMin;
            canvasCenter = (_rectMask.anchorMin + _rectMask.anchorMax) / 2;
            screenScale = new Vector2(Screen.width, Screen.height);
        }
        if (_isOnCanvas) ChangeZoomParameter();
        if (_isPaintable) PaintWithMouse();
        MovePicProcess(_rect.offsetMin + (MovePicDirection * Time.deltaTime));
    }

    public void SetOnOrOutOfCanvas(bool _isOn) {_isOnCanvas = _isOn;}

    public void SetPaintable(bool _isOn) {_isPaintable = _isOn;}

    private void PaintWithMouse() {
        int x = (int) Mathf.Floor((Input.mousePosition.x - (canvasCenter.x * Screen.width) - _rect.offsetMin.x) / ScaleProportion.x / canvasSize.x / Screen.width / Zoom * GameStatusData.size2D[0] + (GameStatusData.size2D[0])/2  + (GameStatusData.size2D[0]) % 2 / 2f);
        int y = (int) Mathf.Floor((Input.mousePosition.y - (canvasCenter.y * Screen.height) - _rect.offsetMin.y) / ScaleProportion.y / canvasSize.y / Screen.height / Zoom * GameStatusData.size2D[1] + (GameStatusData.size2D[1])/2 + (GameStatusData.size2D[1]) % 2 / 2f);
        if (x < 0) x = 0;
        if (x > GameStatusData.size2D[0] - 1) x = GameStatusData.size2D[0] - 1;
        if (y < 0) y = 0;
        if (y > GameStatusData.size2D[1] - 1) y = GameStatusData.size2D[1] - 1;
        CurrentCoordOut.text = $"Координата курсора: ({x};{y})";
        if (Input.GetKey(KeyCode.Mouse0)) {
            _texture.SetPixel(x, y, _colors[_activeColorNumber]);
            GameStatusData.All2DCells[x, y] = Convert.ToByte(_activeColorNumber);
            _texture.Apply();
        }
    }

    public void PaintToPlay(int x, int y, int ColorID) {_texture.SetPixel(x, y, _colors[ColorID]);}

    public void recolor(int type) {
        for (int x = 0; x < _textureScale[0]; x++) {
            for (int y = 0; y < _textureScale[1]; y++) {
                if (GameStatusData.All2DCells[x,y] == type)
                    _texture.SetPixel(x, y, _colors[type]);
            }
        }
        _texture.Apply();
    }

    public void recolorVisualisers() {
        for (int i = 0; i < PaletteVisualiser.Length; i++)
            PaletteVisualiser[i].color = _colors[i];
    }
    
    private void MovePicProcess(Vector2 MoveValue) {
        if (Zoom == minZoom) {
            ActivateCertainButton(0, false);
            ActivateCertainButton(1, false);
            ActivateCertainButton(2, false);
            ActivateCertainButton(3, false);
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
            MovePic(4);
            return;
        }
        Vector2 maskSize = screenScale * canvasSize;
        Vector2 maxDistance = (new Vector2(transform.localScale.x, transform.localScale.y) - Vector2.one) * maskSize / 2;
        if (transform.localScale.x < 1) maxDistance.x = 0;
        if (transform.localScale.y < 1) maxDistance.y = 0;

        if (MoveValue.x < maxDistance.x) ActivateCertainButton(0, MovePicButParent.localScale.x >= 1);
        if (MoveValue.x > -maxDistance.x) ActivateCertainButton(1, MovePicButParent.localScale.x >= 1);
        if (MoveValue.y < maxDistance.y) ActivateCertainButton(2, MovePicButParent.localScale.y >= 1);
        if (MoveValue.y > -maxDistance.y) ActivateCertainButton(3, MovePicButParent.localScale.y >= 1);

        if (MoveValue.x > maxDistance.x) {
            MoveValue.x = maxDistance.x;
            ActivateCertainButton(0, false);
        } else if (MoveValue.x < -maxDistance.x) {
            MoveValue.x = -maxDistance.x;
            ActivateCertainButton(1, false);
        }
        if (MoveValue.y > maxDistance.y) {
            MoveValue.y = maxDistance.y;
            ActivateCertainButton(2, false);
        } else if (MoveValue.y < -maxDistance.y) {
            MoveValue.y = -maxDistance.y;
            ActivateCertainButton(3, false);
        }
        _rect.offsetMin = MoveValue;
        _rect.offsetMax = MoveValue;
    }
    
    public void ChangeActiveColor(int ColorNumber) {
        ColorActivator.SetParent(ColorActivator_parentsList.GetChild(ColorNumber),false);
        _activeColorNumber = ColorNumber;
    }

    public void DrawSlice() {
        Resize();
        int axis = SliceCutter.AxisNumber;
        int coord = SliceCutter.Coordinate;
        for (int width = 0; width < _textureScale[0]; width++) {
            for (int height = 0; height < _textureScale[1]; height++) {
                if (axis == 0) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.AllCells[coord, height, width]]);
                    GameStatusData.All2DCells[width, height] = GameStatusData.AllCells[coord, height, width];
                }
                else if (axis == 1) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.AllCells[width, coord, height]]);
                    GameStatusData.All2DCells[width, height] = GameStatusData.AllCells[width, coord, height];
                }
                else if (axis == 2) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.AllCells[width, height, coord]]);
                    GameStatusData.All2DCells[width, height] = GameStatusData.AllCells[width, height, coord];
                }
            }
        }
        _texture.Apply();
    }

    private void SetNormalSize() {
        float x = GameStatusData.size2D[0];
        float y = GameStatusData.size2D[1];
        if (x < y) ScaleProportion = new Vector2(x/y, 1);
        else ScaleProportion = new Vector2(1, y/x);
        transform.localScale = minZoom * ScaleProportion;
        MovePicButParent.localScale = new Vector3 (
                Mathf.Min(transform.localScale.x, 1),
                Mathf.Min(transform.localScale.y, 1),
                1);
    }

    private void ActivateCertainButton(int number, bool Activity) {MovePicButObjectList[number].SetActive(Activity);}

    private void ChangeZoomParameter() {
        float CurrentZoom = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100;
        float previousZoom = Zoom;
        if (CurrentZoom != 0) {
            if (Zoom + CurrentZoom <= maxZoom && Zoom + CurrentZoom >= minZoom)
                Zoom += CurrentZoom;
            else if (Zoom + CurrentZoom > maxZoom && Zoom < maxZoom)
                Zoom = maxZoom;
            else if (Zoom + CurrentZoom < minZoom && Zoom > minZoom)
                Zoom = minZoom;
            else return;

            if (previousZoom != 1) {
                float MoveValue = (Zoom - 1) / (previousZoom - 1);
                _rect.offsetMin *= MoveValue;
                _rect.offsetMax *= MoveValue;
            }

            transform.localScale = Zoom * ScaleProportion;

            MovePicButParent.localScale = new Vector3 (
                Mathf.Min(transform.localScale.x, 1),
                Mathf.Min(transform.localScale.y, 1),
                1);
            
            Vector3 horButScale = new Vector3 (1/MovePicButParent.localScale.x, 1, 1);
            Vector3 vertButScale = new Vector3 (1, 1/MovePicButParent.localScale.y, 1);

            MovePicButObjectList[0].transform.localScale = horButScale;
            MovePicButObjectList[1].transform.localScale = horButScale;
            MovePicButObjectList[2].transform.localScale = vertButScale;
            MovePicButObjectList[3].transform.localScale = vertButScale;
        }
    }

    public void MovePic(int DirectionID) {
        switch (DirectionID) {
            case 0:
                MovePicDirection = Vector2.left * Screen.width;
                break;
            case 1:
                MovePicDirection = Vector2.right * Screen.width;
                break;
            case 2:
                MovePicDirection = Vector2.down * Screen.width;
                break;
            case 3:
                MovePicDirection = Vector2.up * Screen.width;
                break;
            case 4:
                MovePicDirection = Vector2.zero;
                break;
        }
    }
}
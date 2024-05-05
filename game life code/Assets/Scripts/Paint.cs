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
    [SerializeField] private byte _activeColorNumber = 1;
    
    [SerializeField] private Text CurrentCoordOut;

    [SerializeField] private RectTransform _rectMask;
    [SerializeField] private RectTransform _rect;

    private bool _isOnCanvas = false;
    private bool _isPaintable = false;
    private bool _isInGame = false;
    private bool _isSliderDragged = false;

    [SerializeField] private GameObject canvas2D;

    private void Awake() {
        GameStatusData.size2D = new int[] {10,10};
        _textureScale = GameStatusData.size2D;
        _texture = new Texture2D(_textureScale[0], _textureScale[1]);
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.filterMode = FilterMode.Point;
        _material.mainTexture = _texture;
        _texture.Apply();
        if (!(MainMenuLogic._isChosen2D && MainMenuLogic.data_slot_to_load >= 0)) Clear();
        if (!MainMenuLogic._isChosen2D) canvas2D.SetActive(false);
        canvasSize = _rectMask.anchorMax - _rectMask.anchorMin;
        canvasCenter = (_rectMask.anchorMin + _rectMask.anchorMax) / 2;
        screenScale = new Vector2(Screen.width, Screen.height);
    }

    public void Resize() {
        GameStatusData.size2D = new int[] {
            SliceCutter.AxisNumber == 0 ? GameStatusData.size3D[2] : GameStatusData.size3D[0],
            SliceCutter.AxisNumber == 1 ? GameStatusData.size3D[2] : GameStatusData.size3D[1]
        };
        GetNewSize();
        GameStatusData.All2DCells = new byte[_textureScale[0], _textureScale[1]];
    }

    public void CutField() {
        int[] OldTexture = _textureScale;
        GetNewSize();
        for (int x = 0; x < _textureScale[0]; x++) {
            for (int y = 0; y < _textureScale[1]; y++) {
                if (x >= OldTexture[0] || y >= OldTexture[1]) _texture.SetPixel(x, y, _colors[0]);
                else _texture.SetPixel(x, y, _colors[GameStatusData.All2DCells[x,y]]);
            }
        }
        _texture.Apply();
    }

    private void Update() {
        if (_isOnCanvas && !_isSliderDragged) ChangeZoomParameter();
        if (_isPaintable && !_isSliderDragged && !_isInGame) PaintWithMouse();
        else CurrentCoordOut.text = "";
        MovePicProcess(_rect.offsetMin + (MovePicDirection * Time.deltaTime));
    }

    public void SetOnOrOutOfCanvas(bool _isOn) {_isOnCanvas = _isOn;}
    public void SetPaintable(bool _isOn) {_isPaintable = _isOn;}
    public void SetSliderDragged(bool _isOn) {_isSliderDragged = _isOn;}
    public void SetInGame(bool _isOn) {_isInGame = _isOn;}

    private void PaintWithMouse() {
        int[] coords = new int[2];
        for (int i=0; i<2; i++) {
            coords[i] = (int) Mathf.Floor((Input.mousePosition[i] - (canvasCenter[i] * screenScale[i]) - _rect.offsetMin[i]) / ScaleProportion[i] / canvasSize[i] / screenScale[i] / Zoom * GameStatusData.size2D[i] + (GameStatusData.size2D[i])/2  + (GameStatusData.size2D[i]) % 2 / 2f);
            if (coords[i] < 0) coords[i] = 0;
            if (coords[i] > GameStatusData.size2D[i] - 1) coords[i] = GameStatusData.size2D[i] - 1;
        }
        CurrentCoordOut.text = $"{coords[0]};{coords[1]}";
        if (Input.GetKey(KeyCode.Mouse0)) {
            SetCell(coords[0], coords[1], _activeColorNumber);
            _texture.Apply();
        }
    }

    public void PaintToPlay(int x, int y, int ColorID) {_texture.SetPixel(x, y, _colors[ColorID]);}

    public void recolor(int type) {
        for (int x = 0; x < _textureScale[0]; x++) {
            for (int y = 0; y < _textureScale[1]; y++) {
                if (GameStatusData.All2DCells[x,y] == type) _texture.SetPixel(x, y, _colors[type]);
            }
        }
        _texture.Apply();
    }

    public void recolorVisualisers() {
        for (int i = 0; i < PaletteVisualiser.Length; i++) PaletteVisualiser[i].color = _colors[i];
    }
    
    private void MovePicProcess(Vector2 MoveValue) {
        if (Zoom == minZoom) {
            for (int i = 0; i<4; i++)
                ActivateCertainButton(i, false);
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
            MovePic(Vector2.zero);
            return;
        }
        Vector2 maskSize = screenScale * canvasSize;
        Vector2 maxDistance = (new Vector2(transform.localScale.x-1, transform.localScale.y-1) * maskSize) / 2;

        for (int i = 0; i<2; i++) {
            if (transform.localScale[i] < 1) maxDistance[i] = 0;
            int mv = (int) MoveValue[i]*10;
            int md = (int) maxDistance[i]*10;
            if (mv >= md) {
                if (MovePicDirection[i]>0) MovePic(Vector2.zero);
                if (mv > md) MoveValue[i] = maxDistance[i];
                ActivateCertainButton(2*i, false);
            } else ActivateCertainButton(2*i, MovePicButParent.localScale[i] >= 1);
            if (mv <= -md) {
                if (MovePicDirection[i]<0) MovePic(Vector2.zero);
                if (mv < -md) MoveValue[i] = -maxDistance[i];
                ActivateCertainButton(2*i+1, false);
            } else ActivateCertainButton(2*i+1, MovePicButParent.localScale[i] >= 1);
        }
        _rect.offsetMin = MoveValue;
        _rect.offsetMax = MoveValue;
    }
    
    public void ChangeActiveColor(int ColorNumber) {
        ColorActivator.SetParent(ColorActivator_parentsList.GetChild(ColorNumber),false);
        _activeColorNumber = Convert.ToByte(ColorNumber);
    }

    public void DrawSlice() {
        Resize();
        int axis = SliceCutter.AxisNumber;
        int coord = SliceCutter.Coordinate;
        for (int x = 0; x < _textureScale[0]; x++) {
            for (int y = 0; y < _textureScale[1]; y++) {
                if (axis == 0) SetCell(x, y, GameStatusData.AllCells[coord, y, x]);
                else if (axis == 1) SetCell(x, y, GameStatusData.AllCells[x, coord, y]);
                else SetCell(x, y, GameStatusData.AllCells[x, y, coord]);
            }
        }
        _texture.Apply();
    }

    public void Clear() {
        for (int x = 0; x < _textureScale[0]; x++) {
            for (int y = 0; y < _textureScale[1]; y++) {
                SetCell(x, y, 0);
            }
        }
        _texture.Apply();
    }

    private void SetCell(int x, int y, byte id) {
        _texture.SetPixel(x, y, _colors[id]);
        GameStatusData.All2DCells[x,y] = id;
    }

    public void GetNewSize() {
        _textureScale = GameStatusData.size2D;
        Zoom = minZoom;
        float x = GameStatusData.size2D[0];
        float y = GameStatusData.size2D[1];
        if (x < y) ScaleProportion = new Vector2(x/y, 1);
        else ScaleProportion = new Vector2(1, y/x);
        SetCanvasScale();
        _texture.Resize(_textureScale[0], _textureScale[1]);
    }

    private void SetCanvasScale() {
        transform.localScale = Zoom * ScaleProportion;
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
            SetCanvasScale();
            
            Vector3[] ButScale = new Vector3[]
                {new Vector3(1/MovePicButParent.localScale.x, 1, 1),
                new Vector3 (1, 1/MovePicButParent.localScale.y, 1)};
            for (int i = 0; i<4; i++) {MovePicButObjectList[i].transform.localScale = ButScale[i/2];}
        }
    }

    public void MovePic(int id) {
        switch (id) {
            case 0:
                MovePic(Vector2.left);
                break;
            case 1:
                MovePic(Vector2.right);
                break;
            case 2:
                MovePic(Vector2.down);
                break;
            case 3:
                MovePic(Vector2.up);
                break;
            case 4:
                MovePic(Vector2.zero);
                break;
        }
    }

    private void MovePic(Vector2 Direction) {MovePicDirection = Direction * screenScale.x;}
}
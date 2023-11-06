using System;
using UnityEngine;
using UnityEngine.UI;

public class Paint : MonoBehaviour {
    public int _textureX;
    public int _textureY;
    float Zoom = 1;
    const int minZoom = 1;
    const int maxZoom = 12;
    [SerializeField] Vector2 MovePicDirection;
    [SerializeField] TextureWrapMode _textureWrapMode;
    [SerializeField] FilterMode _filterMode;
    public Texture2D _texture;
    [SerializeField] Material _material;
    [SerializeField] Collider _collider;
    [SerializeField] Camera _camera;
    [SerializeField] Transform ColorActivator;
    [SerializeField] Text CurrentCoordOut;
    [SerializeField] RectTransform MovePicButtons;

    public Color[] _colors = {Color.white, Color.green, Color.red, new Color(0.6f, 0.3f, 0, 1), Color.magenta};
    int _activeColorNumber = 1;

    [SerializeField] GameObject[] MovePicButObjectList = new GameObject[4];
    [SerializeField] RectTransform[] MovePicButtonsList = new RectTransform[4];

    private bool _paintPermission = true;

    void Awake() {
        _textureX = GameStatusData.Z_size;
        _textureY = GameStatusData.Y_size;
        _texture = new Texture2D(_textureX, _textureY);
        _texture.wrapMode =_textureWrapMode;
        _texture.filterMode = _filterMode;
        _material.mainTexture = _texture;
        _texture.Apply();

        if (MainMenuLogic._isChosen2D && MainMenuLogic.data_slot_to_load >= 0) {
            GetNewSize();
            _texture.Apply();
        }
        else {
            _material.mainTextureScale = Vector2.one;
            _material.mainTextureOffset = Vector2.zero;
            DrawSlice();
        }
    }

    public void PaintPermissionSet(bool permission) {_paintPermission = permission;}

    public void Resize() {
        Zoom = minZoom;
        _material.mainTextureScale = Vector2.one;
        _material.mainTextureOffset = Vector2.zero;
        _textureX = SliceCutter.AxisNumber == 0 ? GameStatusData.Z_size : GameStatusData.X_size;
        _textureY = SliceCutter.AxisNumber == 1 ? GameStatusData.Z_size : GameStatusData.Y_size;
        SetNormalSize();
        _texture.Resize(_textureX, _textureY);
        GameStatusData.X_size2D = _textureX;
        GameStatusData.Y_size2D = _textureY;
        GameStatusData.All2DCells = new byte[_textureX, _textureY];
    }

    public void GetNewSize() {
        _textureX = GameStatusData.X_size2D;
        _textureY = GameStatusData.Y_size2D;
        Zoom = minZoom;
        _material.mainTextureScale = Vector2.one;
        _material.mainTextureOffset = Vector2.zero;
        SetNormalSize();
        _texture.Resize(_textureX, _textureY);
    }

    public void CutField() {
        Zoom = minZoom;
        _material.mainTextureScale = Vector2.one;
        _material.mainTextureOffset = Vector2.zero;
        int OldTextureX = _textureX;
        int OldTextureY = _textureY;
        _textureX = GameStatusData.X_size2D;
        _textureY = GameStatusData.Y_size2D;
        SetNormalSize();
        _texture.Resize(_textureX, _textureY);
        int axis = SliceCutter.AxisNumber;
        byte[,] newAll2DCells = new byte[_textureX, _textureY];
        for (int width = 0; width < _textureX; width++) {
            for (int height = 0; height < _textureY; height++) {
                if (width >= OldTextureX || height >= OldTextureY) {
                    _texture.SetPixel(width, height, _colors[0]);
                    newAll2DCells[width, height] = 0;
                }
                else if (axis == 0) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.All2DCells[width, height]]);
                    newAll2DCells[width, height] = GameStatusData.All2DCells[width, height];
                }
                else if (axis == 1) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.All2DCells[width, height]]);
                    newAll2DCells[width, height] = GameStatusData.All2DCells[width, height];
                }
                else if (axis == 2) {
                    _texture.SetPixel(width, height, _colors[GameStatusData.All2DCells[width, height]]);
                    newAll2DCells[width, height] = GameStatusData.All2DCells[width, height];
                }
            }
        }
        _texture.Apply();
        GameStatusData.All2DCells = newAll2DCells;
    }

    void Update() {
        ChangeZoomParameter();
        PaintWithMouse();
        MovePicProcess(_material.mainTextureOffset + (MovePicDirection * Time.deltaTime));
    }

    private void PaintWithMouse() {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (_collider.Raycast(ray, out hit, 100f) && _camera.enabled && _paintPermission) {
            float XScaleCorrection = Mathf.Max(_material.mainTextureScale.x / _material.mainTextureScale.y, 1);
            float YScaleCorrection = Mathf.Max(_material.mainTextureScale.y / _material.mainTextureScale.x, 1);
            int rayX = (int) ((hit.textureCoord.x / Zoom * XScaleCorrection + _material.mainTextureOffset[0]) * _textureX);
            int rayY = (int) ((hit.textureCoord.y / Zoom * YScaleCorrection + _material.mainTextureOffset[1]) * _textureY);
            CurrentCoordOut.text = $"({rayX}; {rayY})";
            if (Input.GetKey(KeyCode.Mouse0)) {
                _texture.SetPixel(rayX, rayY, _colors[_activeColorNumber]);
                GameStatusData.All2DCells[rayX, rayY] = Convert.ToByte(_activeColorNumber);
                _texture.Apply();
            }
        }
    }

    public void PaintToPlay(int x, int y, int ColorID) {_texture.SetPixel(x, y, _colors[ColorID]);}
    
    private void MovePicProcess(Vector2 MoveProgress) {
        Vector2 NewOffset = MoveProgress;
        if (MoveProgress.y <= 0) {
            NewOffset.y = 0;
            ActivateCertainButton(0, false);
        } else ActivateCertainButton(0, true);
        if (MoveProgress.y >= 1 - _material.mainTextureScale.y) {
            NewOffset.y = 1 - _material.mainTextureScale.y;
            ActivateCertainButton(1, false);
        } else ActivateCertainButton(1, true);
        if (MoveProgress.x <= 0) {
            NewOffset.x = 0;
            ActivateCertainButton(2, false);
        } else ActivateCertainButton(2, true);
        if (MoveProgress.x >= 1 - _material.mainTextureScale.x) {
            NewOffset.x = 1 - _material.mainTextureScale.x;
            ActivateCertainButton(3, false);
        } else ActivateCertainButton(3, true);
        _material.mainTextureOffset = NewOffset;
    }
    
    public void ChangeActiveColor(int ColorNumber) {
        ColorActivator.localPosition = Vector3.up * ((-160 * ColorNumber) - 64);
        _activeColorNumber = ColorNumber;
    }

    public void DrawSlice() {
        Resize();
        int axis = SliceCutter.AxisNumber;
        int coord = SliceCutter.Coordinate;
        for (int width = 0; width < _textureX; width++) {
            for (int height = 0; height < _textureY; height++) {
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

    void ButtonsResize() {
        MovePicButtons.sizeDelta = new Vector2(transform.localScale.z, transform.localScale.x) * 852;
        MovePicButtonsList[0].sizeDelta = new Vector2(30, 852 * transform.localScale.x);
        MovePicButtonsList[1].sizeDelta = new Vector2(30, 852 * transform.localScale.x);
        MovePicButtonsList[2].sizeDelta = new Vector2(852 * transform.localScale.z, 30);
        MovePicButtonsList[3].sizeDelta = new Vector2(852 * transform.localScale.z, 30);
    }

    void SetNormalSize() {
        if (_textureX > _textureY)
            transform.localScale = new Vector3(1,1,(float)_textureY/_textureX);
        else
            transform.localScale = new Vector3((float)_textureX/_textureY,1,1);
        ButtonsResize();
    }

    private void ActivateCertainButton(int number, bool Activity) {MovePicButObjectList[number].SetActive(Activity);;}

    void ChangeZoomParameter() {
        float CurrentZoom = -(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100);
        if (CurrentZoom != 0) {
            if (Zoom + CurrentZoom <= maxZoom && Zoom + CurrentZoom >= minZoom) {
                Zoom += CurrentZoom;
                int X = GameStatusData.X_size2D;
                int Y = GameStatusData.Y_size2D;
                float x_resize = (float)X/Y*Zoom;
                float y_resize = (float)Y/X*Zoom;
                if (X > Y) {
                    if (y_resize <= 1)
                        transform.localScale = new Vector3(1,1,y_resize);
                    else if (transform.localScale.z < 1)
                        transform.localScale = new Vector3(1,1,1);
                    float width = transform.localScale.x;
                    float height = transform.localScale.z;
                    if (width != height)
                        _material.mainTextureScale = new Vector2(1/Zoom, 1);
                    else _material.mainTextureScale = new Vector2(1/Zoom, X/Y/Zoom);
                }
                else if (Y > X) {
                    if (x_resize <= 1)
                        transform.localScale = new Vector3(x_resize,1,1);
                    else if (transform.localScale.x < 1)
                        transform.localScale = new Vector3(1,1,1);
                    float width = transform.localScale.x;
                    float height = transform.localScale.z;
                    if (width != height)
                        _material.mainTextureScale = new Vector2(1, 1/Zoom);
                    else _material.mainTextureScale = new Vector2(Y/X/Zoom, 1/Zoom);
                }
                else {
                    if (y_resize <= 1)
                        transform.localScale = new Vector3(x_resize,1,y_resize);
                    _material.mainTextureScale = new Vector2(1/Zoom, 1/Zoom);
                }
            }
            else if (Zoom + CurrentZoom > maxZoom && Zoom < maxZoom) {
                Zoom = maxZoom;
                float XScaleCorrection = Mathf.Max(_material.mainTextureScale.x / _material.mainTextureScale.y, 1);
                float YScaleCorrection = Mathf.Max(_material.mainTextureScale.y / _material.mainTextureScale.x, 1);
                _material.mainTextureScale = new Vector2(XScaleCorrection/Zoom, YScaleCorrection/Zoom);
            }
            else if (Zoom + CurrentZoom < minZoom && Zoom > minZoom) {
                Zoom = minZoom;
                _material.mainTextureScale = Vector2.one;
                SetNormalSize();
            }
            ButtonsResize();
            if (CurrentZoom < 0)
                MovePicProcess(_material.mainTextureOffset);
        }
    }

    public void MovePic(int DirectionID) {
        switch (DirectionID) {
            case 0:
                MovePicDirection = Vector2.up;
                break;
            case 1:
                MovePicDirection = Vector2.down;
                break;
            case 2:
                MovePicDirection = Vector2.right;
                break;
            case 3:
                MovePicDirection = Vector2.left;
                break;
            case 4:
                MovePicDirection = Vector2.zero;
                break;
        }
    }
}
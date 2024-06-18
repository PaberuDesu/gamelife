using UnityEngine;

public class escapeSliceMenu : MonoBehaviour
{
    private static Camera main_camera;
    [SerializeField] private GameObject canvas_2D;
    [SerializeField] private GameObject canvas_3D;
    [SerializeField] private GameObject escape_menu_2D;
    [SerializeField] private GameObject escape_menu_3D;
    [SerializeField] private GameObject canvas;
    private bool _is2D;

    private void Awake() {main_camera = Camera.main;}

    private void Update() {if (Input.GetKeyDown(KeyCode.Escape)) Leave(true);}

    public void SetDimNumAndLeave(bool is2D) {
        SetDimensionNum(is2D);
        Leave(false);
    }

    public void SetDimensionNum(bool is2D) {_is2D = is2D;}

    private void Leave(bool isEscaped) {
        main_camera.enabled = true;
        gameObject.SetActive(false);
        canvas.SetActive(false);
        if (_is2D) {
            canvas_2D.SetActive(true);
            if (isEscaped) escape_menu_2D.SetActive(true);
        } else {
            canvas_3D.SetActive(true);
            if (isEscaped) escape_menu_3D.SetActive(true);
        }
    }
}
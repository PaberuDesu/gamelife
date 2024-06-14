using UnityEngine;

public abstract class SupportTypeSelecting : MonoBehaviour {public int SelectedCellType = 1;}

public abstract class Field : SupportTypeSelecting {
    public abstract void Clear();
    public abstract void AddAction();
}

using UnityEngine;

public class Resize2Dmap : MonoBehaviour
{
    public void OnEnable() {
        float x = GameStatusData.size2D[0];
        float y = GameStatusData.size2D[1];
        Vector2 ScaleProportion;
        if (x < y) ScaleProportion = new Vector2(x/y, 1);
        else ScaleProportion = new Vector2(1, y/x);
        transform.localScale = ScaleProportion;
    }
}
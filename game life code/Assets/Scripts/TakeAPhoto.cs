using System.Collections;
using System.IO;
using UnityEngine;

public class TakeAPhoto : MonoBehaviour {
    private Camera Cam;

    private void Awake() {Cam = GetComponent<Camera>();}

    public Texture2D CamPhoto() {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;
 
        Cam.Render();
 
        Texture2D photo = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Rect rect = new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height);
        photo.ReadPixels(rect, 0, 0);
        photo.Apply();
        RenderTexture.active = currentRT;
        return photo;
    }
}
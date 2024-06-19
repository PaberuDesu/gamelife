using UnityEngine;
using UnityEngine.UI;

public class ControlSettings : MonoBehaviour
{
    private float speed = 1.0f, crs = 1.0f, frs = 1.0f, createSpeed = 1.0f;
    [SerializeField] private Slider speedSlider, crsSlider, frsSlider, createSpeedSlider;

    public void setSpeed(float value) {speed = value;}
    public void setCharacterRotateSensitivity(float value) {crs = value;}
    public void setFieldRotateSensitivity(float value) {frs = value;}
    public void setCellCreatingSpeed(float value) {createSpeed = value;}

    private void OnEnable() {
        speedSlider.value = PlayerPrefs.GetFloat("speed");
        crsSlider.value = PlayerPrefs.GetFloat("crs");
        frsSlider.value = PlayerPrefs.GetFloat("frs");
        createSpeedSlider.value = PlayerPrefs.GetFloat("createSpeed");
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat("speed", speed);
        PlayerPrefs.SetFloat("crs", crs);
        PlayerPrefs.SetFloat("frs", frs);
        PlayerPrefs.SetFloat("createSpeed", createSpeed);
    }
}
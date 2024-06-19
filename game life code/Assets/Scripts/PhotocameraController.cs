using UnityEngine;

public class PhotocameraController : MonoBehaviour
{
    [SerializeField] GameObject photocamera;
    private void OnEnable() {photocamera.SetActive(true);}
    private void OnDisable() {if (photocamera) photocamera.SetActive(false);}
}
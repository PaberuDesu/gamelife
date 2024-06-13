using UnityEngine;

public class suicide : MonoBehaviour
{
    private void Start() {Invoke("Countdown", 1.0f);}
    private void Countdown() {Destroy(gameObject);}
}
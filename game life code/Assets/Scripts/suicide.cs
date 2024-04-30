using System.Collections;
using UnityEngine;

public class suicide : MonoBehaviour
{
    private void Start() {
       StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class suicide : MonoBehaviour
{
    private void Awake() {
       StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
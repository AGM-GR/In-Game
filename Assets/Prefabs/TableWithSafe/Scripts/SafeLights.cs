using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeLights : MonoBehaviour {

    [Header("Lights Renders")]
    public Renderer correctCodeLight;
    public Renderer incorrectCodeLight;

    [Header("Incorrect Code Blink Options")]
    public int blinkTimes = 3;
    public float blinkTime = 0.2f;

    public void Awake() {
        correctCodeLight.material.DisableKeyword("_EMISSION");
        incorrectCodeLight.material.DisableKeyword("_EMISSION");
    }

    public void BlinkRedLight() {
        correctCodeLight.material.DisableKeyword("_EMISSION");
        StartCoroutine(BlinkRed());
    }

    private IEnumerator BlinkRed() {

        for (int i = 0; i < blinkTimes; i++) {
            incorrectCodeLight.material.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(blinkTime);
            incorrectCodeLight.material.DisableKeyword("_EMISSION");
            yield return new WaitForSeconds(blinkTime);
        }
    }

    public void ActivateGreenLight() {
        StopCoroutine(BlinkRed());
        correctCodeLight.material.EnableKeyword("_EMISSION");
        incorrectCodeLight.material.DisableKeyword("_EMISSION");
    }

}

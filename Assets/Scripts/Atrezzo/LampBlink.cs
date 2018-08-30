using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampBlink : MonoBehaviour {

    public List<Light> blinkingLight = new List<Light> ();
    public Renderer blinkingEmission;

    [Header("Start Options")]
    public bool startOn = true;

    [Header("On Time")]
    public float minOn = 3f;
    public float maxOn = 6f;

    [Header("Off Time")]
    public float minOff = 0.6f;
    public float maxOff = 2f;

    private Material blinkingMaterialEmission;

    private void Awake() {
        if (blinkingEmission != null)
            blinkingMaterialEmission = blinkingEmission.material;
    }

    private void Start() {
        if (startOn)
            StartCoroutine(LightBlinkOn());
        else
            StartCoroutine(LightBlinkOff());
    }

    private IEnumerator LightBlinkOn() {

        foreach (Light light in blinkingLight)
            light.enabled = true;
        if (blinkingEmission != null)
            blinkingMaterialEmission.EnableKeyword("_EMISSION");

        yield return new WaitForSeconds(Random.Range(minOn, maxOn));

        StartCoroutine(LightBlinkOff());
    }

    private IEnumerator LightBlinkOff() {

        foreach (Light light in blinkingLight)
            light.enabled = false;
        if (blinkingEmission != null)
            blinkingMaterialEmission.DisableKeyword("_EMISSION");

        yield return new WaitForSeconds(Random.Range(minOff, maxOff));

        StartCoroutine(LightBlinkOn());
    }

}

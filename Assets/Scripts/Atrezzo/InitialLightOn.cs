using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialLightOn : MonoBehaviour {

    public List<Light> lights = new List<Light> ();
    public Renderer emission;

    [Header("Start Options")]
    public float startTime = 4f;

    private Material materialEmission;

    private void Awake() {
        if (emission != null)
            materialEmission = emission.material;
        if (materialEmission != null)
            materialEmission.DisableKeyword("_EMISSION");

        foreach (Light light in lights)
            light.enabled = false;
    }

    private void Start() {
        StartCoroutine(LightOn());
    }

    private IEnumerator LightOn() {

        yield return new WaitForSeconds(startTime);

        foreach (Light light in lights)
            light.enabled = true;
        if (emission != null)
            materialEmission.EnableKeyword("_EMISSION");
    }
}

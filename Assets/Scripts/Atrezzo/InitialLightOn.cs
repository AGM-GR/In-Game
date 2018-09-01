using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialLightOn : MonoBehaviour {

    public List<Light> lights = new List<Light> ();
    public List<GameObject> objects = new List<GameObject>();
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

        foreach (GameObject go in objects)
            go.SetActive(false);
    }

    private void Start() {
        StartCoroutine(LightOn());
    }

    private IEnumerator LightOn() {

        yield return new WaitForSeconds(startTime);

        foreach (GameObject go in objects)
            go.SetActive(true);
        foreach (Light light in lights)
            light.enabled = true;
        if (emission != null)
            materialEmission.EnableKeyword("_EMISSION");
    }

    public void LightOff() {
        foreach (GameObject go in objects)
            go.SetActive(false);
        foreach (Light light in lights)
            light.enabled = false;
        if (emission != null)
            materialEmission.DisableKeyword("_EMISSION");
    }
}

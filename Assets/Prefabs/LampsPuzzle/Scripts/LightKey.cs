using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightKey : MonoBehaviour {

    public LightPuzzleController lightPuzzleController;

    [Header("Light")]
    public Light lightToSwitch;
    public Renderer lightRender;
    public int materialIndex = 0;
    public bool lightOn = false;

    private void Awake() {
        if (lightToSwitch == null)
            lightToSwitch = GetComponentInChildren<Light>();
        if (lightToSwitch != null) {
            lightToSwitch.enabled = lightOn;
        }

        if (lightRender) {
            if (lightOn)
                lightRender.materials[materialIndex].EnableKeyword("_EMISSION");
            else
                lightRender.materials[materialIndex].DisableKeyword("_EMISSION");
        }

        if (lightPuzzleController != null)
            lightPuzzleController.AddLightStatus(this, lightOn);
    }

    public void SwitchLight() {
        lightOn = !lightOn;
        if (lightToSwitch != null) {
            lightToSwitch.enabled = lightOn;
        }

        if (lightRender) {
            if (lightOn)
                lightRender.materials[materialIndex].EnableKeyword("_EMISSION");
            else
                lightRender.materials[materialIndex].DisableKeyword("_EMISSION");
        }

        if (lightPuzzleController != null)
            lightPuzzleController.UpdateLightStatus(this, lightOn);
    }

}

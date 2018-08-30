using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScreenOnCharge : MonoBehaviour {

    public Renderer screenRender;

    public Material offMaterial;
    public Material onMaterial;

    private void OnTriggerEnter(Collider other) {
        if (screenRender != null && other.enabled && other.gameObject.CompareTag("Charger"))
            screenRender.material = onMaterial;
    }

    private void OnTriggerExit(Collider other) {
        if (screenRender != null && other.enabled && other.gameObject.CompareTag("Charger"))
            screenRender.material = offMaterial;
    }

}

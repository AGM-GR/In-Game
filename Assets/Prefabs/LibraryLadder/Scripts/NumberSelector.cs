using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSelector : MonoBehaviour {

    public LibraryCodeController libraryCodeController;

    [SerializeField]
    private int selectedNumber = 1;
    [SerializeField]
    private int codeNumber = 1;

    void Awake() {
        if (libraryCodeController)
            libraryCodeController.AddCodeStatus(this, selectedNumber == codeNumber);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.enabled && other.gameObject.GetComponent<NumberKey>() != null) {
            selectedNumber = other.gameObject.GetComponent<NumberKey>().numberKey;
            libraryCodeController.UpdateCodeStatus(this, selectedNumber == codeNumber);
        }
    }
}

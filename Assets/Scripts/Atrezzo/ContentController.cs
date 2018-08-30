using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentController : MonoBehaviour {

    public bool startContentDeactivated = true;

    public bool contentInChildrens = false;

    public List<GameObject> content = new List<GameObject>();

    private void Awake() {
        if (startContentDeactivated) {
            if (contentInChildrens) {
                for (int i = 0; i < transform.childCount; i++) {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            foreach (GameObject go in content) {
                go.SetActive(false);
            }
        }
    }

    public void ActivateContent() {
        if (contentInChildrens) {
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        foreach (GameObject go in content) {
            go.SetActive(true);
        }
    }

    public void DeactivateContent() {
        if (contentInChildrens) {
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        foreach (GameObject go in content) {
            go.SetActive(false);
        }
    }
}

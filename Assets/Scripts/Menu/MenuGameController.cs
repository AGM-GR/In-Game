using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameController : MonoBehaviour {

    public void ExitGame() {
        Application.Quit();
    }

    public void StartGame() {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene() {
        Debug.Log("Cargando escena principal");
        AsyncOperation asyncload = SceneManager.LoadSceneAsync("ScapeRoom");

        while (!asyncload.isDone) {
            Debug.Log("Cargando al: " + asyncload.progress);
            yield return null;
        }
    }
}

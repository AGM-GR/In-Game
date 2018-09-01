using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameController : MonoBehaviour {

    public InitialLightOn sceneLight;

    private CustomFadeManager gameFadePlane;

    private void Start() {
        gameFadePlane = CustomFadeManager.Instance;
        StartCoroutine(StartMenuGame());
    }

    private IEnumerator StartMenuGame() {
        //Empieza con la pantalla en negro aclarandose
        gameFadePlane.ChangeFadeColor(Color.black);
        gameFadePlane.DoFade(0f, 3.4f, null, null);
        yield return null;
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void StartGame() {
        if (sceneLight)
            sceneLight.LightOff();
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene() {

        yield return new WaitForSeconds(2f);

        AsyncOperation asyncload = SceneManager.LoadSceneAsync("ScapeRoom");

        yield return new WaitWhile(() => !asyncload.isDone);

    }
}

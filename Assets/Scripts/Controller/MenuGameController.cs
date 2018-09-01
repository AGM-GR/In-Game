using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuGameController : MonoBehaviour {

    public InitialLightOn sceneLight;

    private CustomFadeManager gameFadePlane;
    private bool loading = false;

    private void Start() {
        gameFadePlane = CustomFadeManager.Instance;
        loading = false;
        StartCoroutine(StartMenuGame());
    }

    private IEnumerator StartMenuGame() {
        //Empieza con la pantalla en negro aclarandose
        gameFadePlane.ChangeFadeColor(Color.black);
        gameFadePlane.DoFade(0f, 3.4f, null, null);
        yield return null;
    }

    public void ExitGame() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void StartGame() {
        if (sceneLight)
            sceneLight.LightOff();
        if (!loading) {
            loading = true;
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadAsyncScene() {
        
        yield return new WaitForSeconds(2f);

        AsyncOperation asyncload = SceneManager.LoadSceneAsync("ScapeRoom");

        yield return new WaitWhile(() => !asyncload.isDone);

    }
}

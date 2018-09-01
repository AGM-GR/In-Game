using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class GameController : MonoBehaviour {

    public Vector3 initialPosition;
    public List<GameObject> holoToolkit = new List<GameObject>();

    private CustomFadeManager gameFadePlane;
    private Action gameFinishedAction;
    private ContentController contentController;

    private void Awake() {
        gameFinishedAction += LoadMenu;
        contentController = GetComponent<ContentController>();
        if (GameObject.FindGameObjectsWithTag("HoloToolkitCamera").Length == 0) {
            foreach (GameObject ht in holoToolkit)
                ht.SetActive(true);
        }
        else {
            foreach (GameObject htCamera in GameObject.FindGameObjectsWithTag("HoloToolkitCamera"))
                if (htCamera.activeInHierarchy)
                    htCamera.transform.position = initialPosition;
        }
    }

    private void Start() {
        gameFadePlane = CustomFadeManager.Instance;
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame() {
        //Empieza con la pantalla en negro aclarandose
        gameFadePlane.ChangeFadeColor(Color.black);
        gameFadePlane.DoFade(0f, 6f, null, null);
        yield return null;
        contentController.DeactivateContent();
    }

    public void FinishGame() {
        //Termina la partida con un fade en negro y el menu
        gameFadePlane.ChangeFadeColor(Color.white);
        gameFadePlane.DoFade(2.6f, 4f, gameFinishedAction, null);
    }

    private void LoadMenu() {

        //Destruye los objetos del HoloToolkit
        Destroy(InputManager.Instance.gameObject);
        Destroy(MixedRealityTeleport.Instance.gameObject);

        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadMenuScene() {
        yield return new WaitForSeconds(2f);
        //Vuelve al menu
        AsyncOperation asyncload = SceneManager.LoadSceneAsync("Menu");
        yield return new WaitWhile(() => !asyncload.isDone);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableKey : BaseInteractable {

    [Header("Key Interact Methods")]
    public UnityEvent keyMethods;

    [Header("Sound")]
    public AudioClip keyPressed;

    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    protected override void AttachToInteractor(BaseInteractor interactor) {
        audioSource.PlayOneShot(keyPressed);
        keyMethods.Invoke();
        interactor.FinishInteraction();
    }
}

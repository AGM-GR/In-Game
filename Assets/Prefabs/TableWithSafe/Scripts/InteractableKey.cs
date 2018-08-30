using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableKey : BaseInteractable {

    [Header("Key Interact Methods")]
    public UnityEvent keyMethods;

    protected override void AttachToInteractor(BaseInteractor interactor) {
        keyMethods.Invoke();
        interactor.FinishInteraction();
    }
}

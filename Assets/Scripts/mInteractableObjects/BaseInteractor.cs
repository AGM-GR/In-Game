using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;


#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

public abstract class BaseInteractor : MonoBehaviour{
	
	public event Action<BaseInteractor> OnInteractStateChange;
	public event Action<BaseInteractor> OnContactStateChange;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
    public InteractionSourceHandedness Handedness { get { return handedness; } set { handedness = value; } }
#endif

	public List<BaseInteractable> InteractedObjects { get { return new List<BaseInteractable>(interactedObjects); } }


    public GrabStateEnum InteractState {
        get {
			if (interactedObjects.Count > 1) {
                return GrabStateEnum.Multi;
            }

			return interactedObjects.Count > 0 ? GrabStateEnum.Single : GrabStateEnum.Inactive;
        }
    }

    public GrabStateEnum ContactState {
        get {
            if (contactObjects.Count > 1)
                return GrabStateEnum.Multi;
            else if (contactObjects.Count > 0)
                return GrabStateEnum.Single;
            else
                return GrabStateEnum.Inactive;
        }
    }

    public Transform InteractHandle {
        get {
            return interactAttachSpot != null ? interactAttachSpot : transform;
        }
    }

    [SerializeField]
    protected Transform interactAttachSpot;

	protected HashSet<BaseInteractable> interactedObjects = new HashSet<BaseInteractable>();
	protected List<BaseInteractable> contactObjects = new List<BaseInteractable>();

    private GrabStateEnum prevInteractState = GrabStateEnum.Inactive;
    private GrabStateEnum prevContactState = GrabStateEnum.Inactive;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
    [SerializeField]
    protected InteractionSourceHandedness handedness;
#endif

	public bool IsInteracting(BaseInteractable interactable) {
		return interactedObjects.Contains(interactable);
    }

	public virtual bool CanTransferOwnershipTo(BaseInteractable ownerInteractor, BaseInteractor otherInteractor) {
		Debug.Log("Transferring ownership of " + ownerInteractor.name + " to interactor " + otherInteractor.name);
		interactedObjects.Remove(ownerInteractor);
        return true;
    }
		
    protected virtual void InteractStart() {
        // Clean out the list of available objects list
        for (int i = contactObjects.Count - 1; i >= 0; i--) {
			if ((contactObjects[i] == null || !contactObjects[i].isActiveAndEnabled) && !interactedObjects.Contains(contactObjects[i])) {
                contactObjects.RemoveAt(i);
            }
        }

        // If there are any left after pruning
        if (contactObjects.Count > 0) {
            // Sort by distance and try to interact the closest
            SortAvailable();
			BaseInteractable closestAvailable = contactObjects[0];
            if (closestAvailable.TryInteractWith(this)) {
				interactedObjects.Add(contactObjects[0]);
            }
        }
    }
		
    protected virtual void InteractEnd() {
		interactedObjects.Clear();
    }

    protected virtual void OnEnable(){
    }

    protected virtual void OnDisable() {
		interactedObjects.Clear();
    }
		
	protected void AddContact(BaseInteractable availableObject) {
        if (!contactObjects.Contains(availableObject)) {
            contactObjects.Add(availableObject);
            availableObject.AddContact(this);
        }
    }

	public void RemoveContact(BaseInteractable availableObject) {
        contactObjects.Remove(availableObject);
        availableObject.RemoveContact(this);
    }

	public void RemoveContactList(BaseInteractable availableObject) {
		contactObjects.Remove(availableObject);
	}

	protected void ClearContact() {
		foreach (BaseInteractable bi in contactObjects)
			bi.RemoveContact(this);
		contactObjects.Clear ();
	}

    protected virtual void SortAvailable() {
		contactObjects.Sort(delegate (BaseInteractable b1, BaseInteractable b2) {
            return Vector3.Distance(b1.InteractPoint, InteractHandle.position).CompareTo(Vector3.Distance(b1.InteractPoint, InteractHandle.position));
        });
    }

    void Update() {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == gameObject) {
            if (Input.GetKeyDown(KeyCode.G)) {
                if (InteractState == GrabStateEnum.Inactive) {
                    Debug.Log("Interact start");
                    InteractStart();
                }
                else {
                    Debug.Log("Interact end");
                    InteractEnd();
                }
            }
        }
#endif

        if (prevInteractState != InteractState && OnInteractStateChange != null) {
            Debug.Log("Calling on grab change in interactor");
            OnInteractStateChange(this);
        }

        if (prevContactState != ContactState && OnContactStateChange != null) {
            Debug.Log("Calling on contact change in interactor");
            OnContactStateChange(this);
        }

        prevInteractState = InteractState;
        prevContactState = ContactState;
    }

	//Función propia, finaliza el agarre.
	public void FinishInteraction () {
		InteractEnd ();
	}

	public void FinishContact (BaseInteractable baseInteractable) {
		RemoveContact(baseInteractable);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;


public abstract class BaseInteractable : MonoBehaviour {
	public event Action<BaseInteractable> OnInteractStateChange;
	public event Action<BaseInteractable> OnContactStateChange;
	public event Action<BaseInteractable> OnInteracted;
	public event Action<BaseInteractable> OnInteractedBlocked;
	public event Action<BaseInteractable> OnReleased;


    public BaseInteractor InteractorPrimary {
        get {
			return activeInteractors.Count > 0 ? activeInteractors[activeInteractors.Count - 1] : null;
        }
    }

	public BaseInteractor InteractorContactPrimary {
		get { 
			return availableInteractors.Count > 0 ? availableInteractors [0] : null;
		}
	}

	public BaseInteractor[] ActiveInteractors {
        get {
			return activeInteractors.ToArray();
        }
    }

    public Vector3 InteractPoint {
        get {
            return interactSpot != null ? interactSpot.position : transform.position;
        }
    }

    public GrabStateEnum InteractState {
        get {
			if (activeInteractors.Count > 1) {
                return GrabStateEnum.Multi;
            }

			return activeInteractors.Count > 0 ? GrabStateEnum.Single : GrabStateEnum.Inactive;
        }
    }

    public GrabStateEnum ContactState {
        get {
            if (availableInteractors.Count > 1)
                return GrabStateEnum.Multi;
            else if (availableInteractors.Count > 0)
                return GrabStateEnum.Single;
            else
                return GrabStateEnum.Inactive;
        }
    }

	protected List<BaseInteractor> availableInteractors = new List<BaseInteractor>();

	protected List<BaseInteractor> activeInteractors = new List<BaseInteractor>();

    //left protected unless we have the occasion to use them publicly, then switch to public access
    [SerializeField]
    protected Transform interactSpot;

    [SerializeField]
    protected GrabStyleEnum interactStyle = GrabStyleEnum.Exclusive;

    private GrabStateEnum prevInteractState = GrabStateEnum.Inactive;
    private GrabStateEnum prevContactState = GrabStateEnum.Inactive;
    private Vector3 velocity;
    private Vector3 averageVelocity;
    private Vector3 previousVel;

	private bool contactEnabled = true;
	private bool interactionEnabled = true;

	public void InteractionEnabled (bool enabled) {
		interactionEnabled = enabled;
	}

	public virtual bool TryInteractWith(BaseInteractor interactor) {
		if (interactionEnabled) {
			// TODO error checking, multi-interact checking
			if (InteractState != GrabStateEnum.Inactive) {
				switch (interactStyle) {
				case GrabStyleEnum.Exclusive:
                    // Try to transfer ownership of interacted object
					BaseInteractor primary = InteractorPrimary;
					if (InteractorPrimary.CanTransferOwnershipTo (this, interactor)) {
						// Remove from interactable list and detach
						activeInteractors.Remove (primary);
						DetachFromInteractor (primary);
					} else {
						// If we can't, it's a no-go
						return false;
					}
					break;
				case GrabStyleEnum.Multi:
					break;
				default:
					throw new ArgumentOutOfRangeException ();
				}
			}

			StartInteract (interactor);
			return true;
		}

		if (OnInteractedBlocked != null)
			OnInteractedBlocked (this);

		return false;
    }
		
    public void AddContact(BaseInteractor availableObject) {
        availableInteractors.Add(availableObject);
    }
		
    public void RemoveContact(BaseInteractor availableObject) {
        availableInteractors.Remove(availableObject);
    }

	public void ClearContact() {
		foreach (BaseInteractor bi in availableInteractors)
			bi.RemoveContactList (this);
		availableInteractors.Clear ();
	}

    // The next three functions provide basic behavior. Extend from this base script in order to provide more specific functionality.

    protected virtual void AttachToInteractor(BaseInteractor interactor) {
        // By default this does nothing
        // In most cases this will parent or create a joint
    }

    protected virtual void DetachFromInteractor(BaseInteractor interactor) {
        // By default this does nothing
        // In most cases this will un-parent or destroy a joint
    }

    protected virtual void StartInteract(BaseInteractor interactor) {
        Debug.Log("Start interact");
        if (InteractState == GrabStateEnum.Inactive) {
            Debug.Log("State is inactive");
            // If we're not already updating our interact state, start now
            activeInteractors.Add(interactor);
            StartCoroutine(StayInteract());
        }
        else {
            Debug.Log("State is not inactive");
            // Otherwise just push the interactor
            activeInteractors.Add(interactor);
        }

        // Attach ourselves to this interactor
        AttachToInteractor(interactor);
        if (OnInteracted != null) {
            OnInteracted(this);
        }
    }
		
    protected virtual IEnumerator StayInteract() {
        yield return null;

        // While interactors are interacting
        while (InteractState != GrabStateEnum.Inactive) {
            // Call on interact stay in case this interactable wants to update itself
            OnInteractStay();
            for (int i = activeInteractors.Count - 1; i >= 0; i--) {
                if (activeInteractors[i] == null || !activeInteractors[i].IsInteracting(this)) {
                    Debug.Log("no longer being interacted by active interactor");
                    if (activeInteractors[i] != null) {
                        DetachFromInteractor(activeInteractors[i]);
                    }

                    activeInteractors.RemoveAt(i);
                }
            }
            yield return null;
        }

        EndInteract();
    }

    protected virtual void EndInteract() {
        if (OnReleased != null) {
            OnReleased(this);
        }
    }
		
    protected virtual void OnInteractStay() {
    }

    protected virtual void Start() {
    }

    protected virtual void Update() {
        if (prevInteractState != InteractState && OnInteractStateChange != null) {
            Debug.Log("Calling on interact change in interactable");
            OnInteractStateChange(this);
        }

        if (prevContactState != ContactState && OnContactStateChange != null) {
            Debug.Log("Calling on contact change in interactable");
            OnContactStateChange(this);
        }

        prevInteractState = InteractState;
        prevContactState = ContactState;
    }

	protected virtual void OnDisable() {
		if (InteractorPrimary)
			InteractorPrimary.FinishInteraction ();
		ClearContact ();
		activeInteractors.Clear ();

		if (OnInteractStateChange != null)
			OnInteractStateChange (this);
		if (OnContactStateChange != null)
			OnContactStateChange (this);
		if (OnReleased != null)
			OnReleased (this);
	}
}
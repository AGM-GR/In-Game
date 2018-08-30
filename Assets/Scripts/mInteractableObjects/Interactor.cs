using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;


#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

public class Interactor : BaseInteractor {
    [SerializeField]
    private LayerMask interactableLayers = ~0;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
    [SerializeField]
    private InteractionSourcePressType pressType = InteractionSourcePressType.None;
#endif

    protected override void OnEnable() {
        base.OnEnable();
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        InteractionManager.InteractionSourcePressed += InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionSourceReleased;
#endif
    }

    protected override void OnDisable() {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        InteractionManager.InteractionSourcePressed -= InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionSourceReleased;
#endif

		//Clear interaction at disable
		InteractEnd ();
		ClearContact();

        base.OnDisable();
    }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
    private void InteractionSourcePressed(InteractionSourcePressedEventArgs obj) {
        if (obj.pressType == pressType && obj.state.source.handedness == handedness) {
            InteractStart();
        }
    }

    private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj) {
        if (obj.pressType == pressType && obj.state.source.handedness == handedness) {
			TrySetThrowableObject(InteractedObjects.Count > 0 ? InteractedObjects[0] : null, obj.state.sourcePose);
            InteractEnd();
        }
    }
#endif


    protected virtual void OnTriggerEnter(Collider other) {
        if (!gameObject.activeInHierarchy || !this.enabled) {
            return;
        }

        Debug.Log("Entered trigger with " + other.name);
        if (((1 << other.gameObject.layer) & interactableLayers.value) == 0) {
            return;
        }

		BaseInteractable bi = other.GetComponent<BaseInteractable>();
        if (bi == null && other.attachedRigidbody != null) {
			bi = other.attachedRigidbody.GetComponent<BaseInteractable>();
        }

        if (bi == null) {
            return;
        }

		if (!bi.enabled)
		{
			return;
		}

        Debug.Log("Adding contact");

        AddContact(bi);
    }

    protected virtual void OnTriggerExit(Collider other) {
        if (!gameObject.activeInHierarchy || !this.enabled) {
            return;
        }

        Debug.Log("Exited trigger with " + other.name);
        if (((1 << other.gameObject.layer) & interactableLayers.value) == 0) {
            return;
        }

		BaseInteractable bi = other.GetComponent<BaseInteractable>();
        if (bi == null && other.attachedRigidbody != null) {
			bi = other.attachedRigidbody.GetComponent<BaseInteractable>();
        }

        if (bi == null) {
            return;
        }

		if (!bi.enabled)
		{
			return;
		}

        Debug.Log("Removing contact");

        RemoveContact(bi);
    }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
	public bool TrySetThrowableObject(BaseInteractable interactable, InteractionSourcePose poseInfo) {
		if (interactable == null) {
            return false;
        }

		if (!interactable.GetComponent<BaseThrowable>()) {
            return false;
        }

		if (!interactable.GetComponent<Rigidbody>()) {
            return false;
        }

		Rigidbody rb = interactable.GetComponent<Rigidbody>();
        Debug.Log("name of our rb.center of mass ========= " + rb.name);
		ControllerReleaseData controlReleaseData = interactable.GetComponent<Rigidbody>().GetThrowReleasedVelocityAndAngularVelocity(rb.centerOfMass, poseInfo);

		interactable.GetComponent<BaseThrowable>().LatestControllerThrowVelocity = controlReleaseData.Velocity;
		interactable.GetComponent<BaseThrowable>().LatestControllerThrowAngularVelocity = controlReleaseData.AngleVelocity;
        return true;
    }
#endif
}
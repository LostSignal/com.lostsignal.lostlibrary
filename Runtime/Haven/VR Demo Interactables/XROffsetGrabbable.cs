#pragma warning disable

#pragma warning disable

#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

using Test;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Small modification of the classic XRGrabInteractable that will keep the position and rotation offset between the
/// grabbed object and the controller instead of snapping the object to the controller. Better for UX and the illusion
/// of holding the thing (see Tomato Presence : https://owlchemylabs.com/tomatopresence/)
/// </summary>
public class XROffsetGrabbable : BetterXRGrabInteractable
{
#pragma warning disable 0649
    [SerializeField] private bool isOffsetGrabbable = true;
#pragma warning restore 0649

    private float originalTightenPosition;

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        int interactorLayerMask = 1 << interactor.gameObject.layer;
        return base.IsSelectableBy(interactor) && (interactionLayerMask.value & interactorLayerMask) != 0;
    }

    protected override void Awake()
    {
        base.Awake();
        this.originalTightenPosition = this.tightenPosition;
    }

    protected override void OnSelectEntering(XRBaseInteractor interactor)
    {
        if (this.isOffsetGrabbable)
        {
            if (this.attachTransform == null)
            {
                this.attachTransform = new GameObject("Attach Transform").transform;
                this.attachTransform.SetParent(this.transform);
                this.attachTransform.localPosition = Vector3.zero;
                this.attachTransform.localRotation = Quaternion.identity;
                this.attachTransform.localScale = Vector3.one;
            }

            this.attachTransform.position = interactor.attachTransform.position;
        }

        base.OnSelectEntering(interactor);
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if (this.isOffsetGrabbable)
        {
            bool isDirect = interactor is XRDirectInteractor;
            this.tightenPosition = isDirect ? 1 : this.originalTightenPosition;
        }

        base.OnSelectEntered(interactor);
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        base.OnSelectExited(interactor);
    }
}

#endif

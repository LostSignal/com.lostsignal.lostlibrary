#if UNITY && USING_UNITY_XR_INTERACTION_TOOLKIT

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Subclass of the classic Socket Interactor from the Interaction toolkit that will only accept object with the right.
/// </summary>
public class XRExclusiveSocketInteractor : XRSocketInteractor
{
    #pragma warning disable 0649
    [SerializeField]
    [FormerlySerializedAs("AcceptedType")]
    private string acceptedType;
    #pragma warning restore 0649

    public string AcceptedType => this.acceptedType;

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        SocketTarget socketTarget = interactable.GetComponent<SocketTarget>();

        if (socketTarget == null)
        {
            return false;
        }

        return base.CanSelect(interactable) && (socketTarget.SocketType == this.acceptedType);
    }

    public override bool CanHover(XRBaseInteractable interactable)
    {
        return this.CanSelect(interactable);
    }
}

#endif

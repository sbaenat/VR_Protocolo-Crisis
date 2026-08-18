using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System;

[Serializable]
public class HapticSettings
{
    public bool active;
    [Range(0f, 1f)]
    public float intensity;
    public float duration;
}

public class HepticFeedback : MonoBehaviour, IPointerEnterHandler
{
    public HapticSettings OnHoverEnter;

    private XRUIInputModule InputModule => EventSystem.current.currentInputModule as XRUIInputModule;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnHoverEnter != null && OnHoverEnter.active)
        {
            TriggerHaptic(eventData, OnHoverEnter);
        }
    }

    private void TriggerHaptic(PointerEventData eventData, HapticSettings settings)
    {
        if (InputModule == null) return;

        // Obtenemos el interactor que disparó el evento de UI
        var interactor = InputModule.GetInteractor(eventData.pointerId);

        // Solución directa: Intentamos tratarlo como un Ray Interactor
        if (interactor is XRRayInteractor ray)
        {
            ray.SendHapticImpulse(settings.intensity, settings.duration);
        }
    }
}

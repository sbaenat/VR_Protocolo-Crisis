using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;

    private readonly List<InputDevice> devices = new();

    private int redBars = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        RefreshDevices();
        StartCoroutine(AnxietyPulse());
    }

    public void RefreshDevices()
    {
        devices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller,
            devices
        );
    }

    public void RegisterRedBar()
    {
        redBars++;
    }

    public void UnregisterRedBar()
    {
        redBars = Mathf.Max(0, redBars - 1);
    }

    public int GetRedBars()
    {
        return redBars;
    }

    private void Vibrate(float amplitude, float duration)
    {
        foreach (InputDevice device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    device.SendHapticImpulse(0, amplitude, duration);
                }
            }
        }
    }

    private IEnumerator AnxietyPulse()
    {
        while (true)
        {
            if (redBars > 0)
            {
                // Intensidad según cantidad de barras en rojo
                float intensity = Mathf.Lerp(0.2f, 1f, redBars / 8f);

                // Frecuencia según cantidad de barras en rojo
                float interval = Mathf.Lerp(8f, 1f, redBars / 8f);

                Vibrate(intensity, 0.15f);

                yield return new WaitForSeconds(interval);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}

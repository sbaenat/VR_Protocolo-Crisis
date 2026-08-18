using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NeedBar : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;
    public Image fillImage;
    public Image iconImage;

    [Header("Need Settings")]
    [Range(0, 100)]
    public float currentValue = 100f;

    public float decreaseRate = 10f;

    [Header("Failure System")]
    public GameObject repairButton;

    private bool isFailing = false;
    public bool IsFailing => isFailing;

    private bool isInWarningZone = false;
    private bool inCrisis = false;

    private bool countedBelow50 = false;

    private Coroutine pulseRoutine;
    private Coroutine crisisRoutine;

    private float pulseTimer;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = currentValue;

        if (repairButton != null)
            repairButton.SetActive(false);

        UpdateUI();
    }

    private void Update()
    {
        if (!isFailing)
            return;

        if (!countedBelow50 && currentValue < 50f)
        {
            countedBelow50 = true;
            EndGameManager.Instance?.RegisterLowThreshold();
        }

        if (!inCrisis)
        {
            float multiplier = GetSpeedMultiplier();

            currentValue -= decreaseRate * multiplier * Time.deltaTime;
            currentValue = Mathf.Clamp(currentValue, 0f, 100f);

            if (currentValue <= 0f)
            {
                EnterCrisis();
            }
        }

        UpdateUI();
        CheckWarningZone();
    }

    private float GetSpeedMultiplier()
    {
        if (GameManager.Instance == null)
            return 1f;

        float timeLeft = GameManager.Instance.currentTime;

        if (timeLeft > 30f)
        {
            float t = Mathf.InverseLerp(120f, 30f, timeLeft);
            return Mathf.Lerp(1f, 5f, t);
        }

        return 5f;
    }

    private void UpdateUI()
    {
        if (!inCrisis)
        {
            slider.value = currentValue;

            fillImage.color = Color.Lerp(
                Color.red,
                Color.green,
                currentValue / 100f
            );
        }
    }

    private void CheckWarningZone()
    {
        if (inCrisis)
            return;

        if (currentValue <= 75f && !isInWarningZone)
        {
            isInWarningZone = true;

            if (pulseRoutine == null)
                pulseRoutine = StartCoroutine(PulseIcon());
        }
        else if (currentValue > 75f && isInWarningZone)
        {
            isInWarningZone = false;

            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }

            Color c = iconImage.color;
            c.a = 1f;
            iconImage.color = c;
        }
    }

    private IEnumerator PulseIcon()
    {
        while (true)
        {
            float urgency = 1f - (currentValue / 75f);
            urgency = Mathf.Clamp01(urgency);

            float speed = Mathf.Lerp(0.75f, 2.35f, urgency);

            pulseTimer += Time.deltaTime * speed;

            float alpha = (Mathf.Sin(pulseTimer * Mathf.PI) + 1f) * 0.5f;

            Color c = iconImage.color;
            c.a = Mathf.Lerp(1f, 0.25f, alpha);

            iconImage.color = c;

            yield return null;
        }
    }

    private void EnterCrisis()
    {
        inCrisis = true;

        slider.value = 100f;

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        Color iconColor = iconImage.color;
        iconColor.a = 1f;
        iconImage.color = iconColor;

        if (crisisRoutine == null)
            crisisRoutine = StartCoroutine(CrisisFlash());
    }

    private IEnumerator CrisisFlash()
    {
        Color brightRed = new Color(1f, 0f, 0f, 1f);
        Color darkRed = new Color(0.45f, 0f, 0f, 1f);

        while (inCrisis)
        {
            float pulse = (Mathf.Sin(Time.time * 8f) + 1f) * 0.5f;

            fillImage.color = Color.Lerp(darkRed, brightRed, pulse);

            yield return null;
        }

        crisisRoutine = null;
    }

    public void StartFailure()
    {
        if (isFailing)
            return;

        isFailing = true;

        countedBelow50 = false;

        if (repairButton != null)
            repairButton.SetActive(true);
    }

    public void ResolveFailure()
    {
        isFailing = false;

        EndGameManager.Instance?.RegisterCompletedTask();

        FillToMax();

        if (repairButton != null)
            repairButton.SetActive(false);
    }

    public void FillToMax()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        if (crisisRoutine != null)
        {
            StopCoroutine(crisisRoutine);
            crisisRoutine = null;
        }

        inCrisis = false;
        isInWarningZone = false;

        currentValue = 100f;
        slider.value = 100f;

        fillImage.color = Color.green;

        Color c = iconImage.color;
        c.a = 1f;
        iconImage.color = c;

        pulseTimer = 0f;

        UpdateUI();
    }
}
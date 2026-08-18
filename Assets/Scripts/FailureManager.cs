using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FailureManager : MonoBehaviour
{
    public NeedBar[] needBars;

    [Header("Failure Timing")]
    public float minStart = 10f;
    public float maxStart = 30f;

    public float minEnd = 3f;
    public float maxEnd = 5f;

    [Header("Alarm System")]
    public bool alarmActive = false;
    public int alarmThreshold = 3;

    [Header("Alarm Audio")]
    public AudioSource alarmAudio;

    [Header("Alarm Visual")]
    public Image alarmImage;

    private Coroutine alarmFlashRoutine;
    private bool running = false;

    private void Start()
    {
        if (alarmImage != null)
        {
            alarmImage.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (!running)
            return;

        CheckAlarmState();

        if (alarmActive)
        {
            EndGameManager.Instance?.AddCrisisTime(Time.deltaTime);
        }
    }

    public void StartFailureLoop()
    {
        if (running) return;

        running = true;
        StartCoroutine(FailureLoop());
    }

    private IEnumerator FailureLoop()
    {
        GameManager gm = GameManager.Instance;

        while (gm != null && gm.GameStarted)
        {
            float timeLeft = gm.currentTime;
            float duration = gm.gameDuration;

            float t = 1f - (timeLeft / duration);

            float min = Mathf.Lerp(minStart, minEnd, t);
            float max = Mathf.Lerp(maxStart, maxEnd, t);

            float wait = Random.Range(min, max);

            yield return new WaitForSeconds(wait);

            if (gm.currentTime <= 0f)
                yield break;

            TriggerRandomFailure();
        }
    }

    private void TriggerRandomFailure()
    {
        List<NeedBar> available = new List<NeedBar>();

        foreach (var bar in needBars)
        {
            if (!bar.IsFailing)
                available.Add(bar);
        }

        if (available.Count == 0)
            return;

        NeedBar selected =
            available[Random.Range(0, available.Count)];

        selected.StartFailure();

        EndGameManager.Instance?.RegisterFailure();
    }

    private void CheckAlarmState()
    {
        int lowCount = 0;

        foreach (var bar in needBars)
        {
            if (bar.currentValue < 50f)
                lowCount++;
        }

        if (!alarmActive && lowCount >= alarmThreshold)
        {
            alarmActive = true;
            OnAlarmStart();
            return;
        }

        if (alarmActive && lowCount == 0)
        {
            alarmActive = false;
            OnAlarmEnd();
            return;
        }
    }

    private void OnAlarmStart()
    {
        EndGameManager.Instance?.RegisterCrisis();

        Debug.Log("ALARMA ACTIVADA");

        if (alarmAudio != null && !alarmAudio.isPlaying)
        {
            alarmAudio.loop = true;
            alarmAudio.Play();
        }

        if (alarmImage != null)
        {
            alarmImage.gameObject.SetActive(true);

            if (alarmFlashRoutine == null)
            {
                alarmFlashRoutine = StartCoroutine(FlashAlarmImage());
            }
        }
    }

    private void OnAlarmEnd()
    {
        Debug.Log("ALARMA DESACTIVADA");

        if (alarmAudio != null && alarmAudio.isPlaying)
        {
            alarmAudio.Stop();
        }

        if (alarmImage != null)
        {
            if (alarmFlashRoutine != null)
            {
                StopCoroutine(alarmFlashRoutine);
                alarmFlashRoutine = null;
            }

            Color c = alarmImage.color;
            c.a = 1f;
            alarmImage.color = c;

            alarmImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashAlarmImage()
    {
        while (true)
        {
            Color c = alarmImage.color;

            c.a = 1f;
            alarmImage.color = c;
            yield return new WaitForSeconds(0.4f);

            c.a = 0.2f;
            alarmImage.color = c;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
using UnityEngine;
using TMPro;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;

    [Header("Final UI")]
    public GameObject popupFinal;
    public GameObject mensajeFinal;
    public GameObject imagenCrisis;
    public GameObject sonidoCrisis;

    [Header("Resultado")]
    public TextMeshProUGUI scoreText;

    [Header("Need Bars")]
    public NeedBar[] needBars;

    private int totalFailures;
    private int completedTasks;
    private int lowThresholdFailures;
    private int crisisCount;
    private float totalCrisisTime;

    private bool gameEnded = false;

    private void Awake()
    {
        Instance = this;

        if (mensajeFinal != null)
            mensajeFinal.SetActive(false);
    }

    private void Update()
    {
        if (gameEnded)
            return;

        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.GameStarted)
            return;

        if (GameManager.Instance.currentTime > 0f)
            return;

        EndGame();
    }

    public void RegisterFailure()
    {
        totalFailures++;
    }

    public void RegisterCompletedTask()
    {
        completedTasks++;
    }

    public void RegisterLowThreshold()
    {
        lowThresholdFailures++;
    }

    public void RegisterCrisis()
    {
        crisisCount++;
    }

    public void AddCrisisTime(float time)
    {
        totalCrisisTime += time;
    }

    private void EndGame()
    {
        gameEnded = true;

        foreach (NeedBar bar in needBars)
        {
            if (bar.repairButton != null)
                bar.repairButton.SetActive(false);
        }

        float score = 100f;

        score += completedTasks * 0.4f;

        score -= lowThresholdFailures * 1f;
        score -= crisisCount * 3f;
        score -= totalCrisisTime * 0.4f;

        score = Mathf.Clamp(score, 0f, 100f);

        if (scoreText != null)
            scoreText.text = Mathf.RoundToInt(score) + "%";

        if (popupFinal != null)
            popupFinal.SetActive(true);

        if (mensajeFinal != null)
        {
            mensajeFinal.SetActive(true);
            imagenCrisis.SetActive(false);
            sonidoCrisis.SetActive(false);
        }

        Debug.Log(
            $"RESULTADO FINAL\n" +
            $"Score: {score:F0}%\n" +
            $"Fallas generadas: {totalFailures}\n" +
            $"Tareas completadas: {completedTasks}\n" +
            $"Tareas bajo 50%: {lowThresholdFailures}\n" +
            $"Crisis: {crisisCount}\n" +
            $"Tiempo en crisis: {totalCrisisTime:F1}s"
        );
    }
}

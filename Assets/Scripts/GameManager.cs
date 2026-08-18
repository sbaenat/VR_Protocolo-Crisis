using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GameStarted { get; private set; }

    [Header("Game Time")]
    public float gameDuration = 120f;
    public float currentTime;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    public void StartGame()
    {
        if (GameStarted) return;

        GameStarted = true;
        currentTime = gameDuration;
    }

    private void Update()
    {
        if (!GameStarted) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0f);
    }

    public float GetNormalizedTime()
    {
        return currentTime / gameDuration;
    }

    public void RestartGame()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null)
            yield break;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                0f,
                1f,
                timer / fadeDuration
            );

            SetFadeAlpha(alpha);

            yield return null;
        }

        SetFadeAlpha(1f);
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null)
            yield break;

        SetFadeAlpha(1f);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                timer / fadeDuration
            );

            SetFadeAlpha(alpha);

            yield return null;
        }

        SetFadeAlpha(0f);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}

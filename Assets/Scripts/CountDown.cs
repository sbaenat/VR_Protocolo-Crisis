using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Image timeImage;
    [SerializeField] TextMeshProUGUI timeText;

    GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    public void OnPressStart()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado");
            return;
        }

        //panel.SetActive(false);

        GameManager.Instance.StartGame();

        FindFirstObjectByType<FailureManager>()?.StartFailureLoop();
    }

    private void Update()
    {
        if (gm == null || !gm.GameStarted) return;

        timeImage.fillAmount = gm.GetNormalizedTime();
        timeText.text = Mathf.CeilToInt(gm.currentTime).ToString();
    }
}

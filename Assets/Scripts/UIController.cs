using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Slider timerSlider;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI resultScoreText;

    private readonly string scoreTextFomat = "Score : {0}";

    private void Start()
    {
        var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        gameController.onChangeScore.AddListener(OnChangeScore);
        gameController.onChangeTimer.AddListener(OnChangeSlider);
    }

    public void OnChangeScore(int score)
    {
        scoreText.text = score.ToString();
        resultScoreText.text = string.Format(scoreTextFomat, score.ToString());
    }

    public void OnChangeSlider(float value, float maxValue)
    {
        timerSlider.value = value / maxValue;
    }

}

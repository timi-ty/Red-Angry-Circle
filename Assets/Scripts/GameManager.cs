using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static string InvertControlsKey = "invertControlsToggle";
    private static float scale;
    public static float Scale 
    {
        set => scale = value;
        get
        {
            if(scale <= 0)
                Debug.LogAssertion("You must set positive, non-zero Scale property in an Awake method");
            return scale / 0.7320234f;
        }
    }

    EnemyCommander enemyCommander;
    ObjectiveManager objectiveManager;
    PlayerMovement thePlayer;
    Canvas UICanvas;
    private int score;
    TextMeshProUGUI scoreDisplay;
    GameObject bladeMeter;
    TextMeshProUGUI highScoreDisplay;
    Button retryButton;
    Button resetButton;
    Toggle invertControlsToggle;
    GameObject gameObjectivesDisplay;
    bool gameStarted;
    ParticleSystem backgroundPS;
    Animator scoreAnimator;
    Animator highScoreAnimator;


    void Start()
    {
        gameStarted = false;
        score = 0;

        objectiveManager = GetComponent<ObjectiveManager>();

        backgroundPS = FindObjectOfType<ParticleSystem>();

        enemyCommander = FindObjectOfType<EnemyCommander>();
        thePlayer = FindObjectOfType<PlayerMovement>();

        UICanvas = FindObjectOfType<Canvas>();

        retryButton = UICanvas.transform.GetChild(3).GetComponent<Button>();
        retryButton.gameObject.SetActive(false);

        scoreDisplay = UICanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        scoreDisplay.gameObject.SetActive(false);
        scoreDisplay.text = score.ToString();

        bladeMeter = UICanvas.transform.GetChild(9).gameObject;
        bladeMeter.SetActive(false);

        highScoreDisplay = UICanvas.transform.GetChild(8).GetComponent<TextMeshProUGUI>();
        highScoreDisplay.gameObject.SetActive(false);

        invertControlsToggle = UICanvas.transform.GetChild(4).GetComponent<Toggle>();

        gameObjectivesDisplay = UICanvas.transform.GetChild(5).gameObject;

        resetButton = UICanvas.transform.GetChild(7).GetComponent<Button>();

        scoreAnimator = scoreDisplay.GetComponent<Animator>();
        highScoreAnimator = highScoreDisplay.GetComponent<Animator>();

        //Resize particle emitter to match screen width
        var shape = backgroundPS.shape;
        shape.radius = Boundary.visibleWorldWidth/2;

        invertControlsToggle.isOn = PlayerPrefs.GetInt(InvertControlsKey) == 1;

        enemyCommander.enabled = false;
    }

    private void Update() {
        if(Input.touchCount > 0){
            if(Input.GetTouch(0).phase == TouchPhase.Began && !gameStarted){
                Play();
                enabled = false;
            }
        } 
    }

    void Play()
    {
        UICanvas.gameObject.SetActive(true);

        for(int i = 0; i < UICanvas.transform.childCount; i++){
            UICanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        scoreDisplay.gameObject.SetActive(true);

        bladeMeter.SetActive(true);

        enemyCommander.enabled = true;

        PlayerPrefs.SetInt(InvertControlsKey, invertControlsToggle.isOn ? 1 : 0);

        thePlayer.GetComponent<PlayerBehaviour>().SetBladeMeter(
            bladeMeter.GetComponent<Image>());

        thePlayer.InitializePlayer(!gameStarted);

        SoundFX.ReviveRevolveSound();

        gameStarted = true;
    }

    public void IncrementScore(int payOff)
    {
        if (thePlayer.isActiveAndEnabled)
        {
            payOff = Mathf.RoundToInt(payOff / 10.0f);
            payOff = Mathf.Clamp(payOff, 1, payOff);
            if (payOff >= 12)
            {
                scoreAnimator.SetTrigger("payOffTwo");
            }
            else
            {
                scoreAnimator.SetTrigger("payOffOne");
            }
            score += payOff;
            scoreDisplay.text = score.ToString();
        }
    }

    public void ResetScore()
    {
        score = 0;
        scoreDisplay.text = score.ToString();
    }


    void GameOver()
    {
        UICanvas.gameObject.SetActive(true);

        for(int i = 0; i < UICanvas.transform.childCount; i++){
            UICanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        scoreDisplay.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        invertControlsToggle.gameObject.SetActive(true);
        highScoreDisplay.gameObject.SetActive(true);
        // gameObjectivesDisplay.SetActive(true);
        
        // objectiveManager.ConcludeRound();

        int highScore = ProgressManager.runtimePlayerProgress.highScore;
        string scoreMessage = "High Score \n";
        if(score > highScore){
            ProgressManager.runtimePlayerProgress.highScore = score;
            highScore = score;
            scoreMessage = "New " + scoreMessage;
            highScoreAnimator.SetBool("NewHighScore", true);
        }
        highScoreDisplay.text = scoreMessage + highScore;

        enemyCommander.enabled = false;

        SoundFX.DampenRevolveSound();

        ProgressManager.SaveProgress();
    }

    public void Retry()
    {
        thePlayer.gameObject.SetActive(true);
        BroadcastMessage("Restart");
        ResetScore();

        Play();
    }

    void EnemyDied(string[] data)
    {
        int payOff = int.Parse(data[1]);
        IncrementScore(payOff);
    }
}

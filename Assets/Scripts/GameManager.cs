using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum GameState {
    HighScores = -2,
    Menu = -1,
    Ready = 0,
    Playing = 1,
    GameOver = 2
}

public class GameManager : MonoBehaviour {
    public int currentScore {
        get { return _currScore; }
        set {
            _currScore = value;
            UpdateScore();
        }
    }
    [Header("Current score")]
    public GameObject scoreDigit0;
    public GameObject scoreDigit1;
    public GameObject scoreDigit2;
    [Header("Final score")]
    public GameObject finalScoreDigit0;
    public GameObject finalScoreDigit1;
    public GameObject finalScoreDigit2;
    [Header("Best score")]
    public GameObject bestScoreDigit0;
    public GameObject bestScoreDigit1;
    public GameObject bestScoreDigit2;
    public Sprite[] scoreDigits;
    public GameObject newBestTextGO;
    public AudioClip newBestAC;
    public UnityEvent highScores;
    public UnityEvent menu;
    public UnityEvent ready;
    public UnityEvent playing;
    public UnityEvent gameOver;
    public GameState gameState {
        get { return _gameState; }
        set {
            _gameState = value;
            switch (_gameState) {
                case GameState.HighScores: highScores.Invoke(); break;
                case GameState.Menu: StartCoroutine(TransitionEvent(menu, .5f)); break;
                case GameState.Ready:
                    StartCoroutine(TransitionEvent(ready, .5f));
                    currentScore = 0;
                    break;
                case GameState.Playing: playing.Invoke(); break;
                case GameState.GameOver:
                    UpdateFinalScores();
                    PushNewScore();
                    gameOver.Invoke(); break;
            }
        }
    }

    int _currScore;
    List<int> _highScores;
    GameState _gameState;
    Image _imageDigit0;
    Image _imageDigit1;
    Image _imageDigit2;
    Image _imageFinalDigit0;
    Image _imageFinalDigit1;
    Image _imageFinalDigit2;
    Image _imageBestDigit0;
    Image _imageBestDigit1;
    Image _imageBestDigit2;

    void Awake() {
        _imageDigit0 = scoreDigit0.GetComponent<Image>();
        _imageDigit1 = scoreDigit1.GetComponent<Image>();
        _imageDigit2 = scoreDigit2.GetComponent<Image>();
        _imageFinalDigit0 = finalScoreDigit0.GetComponent<Image>();
        _imageFinalDigit1 = finalScoreDigit1.GetComponent<Image>();
        _imageFinalDigit2 = finalScoreDigit2.GetComponent<Image>();
        _imageBestDigit0 = bestScoreDigit0.GetComponent<Image>();
        _imageBestDigit1 = bestScoreDigit1.GetComponent<Image>();
        _imageBestDigit2 = bestScoreDigit2.GetComponent<Image>();
    }
    
    void Start() {
        _highScores = new List<int>();
        _highScores.Add(0);
        LoadHighScores();
        //UpdateFinalScores();
        PushNewScore();
        //GameObject.Find("CanvasGameOver").GetComponent<Animator>().Play("Screen_GameOver");
        GameObject.Find("CanvasGameOver").GetComponent<Animator>().Play("Empty");
        gameState = GameState.Menu;
        menu.Invoke();
    }

    void UpdateScore() {
        int units = currentScore % 10;
        int tens = currentScore / 10;
        int hundreds = currentScore / 100;

        _imageDigit0.sprite = scoreDigits[units];
        _imageDigit1.sprite = scoreDigits[tens];
        _imageDigit2.sprite = scoreDigits[hundreds];

        scoreDigit1.SetActive(currentScore >= 10);
        scoreDigit2.SetActive(currentScore >= 100);
    }

    void UpdateFinalScores() {
        int units = currentScore % 10;
        int tens = currentScore / 10;
        int hundreds = currentScore / 100;

        _imageFinalDigit0.sprite = scoreDigits[units];
        _imageFinalDigit1.sprite = scoreDigits[tens];
        _imageFinalDigit2.sprite = scoreDigits[hundreds];

        finalScoreDigit1.SetActive(currentScore >= 10);
        finalScoreDigit2.SetActive(currentScore >= 100);

        units = _highScores[0] % 10;
        tens = _highScores[0] / 10;
        hundreds = _highScores[0] / 100;

        _imageBestDigit0.sprite = scoreDigits[units];
        _imageBestDigit1.sprite = scoreDigits[tens];
        _imageBestDigit2.sprite = scoreDigits[hundreds];

        bestScoreDigit1.SetActive(_highScores[0] >= 10);
        bestScoreDigit2.SetActive(_highScores[0] >= 100);
    }

    void PushNewScore() {
        if (currentScore > _highScores[0]) {
            newBestTextGO.SetActive(true);
            GetComponent<AudioSource>().PlayDelayed(0.75f);
        }
        _highScores.Add(currentScore);
        _highScores.Sort();
        _highScores.Reverse();
        _highScores = _highScores.GetRange(0, Mathf.Min(_highScores.Count, 10));
        SaveHighScores();
    }

    void LoadHighScores() {
        if (File.Exists(Application.persistentDataPath + "/highscores.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/highscores.sav", FileMode.Open);
            HighScores scores = (HighScores) bf.Deserialize(file);
            file.Close();

            foreach (int score in scores.highScoresList) {
                _highScores.Add(score);
                Debug.Log($"{score}");
            }
            _highScores.Sort();
            _highScores.Reverse();
        }
    }

    void SaveHighScores() {
        HighScores scores = CreateHighScoresObject();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/highscores.sav");
        bf.Serialize(file, scores);
        file.Close();
    }

    HighScores CreateHighScoresObject() {
        HighScores scores = new HighScores();
        foreach (int score in _highScores) {
            scores.highScoresList.Add(score);
        }
        return scores;
    }

    IEnumerator TransitionEvent(UnityEvent nextEvent, float time) {
        yield return new WaitForSeconds(time);
        nextEvent.Invoke();
    }

    public void SetGameState(int newState) {
        gameState = (GameState) newState;
    }
}

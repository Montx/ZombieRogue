using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public float levelStartDelay = .1f;
    public float turnDelay = .1f;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;

    [HideInInspector] 
    public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        level++;
        InitGame();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame() {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day" + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    private void Update() {
        if (Input.GetKey("escape")) {
            Application.Quit();
        }

        if (playersTurn || enemiesMoving || doingSetup) {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    IEnumerator MoveEnemies() {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0) {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public void GameOver() {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    public void AddEnemyToList(Enemy script) {
        enemies.Add(script);
    }
}

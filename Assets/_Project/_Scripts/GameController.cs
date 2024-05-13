using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameControllerSO gameControllerSO;
    [SerializeField] private SoundManagerSO soundManagerSO;

    public static event Action<Shape> OnStoreShapeInGrid;
    public UnityEvent<string, int> submitScoreEvent;
    public static event Action<int> OnGetScore;

    [Header("Core")]
    [SerializeField] private Board gameBoard;
    [SerializeField] private Spawner spawner;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private SoundManager sManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Holder holder;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject blankPanel;

    [Header("Ghost Shape")]
    [SerializeField] private GhostShape ghostShape;

    [Header("Other")]
    [SerializeField] private TextMeshProUGUI enterNameScoreText;
    [SerializeField] private TextMeshProUGUI leaderboardScoreText;
    [SerializeField] private TextMeshProUGUI leaderboardNameText;
    [SerializeField] private TextMeshProUGUI inputNameText;

    private void OnEnable()
    {
        gameControllerSO.pausePanel = pausePanel;
        gameControllerSO.blankPanel = blankPanel;
        gameControllerSO.enterNamePanel = enterNamePanel;
        gameControllerSO.gameOverPanel = gameOverPanel;
        gameControllerSO.gameplayPanel = gameplayPanel;
        gameControllerSO.leaderboardPanel = leaderboardPanel;
        gameControllerSO.mainMenuPanel = mainMenuPanel;
        gameControllerSO.optionsPanel = optionsPanel;
    }

    private void Start()
    {
        gameControllerSO.Init();

        if (!gameBoard) gameBoard = FindFirstObjectByType<Board>();
        if (!gameBoard) Debug.LogWarning("WARNING! There is no game board defined!!!");

        if (!spawner) { spawner = FindFirstObjectByType<Spawner>(); }
        if (!spawner) { Debug.LogWarning("WARNING! There is no spawner defined!!!"); }
        else
        {
            spawner.transform.position = VectorF.Round(spawner.transform.position);
            if (!gameControllerSO.activeShape)
            {
                gameControllerSO.activeShape = spawner.SpawnShape();
            }
        }

        if (!scoreManager) scoreManager = FindFirstObjectByType<ScoreManager>();
        if (!scoreManager) Debug.LogWarning("WARNING! There is no score manager defined!!!");

        if (!holder) holder = FindFirstObjectByType<Holder>();

        if (!ghostShape) ghostShape = FindFirstObjectByType<GhostShape>();
        if (ghostShape) ghostShape.DrawShape(gameControllerSO.activeShape, gameBoard);

        CreateWaitForSeconds(gameControllerSO.dropTimer, gameControllerSO.dropTimerValue);
        CreateWaitForSeconds(gameControllerSO.moveTimer, gameControllerSO.moveTimerValue);
        CreateWaitForSeconds(gameControllerSO.ghostTimer, gameControllerSO.ghostTimerValue);
        CreateWaitForSeconds(gameControllerSO.removeRowTimer, gameControllerSO.removeRowTimerValue);

        StartCoroutine(MoveDown());

        if (inputManager)
        {
            inputManager.OnMove += MoveShapePressed;
            inputManager.OnRotatePressed += RotateShapePressed;
            inputManager.OnDropPressed += DropShapePressed;
            inputManager.OnPausePressed += PauseGamePressed;
            inputManager.OnHoldPressed += HoldPressed;
        }

        if (scoreManager) scoreManager.levelUp += DidLevelUp;
        if (ghostShape) StartCoroutine(UpdateGhost());
    }

    private void HoldPressed(object sender, EventArgs e)
    {
        if (!holder || gameControllerSO.isPaused || gameControllerSO.isGameOver) return;

        if (!holder.HeldShape())
        {
            holder.Catch(gameControllerSO.activeShape);
            gameControllerSO.activeShape = spawner.SpawnShape();
            sManager.PlaySfxClip(soundManagerSO.HoldSound);
        }
        else if (holder.canRelease)
        {
            Shape shape = gameControllerSO.activeShape;
            gameControllerSO.activeShape = holder.Release();
            gameControllerSO.activeShape.transform.position = spawner.transform.position;
            holder.Catch(shape);
            sManager.PlaySfxClip(soundManagerSO.HoldSound);
        }
        else
        {
            Debug.LogWarning("HOLDER WARNING! Wait for cooldoen!");
            sManager.PlaySfxClip(soundManagerSO.MoveErrorSound);
        }

        if (ghostShape) ghostShape.ResetGhost();
    }

    private IEnumerator UpdateGhost()
    {
        ghostShape.DrawShape(gameControllerSO.activeShape, gameBoard);
        yield return gameControllerSO.ghostTimer;
        StartCoroutine(UpdateGhost());
    }

    private void DidLevelUp(object sender, EventArgs e)
    {
        gameControllerSO.dropTimerSpeedMultiplier = Mathf.Clamp(gameControllerSO.dropTimerValue - (((float)scoreManager.GetLevel() - 1) * 0.05f), 0.05f, 1f);
        gameControllerSO.didLevelUp = true;
        CreateWaitForSeconds(gameControllerSO.dropTimer, gameControllerSO.dropTimerValue * gameControllerSO.dropTimerSpeedMultiplier);
    }

    private void PauseGamePressed(object sender, EventArgs e)
    {
        if (!gameControllerSO.isGameOver) TogglePause();
    }

    private void MoveShapePressed(object sender, EventArgs e)
    {
        if (!gameBoard || !spawner || !gameControllerSO.activeShape || gameControllerSO.isGameOver || !sManager || gameControllerSO.isPaused || !scoreManager) return;

        MoveShape();
    }

    void MoveShape(float volume = 1)
    {
        Vector3 lastpos = gameControllerSO.activeShape.transform.position;
        Vector3 movement = inputManager.GetMovement();

        gameControllerSO.activeShape.Move(movement);

        if (!gameBoard.IsValidPosition(gameControllerSO.activeShape))
        {
            gameControllerSO.activeShape.MoveToLastPosition(lastpos);
            sManager.PlaySfxClip(soundManagerSO.MoveErrorSound, volume);
        }
        else if (inputManager.GetMovement() != Vector3.zero && !inputManager.IsMoveHeld) sManager.PlaySfxClip(soundManagerSO.MoveSound);

        ghostShape.DrawShape(gameControllerSO.activeShape, gameBoard);

        StartCoroutine(MoveShapeHeld());
    }

    IEnumerator MoveShapeHeld()
    {
        yield return gameControllerSO.moveTimer;
        if (inputManager.IsMoveHeld)
        {
            MoveShape(0);
        }
    }

    private IEnumerator MoveDown()
    {
        if (gameControllerSO.activeShape && !gameControllerSO.isGameOver && sManager && !gameControllerSO.isPaused && scoreManager)
        {
            gameControllerSO.activeShape.Move(Vector3.down);

            if (!gameBoard.IsValidPosition(gameControllerSO.activeShape))
            {
                if (gameBoard.IsOverLimit(gameControllerSO.activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }
        }
        yield return gameControllerSO.dropTimer;

        StartCoroutine(MoveDown());
    }

    private void LandShape()
    {
        StartCoroutine(gameControllerSO.activeShape.SetTrailInActive());
        sManager.PlaySfxClip(soundManagerSO.LandSound);
        gameControllerSO.activeShape.Move(Vector3.up);
        OnStoreShapeInGrid?.Invoke(gameControllerSO.activeShape);
        gameControllerSO.activeShape.LandShapeFX();
        if (ghostShape) ghostShape.ResetGhost();

        if (holder)
        {
            holder.canRelease = true;
        }

        StartCoroutine(gameBoard.ClearAllRows(gameControllerSO.removeRowTimer));

        if (gameBoard.CompletedRows > 0)
        {
            scoreManager.ScoreLines(gameBoard.CompletedRows);

            if (gameControllerSO.didLevelUp) sManager.PlaySfxClip(soundManagerSO.LevelUpVocalSound);
            else
            {
                if (gameBoard.CompletedRows > 1)
                {
                    AudioClip randomVocal = sManager.GetRandomClip(soundManagerSO.VocalSounds);
                    sManager.PlaySfxClip(randomVocal);
                }
            }

            sManager.PlaySfxClip(soundManagerSO.ClearRowSound);

            gameControllerSO.didLevelUp = false;
        }

        if (spawner)
        {
            gameControllerSO.activeShape = spawner.SpawnShape();
        }
    }

    private void RotateShapePressed(object sender, EventArgs e)
    {
        float rotationDirection = gameControllerSO.rotateClockwise ? -90f : 90f;

        if (!gameControllerSO.activeShape || gameControllerSO.isGameOver || gameControllerSO.isPaused || !scoreManager) return;

        gameControllerSO.activeShape.FastRotate(rotationDirection);

        if (!gameBoard.IsValidPosition(gameControllerSO.activeShape))
        {
            gameControllerSO.activeShape.FastRotate(-rotationDirection);
            sManager.PlaySfxClip(soundManagerSO.MoveErrorSound);
        }
        else
        {
            ghostShape.DrawShape(gameControllerSO.activeShape, gameBoard);
            gameControllerSO.activeShape.FastRotate(-rotationDirection);
            gameControllerSO.activeShape.Rotate(rotationDirection);
            sManager.PlaySfxClip(soundManagerSO.RotateSound);
        }
    }

    private void DropShapePressed(object sender, EventArgs e)
    {
        if (!gameControllerSO.activeShape || gameControllerSO.isGameOver || gameControllerSO.isPaused || !scoreManager) return;

        gameControllerSO.activeShape.SetTrailActive();

        while (gameBoard.IsValidPosition(gameControllerSO.activeShape))
        {
            gameControllerSO.activeShape.Move(Vector3.down);
        }

        if (!gameBoard.IsValidPosition(gameControllerSO.activeShape))
        {
            if (gameBoard.IsOverLimit(gameControllerSO.activeShape))
            {
                GameOver();
            }
            else
            {
                LandShape();
            }
        }
        sManager.PlaySfxClip(soundManagerSO.DropSound);
    }

    private void CreateWaitForSeconds(WaitForSeconds timer, float time)
    {
        if (timer == gameControllerSO.dropTimer) gameControllerSO.dropTimer = new WaitForSeconds(time);
        if (timer == gameControllerSO.moveTimer) gameControllerSO.moveTimer = new WaitForSeconds(time);
        if (timer == gameControllerSO.removeRowTimer) gameControllerSO.removeRowTimer = new WaitForSeconds(time);
    }

    public void StartGame()
    {
        Restart();
    }

    public void Restart()
    {
        CreateWaitForSeconds(gameControllerSO.dropTimer, gameControllerSO.dropTimerValue);
        gameControllerSO.isPaused = true;
        gameControllerSO.isGameOver = true;
        scoreManager.ResetScore();
        gameControllerSO.score = scoreManager.GetScore();
        gameControllerSO.ResetAllPanels();
        if (gameControllerSO.gameplayPanel) gameControllerSO.gameplayPanel.SetActive(true);
        holder.ResetHolder();
        OnStoreShapeInGrid?.Invoke(gameControllerSO.activeShape);
        gameBoard.ResetBoard();
        spawner.ResetQueue();
        gameControllerSO.activeShape = spawner.SpawnShape();
        if (ghostShape) ghostShape.ResetGhost();
        gameControllerSO.isPaused = false;
        gameControllerSO.isGameOver = false;
        StartCoroutine(UpdateGhost());
    }

    public void ToggleRotation()
    {
        gameControllerSO.rotateClockwise = !gameControllerSO.rotateClockwise;
    }

    public void TogglePause()
    {
        if (gameControllerSO.isGameOver) return;

        gameControllerSO.isPaused = !gameControllerSO.isPaused;

        if (gameControllerSO.pausePanel)
        {
            gameControllerSO.pausePanel.SetActive(gameControllerSO.isPaused);

            if (sManager) sManager.AlterVolume(0.25f, gameControllerSO.isPaused);
        }
    }

    private void GameOver()
    {
        gameControllerSO.activeShape.Move(Vector3.up);
        gameControllerSO.isGameOver = true;
        gameControllerSO.isPaused = true;
        sManager.PlaySfxClip(soundManagerSO.GameOverSound);
        sManager.PlaySfxClip(soundManagerSO.GameOverVocalSound);
        Debug.LogWarning($"{gameControllerSO.activeShape.name} is over the limit!!!");
        gameControllerSO.ResetAllPanels();
        if (gameControllerSO.gameOverPanel) gameControllerSO.gameOverPanel.SetActive(true);
    }

    public void NextButtonPressed()
    {
        gameControllerSO.ResetAllPanels();
        gameControllerSO.enterNamePanel.SetActive(true);
        gameControllerSO.score = scoreManager.GetScore();
        enterNameScoreText.text = gameControllerSO.score.ToString();
        leaderboardScoreText.text = gameControllerSO.score.ToString();
    }

    public void ShowOptionsPanel()
    {
        gameControllerSO.ResetAllPanels();
        if (gameControllerSO.optionsPanel) gameControllerSO.optionsPanel.SetActive(true);
    }

    public void ShowMainMenuPanel()
    {
        scoreManager.ResetScore();
        gameControllerSO.score = scoreManager.GetScore();
        gameControllerSO.ResetAllPanels();
        if (gameControllerSO.blankPanel) gameControllerSO.blankPanel.SetActive(true);
        if (gameControllerSO.mainMenuPanel) gameControllerSO.mainMenuPanel.SetActive(true);
        gameControllerSO.isGameOver = true;
        gameControllerSO.isPaused = true;
    }

    public void SubmitScore()
    {
        gameControllerSO.ResetAllPanels();
        if (inputNameText.text != "") submitScoreEvent?.Invoke(inputNameText.text, gameControllerSO.score);
        OnGetScore?.Invoke(gameControllerSO.score);
        leaderboardNameText.text = inputNameText.text;
        if (gameControllerSO.leaderboardPanel) gameControllerSO.leaderboardPanel.SetActive(true);
    }

    public void QuitGame()
    {
        gameBoard.ResetBoard();
        Application.Quit();
    }
}

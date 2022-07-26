using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        loading,
        inGame,
        paused,
        gameOver
    }

    [Header("GameStates")]
    private GameState gameState;

    [Header("Statistics")]
    [SerializeField] private float health = 20f;
    private int _score;
    private int Score { get { return _score; } set { _score = Mathf.Clamp(value, 0, 9999); } }
    private bool isReceivingDamage = false;

    [Header("GameObjects")]
    private CinemachineVirtualCamera _cinemachine;
    private HealthBar healthbar;
    private TextMeshProUGUI coinCount;
    private PlayerController player;

    private void Awake()
    {
        _cinemachine = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        healthbar = GetComponentInChildren<HealthBar>();
        healthbar.SetMaxHealth(health);

        coinCount = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        healthbar.SetHealthBarValue(health);
        StartCoroutine(DamageAnimation());
        if (health <= 0)
        {
            gameState = GameState.gameOver;
            Debug.Log($"Game Over");
            RestartGame();
            //Destroy(gameObject);
        }

    }

    /// <summary>
    /// Enable and disable renderer to create damage animation
    /// </summary>
    private IEnumerator DamageAnimation()
    {
        float delay = 0.1f;
        var gamerenderer = player.GetComponent<Renderer>();
        gamerenderer.enabled = false;
        yield return new WaitForSeconds(delay);

        gamerenderer.enabled = true;
        yield return new WaitForSeconds(delay);

        gamerenderer.enabled = false;
        yield return new WaitForSeconds(delay);

        gamerenderer.enabled = true;
    }

    /// <summary>
    /// On restart button, load actual scene
    /// </summary>
    public void RestartGame()
    {
        gameState = GameState.inGame;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator ShakeCamera(float amp, float duration)
    {
        if(gameState == GameState.loading)
        {

        }
        CinemachineBasicMultiChannelPerlin shake = _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shake.m_AmplitudeGain = amp;

        yield return new WaitForSeconds(duration);

        shake.m_AmplitudeGain = 0;
    }

    public void UpdateCoinCount(int value)
    {
        Score += value;
        coinCount.text = $"{Score}";
    }

    /// <summary>
    /// Start new game
    /// </summary>
    public void StartGame()
    {
        UpdateCoinCount(0);
        coinCount.gameObject.SetActive(true);
        gameState = GameState.inGame;
    }

    private void Start()
    {
        StartGame();
    }
}

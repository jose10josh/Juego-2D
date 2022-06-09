using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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

    [Header("GameObjects")]
    private CinemachineVirtualCamera _cinemachine;

    private void Awake()
    {
        _cinemachine = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameState = GameState.gameOver;
            Debug.Log($"Game Over");
            RestartGame();
            //Destroy(gameObject);
        }

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
}

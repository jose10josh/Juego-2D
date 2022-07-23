using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    private enum ItemType
    {
        Potion,
        Money,
        PowerUp
    }

    [Header("Statistics")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private int value;

    [Header("GameObjects")]
    private GameManager gameManager;
    private PlayerController player;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(itemType == ItemType.Money)
            {
                gameManager.UpdateCoinCount(value);
                Destroy(gameObject);
            }
        }
    }
}

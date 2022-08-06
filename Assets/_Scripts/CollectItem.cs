using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    private enum ItemType
    {
        Health,
        Money,
        Damage,
        Speed
    }

    [Header("Statistics")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private float value;

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
            if (itemType == ItemType.Money)
            {
                gameManager.UpdateCoinCount(value);
            }
            else if (itemType == ItemType.Health)
            {
                gameManager.ReceiveDamage(-value);
            }
            else if (itemType == ItemType.Speed)
            {

            }
            else if (itemType == ItemType.Damage)
            {
                var swordDamage = collision.transform.Find("SwordParticle1").GetComponent<AttackController>();
                swordDamage.ChangeSwordDamage(value);
            }

            Destroy(gameObject);
        }
    }
}

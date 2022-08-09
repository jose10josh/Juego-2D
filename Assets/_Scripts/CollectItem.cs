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
    [SerializeField] private float attackDuration = 4f;

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
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Health)
            {
                gameManager.ReceiveDamage(-value);
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Speed)
            {

            }
            else if (itemType == ItemType.Damage)
            {
                
                var swordDamage = player.transform.Find("SwordParticle1").GetComponent<AttackController>();
                swordDamage.ChangeSwordDamage(value);
                player.ChangePlayerDamage(value);

                Invoke("ResetPlayerDamage", attackDuration);
                gameObject.GetComponent<Renderer>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }

        }
    }

    private void ResetPlayerDamage()
    {
        var swordDamage = player.transform.Find("SwordParticle1").GetComponent<AttackController>();
        swordDamage.ChangeSwordDamage(-value);
        player.ChangePlayerDamage(-value);
        Destroy(gameObject);
    }
}

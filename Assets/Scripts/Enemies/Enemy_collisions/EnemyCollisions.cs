using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyCollisions : MonoBehaviour
{
    private bool _isDead;
    [SerializeField] private GameObject scorePopupPrefab;

    private static List<EnemyCollisions> _allEnemies = new List<EnemyCollisions>();
    
    public static List<EnemyCollisions> AllEnemies => _allEnemies;

    private void OnEnable()
    {
        _allEnemies.Add(this);
    }

    private void OnDisable()
    {
        _allEnemies.Remove(this);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        if (other.CompareTag("PlayerBullet"))
        {
            HandleBulletHit();
        }
        else if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other.gameObject);
        }
    }


    private void HandleBulletHit()
    {
        EnemyStateController stateController = GetComponent<EnemyStateController>();
        EnemySlot slot = GetComponent<EnemySlot>();
            
        EnemyState stateAtMomentOfHit = stateController.CurrentState;
        if (stateAtMomentOfHit == EnemyState.Dead)
        {
            stateAtMomentOfHit = stateController.PreviousState;
        }
            
        bool wasDivingForScore = false;
        if (stateAtMomentOfHit == EnemyState.Diving)
        {
            if (slot != null && transform.parent != slot.FormationRoot)
            {
                wasDivingForScore = true;
            }
        }

        _isDead = true;
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health != null) health.TakeHit(999); 
        StartCoroutine(HandleEnemyDestruction(wasDivingForScore));
    }

    private void HandlePlayerCollision(GameObject player)
    {
        _isDead = true;
        player.GetComponent<PlayerHealth>()?.TakeHit();
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health != null) health.TakeHit(999);
        StartCoroutine(HandleEnemyDestruction(false));
    }

private IEnumerator HandleEnemyDestruction(bool isActuallyDiving)
{
    EnemyIdentity identity = GetComponent<EnemyIdentity>();
    if (HudManager.Instance != null)
    {
        HudManager.Instance.AddScoreFromEnemy(identity, isActuallyDiving);
    }
    if (isActuallyDiving && identity != null && identity.enemyData != null)
    {
        if (identity.enemyData.bonusScoreSprite != null)
        {
            SpawnScorePopup(identity.enemyData.bonusScoreSprite);
        }
    }
    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.enabled = false;

    EnemyDive diveScript = GetComponent<EnemyDive>();
    if (diveScript != null) diveScript.enabled = false;

    Animator animator = GetComponentInChildren<Animator>();
    if (animator != null)
    {
        animator.speed = 1f;
        animator.SetTrigger("Explode");
        animator.transform.localRotation = Quaternion.identity;
    }

    yield return new WaitForSeconds(0.5f);
    Destroy(gameObject);
    if (SessionManager.Instance != null)
    {
        SessionManager.Instance.CheckForVictory();
    }
}

private void SpawnScorePopup(Sprite bonusSprite)
{
    if (scorePopupPrefab != null)
    {
        GameObject popup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);
        FloatingScore fs = popup.GetComponent<FloatingScore>();
        
        if (fs != null)
        {
            EnemyIdentity identity = GetComponent<EnemyIdentity>();
            int scoreValue = 0;
            if (identity != null && identity.enemyData != null)
            {
                scoreValue = identity.enemyData.divingScoreValue;
            }
            fs.Setup(bonusSprite, scoreValue);
        }
    }
}


    public void ReturnWhenGameOver()
    {
        _isDead = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        EnemyDive dive = GetComponent<EnemyDive>();
        if (dive != null)
        {
            dive.enabled = false;
        }

        ReturnToGroup returnScript = GetComponent<ReturnToGroup>();
        if (returnScript != null)
        {
            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                returnScript.BeginReturn();
            }
        }
    }
}
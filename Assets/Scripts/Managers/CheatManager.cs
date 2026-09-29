using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CheatManager : MonoBehaviour
{
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        if (keyboard.digit1Key.wasPressedThisFrame) StopAllDives();
        
        if (keyboard.digit2Key.wasPressedThisFrame) KillFlagshipWithBonus();
        
        if (keyboard.hKey.wasPressedThisFrame && PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.TakeHit();
        }
        if (keyboard.rKey.wasPressedThisFrame && PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.ResetLives();
        }
        
        if (keyboard.lKey.wasPressedThisFrame && StarSpawner.Instance != null)
        {
            StarSpawner.Instance.SpawnStar();
        }
        if (keyboard.pKey.wasPressedThisFrame && SessionManager.Instance != null)
        {
            Debug.Log("[Cheat] Fast-forwarding to Game Mode...");
            SessionManager.Instance.StartGame();
        }
        
        if (keyboard.kKey.wasPressedThisFrame)
        {
            ExplodeAllEnemies();
        }
    }

    private void ExplodeAllEnemies()
    {
        List<EnemyCollisions> enemiesToExplode = new List<EnemyCollisions>(EnemyCollisions.AllEnemies);
        Debug.Log($"Cheats: Exploding {enemiesToExplode.Count} enemies");

        foreach (EnemyCollisions enemy in enemiesToExplode)
        {
            if (enemy != null)
            {
                enemy.StartCoroutine("HandleEnemyDestruction", false);
            }
        }
    }

    private void StopAllDives()
    {
        Debug.Log($"Cheats: Resetting {EnemyDive.AllDivers.Count} enemies to slots");
        foreach (EnemyDive diver in EnemyDive.AllDivers)
        {
            if (diver != null)
            {
                ReturnToGroup returnToGroup = diver.GetComponent<ReturnToGroup>();
                if (returnToGroup != null) 
                {
                    returnToGroup.BeginReturn();
                }
            }
        }
    }

    private void KillFlagshipWithBonus()
    {
        foreach (EnemyIdentity enemy in EnemyIdentity.AllEnemies)
        {
            if (enemy == null) continue;

            EnemyRoleTag roleTag = enemy.GetComponent<EnemyRoleTag>();
            if (roleTag != null && roleTag.role == EnemyRole.Flagship)
            {
                EnemyCollisions collisions = enemy.GetComponent<EnemyCollisions>();
                if (collisions != null)
                {
                    collisions.StartCoroutine("HandleEnemyDestruction", true);
                    break; 
                }
            }
        }
    }
}
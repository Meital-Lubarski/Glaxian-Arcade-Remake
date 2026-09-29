using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class AttackDirector : MonoBehaviour
{
    [SerializeField] private float firstAttackDelaySeconds = 2f;
    [SerializeField] private float timeBetweenAttacksSeconds = 1.5f;
    [SerializeField, Range(0f, 1f)] private float escortDiveChance = 0.11f;

    [SerializeField] private int minEnemiesToStartAttack = 1;
    [SerializeField] private int maxSoloSearchAttempts = 10;
    [SerializeField] private int maxEscortSearchAttempts = 20;


    private const int SIDE_RIGHT = 1;
    private const int SIDE_LEFT = -1;
    private const int SIDE_ANY = 0;
    private float _timer;

    private void Start()
    {
        _timer = firstAttackDelaySeconds;
    }

    private void Update()
    {
        if (!IsGamePlaying())
        {
            return;
        }

        List<EnemyDive> divers = EnemyDive.AllDivers;
        if (divers == null)
        {
            return;
        }

        divers.RemoveAll(d => d == null);

        if (divers.Count < minEnemiesToStartAttack)
        {
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer > 0f)
        {
            return;
        }

        bool tryEscortThisTick = Random.value < escortDiveChance;
        if (tryEscortThisTick && TryEscortDive(divers))
        {
            _timer = timeBetweenAttacksSeconds;
            return;
        }

        TrySoloDive(divers);
        _timer = timeBetweenAttacksSeconds;
    }

    private static bool IsGamePlaying()
    {
        return SessionManager.Instance != null &&
               SessionManager.Instance.GetCurrentState() == GameState.Playing;
    }

    private void TrySoloDive(List<EnemyDive> divers)
    {
        int maxAttempts = Mathf.Min(divers.Count, maxSoloSearchAttempts);

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            EnemyDive d = divers[Random.Range(0, divers.Count)];
            if (d == null)
            {
                continue;
            }

            EnemyRoleTag tag = d.GetComponent<EnemyRoleTag>();
            if (tag != null && tag.role == EnemyRole.Flagship)
            {
                continue;
            }

            if (d.TryStartDive())
            {
                return;
            }
        }
    }

    private bool TryEscortDive(List<EnemyDive> divers)
    {
        EnemyDive flagship = FindDiveCandidate(divers, EnemyRole.Flagship, null, null, SIDE_ANY);
        if (flagship == null)
        {
            return false;
        }

        int side = GetSideFromSlot(flagship);
        if (side == SIDE_ANY)
        {
            side = SIDE_RIGHT;
        }

        EnemyDive escortA = FindDiveCandidate(divers, EnemyRole.Red, flagship, null, side);
        if (escortA == null)
        {
            return false;
        }

        EnemyDive escortB = FindDiveCandidate(divers, EnemyRole.Red, flagship, escortA, side);
        if (escortB == null)
        {
            return false;
        }

        SplineContainer shared = flagship.PickDiveTemplate();
        if (shared == null)
        {
            return false;
        }

        if (!flagship.TryStartDiveOn(shared))
        {
            return false;
        }

        if (!escortA.TryStartDiveOn(shared))
        {
            return false;
        }

        if (!escortB.TryStartDiveOn(shared))
        {
            return false;
        }

        return true;
    }

    private static int GetSideFromSlot(EnemyDive d)
    {
        if (d == null)
        {
            return SIDE_ANY;
        }

        EnemySlot slot = d.GetComponent<EnemySlot>();
        if (slot == null)
        {
            return SIDE_ANY;
        }

        float x = slot.HomeLocalPosition.x;
        if (x > 0f)
        {
            return SIDE_RIGHT;
        }

        if (x < 0f)
        {
            return SIDE_LEFT;
        }

        return SIDE_ANY;
    }

    private EnemyDive FindDiveCandidate(List<EnemyDive> divers, EnemyRole role, EnemyDive excludeA, EnemyDive excludeB, int requiredSide)
    {
        int maxAttempts = Mathf.Min(divers.Count * 2, maxEscortSearchAttempts);

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            EnemyDive d = divers[Random.Range(0, divers.Count)];
            if (d == null || d == excludeA || d == excludeB)
            {
                continue;
            }

            EnemyRoleTag tag = d.GetComponent<EnemyRoleTag>();
            if (tag == null || tag.role != role)
            {
                continue;
            }

            EnemyStateController st = d.GetComponent<EnemyStateController>();
            if (st == null || st.CurrentState != EnemyState.InFormation)
            {
                continue;
            }

            if (requiredSide != SIDE_ANY)
            {
                int side = GetSideFromSlot(d);
                if (side != requiredSide)
                {
                    continue;
                }
            }

            return d;
        }
        return null;
    }
}

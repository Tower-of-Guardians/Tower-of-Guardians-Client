using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private PlayerUnit player;
    [SerializeField] private List<MonsterUnit> monsters = new();
    [SerializeField] private Button attackButton;
    [SerializeField] private bool playerAttackHitsAll;
    [SerializeField] private float monsterAttackDelay = 0.15f;

    private MonsterUnit selectedTarget;
    private bool isProcessingAttack;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate BattleManager detected - destroying the newer instance.");
            Destroy(this);
            return;
        }

        Instance = this;
        SubscribeToMonsters();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        UnsubscribeFromMonsters();
    }

    private void OnEnable()
    {
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(HandleAttackButton);
        }
    }

    private void OnDisable()
    {
        if (attackButton != null)
        {
            attackButton.onClick.RemoveListener(HandleAttackButton);
        }
    }

    public void RegisterMonster(MonsterUnit monster)
    {
        if (monster == null || monsters.Contains(monster))
        {
            return;
        }

        monsters.Add(monster);
        monster.Clicked += OnMonsterClicked;
    }

    public void UnregisterMonster(MonsterUnit monster)
    {
        if (monster == null)
        {
            return;
        }

        if (monsters.Remove(monster))
        {
            monster.Clicked -= OnMonsterClicked;
        }

        if (selectedTarget == monster)
        {
            monster.SetTargeted(false);
            selectedTarget = null;
        }
    }

    private void SubscribeToMonsters()
    {
        foreach (MonsterUnit monster in monsters)
        {
            if (monster != null)
            {
                monster.Clicked += OnMonsterClicked;
            }
        }
    }

    private void UnsubscribeFromMonsters()
    {
        foreach (MonsterUnit monster in monsters)
        {
            if (monster != null)
            {
                monster.Clicked -= OnMonsterClicked;
            }
        }
    }

    private void OnMonsterClicked(MonsterUnit monster)
    {
        if (monster == null || !monster.IsAlive)
        {
            return;
        }

        if (selectedTarget == monster)
        {
            monster.SetTargeted(false);
            selectedTarget = null;
            return;
        }

        if (selectedTarget != null)
        {
            selectedTarget.SetTargeted(false);
        }

        selectedTarget = monster;
        selectedTarget.SetTargeted(true);
    }

    private void HandleAttackButton()
    {
        if (isProcessingAttack || player == null)
        {
            return;
        }

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isProcessingAttack = true;

        List<MonsterUnit> aliveMonsters = monsters.Where(m => m != null && m.IsAlive).ToList();
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("No monsters available to attack.");
            isProcessingAttack = false;
            yield break;
        }

        List<IDamageable> playerTargets = new();
        if (playerAttackHitsAll)
        {
            playerTargets.AddRange(aliveMonsters);
        }
        else
        {
            MonsterUnit target = selectedTarget != null && selectedTarget.IsAlive
                ? selectedTarget
                : aliveMonsters[Random.Range(0, aliveMonsters.Count)];

            playerTargets.Add(target);
        }

        yield return StartCoroutine(player.PerformAttack(playerTargets, playerAttackHitsAll));

        foreach (MonsterUnit monster in monsters)
        {
            if (monster != null)
            {
                monster.SetTargeted(false);
            }
        }
        selectedTarget = null;

        aliveMonsters = monsters.Where(m => m != null && m.IsAlive).ToList();

        foreach (MonsterUnit monster in aliveMonsters)
        {
            if (monster == null || !monster.IsAlive)
            {
                continue;
            }

            monster.PerformAttack(player);

            if (monsterAttackDelay > 0f)
            {
                yield return new WaitForSeconds(monsterAttackDelay);
            }

            if (!player.IsAlive)
            {
                Debug.Log("Player defeated.");
                break;
            }
        }

        isProcessingAttack = false;
    }
}


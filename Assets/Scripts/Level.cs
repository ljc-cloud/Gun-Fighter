using System;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Instance;

    public string CurrentLevel;
    [SerializeField]
    private int enemyCount;
    [SerializeField]
    private int enemyLeft;

    public event Action OnTargetComplete;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;
        enemyCount = FindObjectsOfType<EnemyController>().Length;
        enemyLeft = enemyCount;
        foreach (var item in GameObject.FindGameObjectsWithTag("Enemy").Select(item => item.transform.GetComponent<Health>()))
        {
            item.OnEnemyDie += Level_OnEnemyDie;
        }
    }

    private void Level_OnEnemyDie()
    {
        enemyLeft--;
        if (enemyLeft == 0)
            OnTargetComplete.Invoke();
    }

}

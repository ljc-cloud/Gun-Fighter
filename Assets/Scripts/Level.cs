using System;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Instance;

    public GameObject HealthPickUpPre;

    public string CurrentLevel;
    public string NextLevel;
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

    private void Level_OnEnemyDie(Transform trans)
    {
        enemyLeft--;
        float random = UnityEngine.Random.Range(0f, 1f);
        Debug.Log($"rnadom => {random}");
        Vector3 pos = trans.position;
        pos.y += 0.5f;
        if (random > 0.5f)
            Instantiate(HealthPickUpPre, pos, Quaternion.identity);
        if (enemyLeft == 0)
            OnTargetComplete.Invoke();
    }

}

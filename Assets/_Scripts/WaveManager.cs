using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Cài đặt sinh quái")]
    public GameObject[] enemyPrefabs; // Chứa bản gốc con quái
    public Transform[] lanes;      // Chứa 3 đường đi
    public float spawnInterval = 2f; 

    void Start()
    {
        // Bắt đầu vòng lặp đẻ quái vô tận để test
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnInterval); // Đợi 1 khoảng thời gian
        }
    }

    void SpawnSingleEnemy()
    {

        // Chọn ngẫu nhiên 0, 1 hoặc 2
        int randomLaneIndex = Random.Range(0, lanes.Length);
        Transform selectedLane = lanes[randomLaneIndex];

        // Đẻ con quái ra bản đồ
        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemy]);

        // Truyền đường đi cho nó
        EnemyMovement moveScript = newEnemy.GetComponent<EnemyMovement>();
        if (moveScript != null)
        {
            moveScript.SetupLane(selectedLane);
        }
    }
}
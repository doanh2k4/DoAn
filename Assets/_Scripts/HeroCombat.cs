using UnityEngine;

public class HeroCombat : MonoBehaviour
{
    [Header("Chỉ số")]
    public float attackRange = 7f;
    public float attackRate = 1.5f;
    private float nextAttackTime = 0f;

    [Header("Vũ khí")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    private Animator anim;
    private Transform currentTarget;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        FindTarget();

        if (currentTarget != null && Time.time >= nextAttackTime)
        {
            Shoot();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= attackRange)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        currentTarget = nearestEnemy != null ? nearestEnemy.transform : null;
    }

    void Shoot()
    {
        // 1. Giật animation Bắn
        if (anim != null) anim.SetTrigger("Attack");

        // 2. Đẻ ra mũi tên
        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Projectile projScript = arrow.GetComponent<Projectile>();
            if (projScript != null) projScript.Seek(currentTarget);
        }
    }
}
using UnityEngine;

public class HeroCombat : MonoBehaviour
{
    [Header("Chỉ số")]
    public float attackRange = 7f;
    public float attackRate = 1.5f;
    private float nextAttackTime = 0f;

    [Header("Thuộc tính Nguyên Tố")]
    public ElementType element; // Kéo chọn hệ cho Tướng trên Inspector

    [Header("Vũ khí")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    private Animator anim;
    private Transform currentTarget;

    // SỬA Ở ĐÂY: Đổi thành public để Castle có thể đọc được hệ số sức mạnh
    public float damageMultiplier = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // ĐÃ XÓA ĐOẠN HỒI MÁU HỆ THỦY Ở ĐÂY!

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
        if (anim != null) anim.SetTrigger("Attack");

        // 📢 BỔ SUNG NGAY TẠI ĐÂY: Phát tiếng bắn cung vèo vèo
        if (AudioManager.Instance != null && AudioManager.Instance.bowShootSound != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bowShootSound);
        }

        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Projectile projScript = arrow.GetComponent<Projectile>();

            if (projScript != null)
            {
                projScript.SetupProjectile(currentTarget, element);
                projScript.damage *= damageMultiplier;
            }
        }
    }

    public void ApplyLevelStats(int level)
    {
        if (level == 2)
        {
            attackRange *= 1.3f;      // Lv2: Tăng 30% tầm bắn
            damageMultiplier = 1.5f;  // Lv2: Tăng 50% sát thương
        }
        else if (level == 3)
        {
            attackRange *= 1.6f;      // Lv3: Tăng 60% tầm bắn
            damageMultiplier = 2.2f;  // Lv3: x2.2 lần sát thương (Siêu cấp)
        }
        Debug.Log($"Con tướng vừa đặt có Tầm bắn: {attackRange} và Hệ số dame: {damageMultiplier}");
    }
}
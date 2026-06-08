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

    // Biến đếm thời gian hồi máu cho riêng Hệ Thủy
    private float healTimer = 0f;
    private float damageMultiplier = 1f; // Mặc định Lv1 là x1 sát thương

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- ĐẶC QUYỀN HỆ THỦY: HỒI 1% MÁU CASTLE / GIÂY ---
        if (element == ElementType.Thuy && Castle.Instance != null)
        {
            healTimer += Time.deltaTime;
            if (healTimer >= 1f)
            {
                float healAmount = Castle.Instance.maxHealth * 0.01f; // 1% máu tối đa
                Castle.Instance.Heal(healAmount);
                healTimer = 0f; // Reset thời gian
            }
        }

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

        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Projectile projScript = arrow.GetComponent<Projectile>();

            if (projScript != null)
            {
                // THAY ĐỔI: Gọi hàm Setup mới để truyền cả Mục tiêu lẫn Hệ nguyên tố vào viên đạn
                projScript.SetupProjectile(currentTarget, element);
                // THÊM DÒNG NÀY: Nhân sát thương của viên đạn với hệ số cấp độ!
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
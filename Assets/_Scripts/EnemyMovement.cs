using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số sinh tồn")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Giao diện")]
    public Slider hpSlider;

    [Header("Chỉ số chiến đấu")]
    public float speed = 3f;
    public float damage = 10f;
    public int goldReward = 15;
    public EnemyAttackType attackType;

    [Header("Cài đặt Tấn công")]
    public float stopDistance = 3f;
    public float attackRate = 1.5f;
    private float nextAttackTime = 0f;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float baseSpeed;
    private Coroutine rootCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine burnCoroutine;
    private bool isAttacking = false;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetupLane(Transform laneParent)
    {
        int nodeCount = laneParent.childCount;
        waypoints = new Transform[nodeCount];
        for (int i = 0; i < nodeCount; i++) waypoints[i] = laneParent.GetChild(i);

        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            currentWaypointIndex = 0;
        }

        // Đã dọn dẹp code bị lặp ở đây
        isAttacking = false;
        currentHealth = maxHealth;
        baseSpeed = speed;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || Castle.Instance == null) return;

        if (isAttacking)
        {
            HandleAttack();
            return;
        }

        if (attackType == EnemyAttackType.Ranged)
        {
            float distanceToCastle = Vector2.Distance(transform.position, Castle.Instance.transform.position);
            if (distanceToCastle <= stopDistance)
            {
                isAttacking = true;
                return;
            }
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                isAttacking = true;
            }
        }
    }

    private void HandleAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            Castle.Instance.TakeDamage(damage);
            nextAttackTime = Time.time + attackRate;

            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Quái bị bắn! Trừ {amount} máu. Còn lại: {currentHealth}");

        if (hpSlider != null)
        {
            hpSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // THUỐC GIẢI Ở ĐÂY: Lột tag Enemy để Trọng tài không đếm nhầm xác chết!
        gameObject.tag = "Untagged";

        Debug.Log("Quái đã bị tiêu diệt!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGold(goldReward);
        }

        if (anim != null) anim.SetTrigger("Death");

        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;

        if (hpSlider != null) hpSlider.gameObject.SetActive(false);

        this.enabled = false;
        Destroy(gameObject, 1f);
    }

    // ================= BỘ 3 HIỆU ỨNG NGUYÊN TỐ =================

    public void ApplyRoot(float duration)
    {
        if (rootCoroutine != null) StopCoroutine(rootCoroutine);
        rootCoroutine = StartCoroutine(RootRoutine(duration));
    }
    IEnumerator RootRoutine(float duration)
    {
        speed = 0f;
        yield return new WaitForSeconds(duration);
        speed = baseSpeed;
    }

    public void ApplySlow(float duration)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(duration));
    }
    IEnumerator SlowRoutine(float duration)
    {
        speed = baseSpeed * 0.5f;
        yield return new WaitForSeconds(duration);
        speed = baseSpeed;
    }

    public void ApplyBurn(float duration)
    {
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        burnCoroutine = StartCoroutine(BurnRoutine(duration));
    }
    IEnumerator BurnRoutine(float duration)
    {
        float elapsed = 0f;
        float damagePerTick = maxHealth * 0.05f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(damagePerTick);
            elapsed += 1f;
        }
    }
}
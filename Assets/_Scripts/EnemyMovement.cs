using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số sinh tồn")]
    public float maxHealth = 50f; // Máu tối đa
    private float currentHealth;

    [Header("Chỉ số chiến đấu")]
    public float speed = 3f;
    public float damage = 10f;
    public EnemyAttackType attackType; // Chú ý: Của mày đang dùng EnemyAttackType

    [Header("Cài đặt Tấn công")]
    public float stopDistance = 3f; // Khoảng cách dừng (dành cho Ranged)
    public float attackRate = 1.5f; // Tốc độ đánh (1.5s đánh 1 lần)
    private float nextAttackTime = 0f;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isAttacking = false;

    // 1. KHAI BÁO BIẾN ANIMATOR
    private Animator anim;

    // 2. LẤY ANIMATOR KHI QUÁI VỪA SINH RA
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

        // Khởi tạo trạng thái ban đầu mỗi khi đẻ quái
        isAttacking = false;
        currentHealth = maxHealth; // Bơm đầy máu
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || Castle.Instance == null) return;

        // Nếu đang trong trạng thái Tấn công thì đứng im và nã sát thương
        if (isAttacking)
        {
            HandleAttack();
            return;
        }

        // LOGIC TẦM XA (Ranged): Kiểm tra khoảng cách để phanh lại
        if (attackType == EnemyAttackType.Ranged)
        {
            float distanceToCastle = Vector2.Distance(transform.position, Castle.Instance.transform.position);
            if (distanceToCastle <= stopDistance)
            {
                isAttacking = true; // Chuyển sang trạng thái tấn công
                return;
            }
        }

        // ĐI BỘ
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;

            // LOGIC CẬN CHIẾN (Melee): Khi đã đi đến điểm cuối cùng (Lâu đài)
            if (currentWaypointIndex >= waypoints.Length)
            {
                isAttacking = true; // Đứng lại và gõ lâu đài liên tục
            }
        }
    }

    private void HandleAttack()
    {
        // Cả Melee và Ranged đều dùng chung hàm này để gây sát thương
        if (Time.time >= nextAttackTime)
        {
            Castle.Instance.TakeDamage(damage);
            nextAttackTime = Time.time + attackRate;

            // KÍCH HOẠT ANIMATION TẤN CÔNG
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    // HÀM NHẬN SÁT THƯƠNG TỪ HERO BẮN VÀO
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Quái bị bắn! Trừ {amount} máu. Còn lại: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Quái đã bị tiêu diệt!");

        // 1. Kích hoạt animation ngã gục (Đã sửa lỗi comment ở đây)
        if (anim != null)
        {
            anim.SetTrigger("Death");
        }

        // 2. Tắt va chạm để đạn không bay vào con quái đã chết nữa
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        // 3. Tắt luôn script này để nó không chạy lên trước hay chém Lâu đài nữa
        this.enabled = false;

        // 4. Đợi 1 giây (cho nó diễn xong animation Chết) rồi mới hủy Object
        Destroy(gameObject, 1f);

        // TODO: Cộng Vàng vào đây (Làm sau)
    }
}
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;
    public float damage = 15f;

    // Biến lưu hệ nguyên tố do Tướng truyền vào
    private ElementType elementType;

    // HÀM MỚI: Tiếp nhận mục tiêu và Hệ từ Tướng bắn ra
    public void SetupProjectile(Transform _target, ElementType _element)
    {
        target = _target;
        elementType = _element;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            EnemyMovement enemy = hitInfo.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                // 1. XỬ LÝ SÁT THƯƠNG BAN ĐẦU
                float finalDamage = damage;

                // Quy tắc Hệ Kim: Sát thương x2 tướng khác
                if (elementType == ElementType.Kim)
                {
                    finalDamage = damage * 2f;
                    Debug.Log("<color=yellow>💥 CHÍ MẠNG HỆ KIM! x2 Sát thương!</color>");
                }

                enemy.TakeDamage(finalDamage); // Quái nhận dame

                // 2. XỬ LÝ HIỆU ỨNG ĐI KÈM CỦA CÁC HỆ KHÁC
                switch (elementType)
                {
                    case ElementType.Moc:
                        enemy.ApplyRoot(1f); // Trói chân đứng im 1 giây
                        Debug.Log("<color=green>🌿 Hệ Mộc: Trói chân quái 1 giây!</color>");
                        break;

                    case ElementType.Hoa:
                        enemy.ApplyBurn(3f); // Đốt cháy trong 3 giây (mỗi giây trừ 5% max HP)
                        Debug.Log("<color=red>🔥 Hệ Hỏa: Đốt cháy mục tiêu!</color>");
                        break;

                    case ElementType.Tho:
                        enemy.ApplySlow(1f); // Sa lầy giảm 50% tốc độ trong 1 giây
                        Debug.Log("<color=brown>⏳ Hệ Thổ: Làm chậm quái 50%!</color>");
                        break;
                }
            }
            Destroy(gameObject); // Hủy mũi tên sau khi nổ
        }
    }
}
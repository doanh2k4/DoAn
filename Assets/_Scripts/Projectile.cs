using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;
    public float damage = 15f;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Quái chết trước khi tên tới -> Hủy tên
            return;
        }

        // Tính hướng bay
        Vector2 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // Xoay mũi tên (arrow.png của cậu đầu nhọn hướng sang phải)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Bay tới
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            EnemyMovement enemy = hitInfo.GetComponent<EnemyMovement>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject); // Nổ mũi tên
        }
    }
}
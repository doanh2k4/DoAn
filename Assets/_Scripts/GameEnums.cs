public enum ElementType
{
    Kim,   // Sát thương cao
    Moc,   // Trói chân 1s
    Thuy,  // Bong bóng (Stun) 1s
    Hoa,   // Đốt 5% Max HP
    Tho    // Giảm 50% tốc độ 1s
}

public enum EnemyClass
{
    Tank,       // Trâu bò, di chuyển chậm
    Speed,      // Máu giấy, di chuyển nhanh
    Balanced,   // Cân bằng
    Swarm,      // Sinh ra theo bầy đàn
    Ranged      // Đứng từ xa bắn
}

public enum EnemyAttackType
{
    Melee,      // Cận chiến (Lao vào chạm lâu đài)
    Ranged      // Tầm xa (Dừng lại bắn)
}
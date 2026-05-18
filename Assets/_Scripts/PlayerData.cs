using System;

[Serializable]
public class PlayerData
{
    public string username;
    public double currentGold;
    public int castleLevel;
    public int highestClearedWave;
    public string lastLoginTime;

    public int[] elementLevels;

    public PlayerData()
    {
        // ĐÃ SỬA DÒNG NÀY: Dùng System.Random thay vì UnityEngine.Random
        username = "Player_" + new System.Random().Next(1000, 9999);

        currentGold = 0;
        castleLevel = 1;
        highestClearedWave = 0;
        lastLoginTime = DateTime.Now.ToString();
        elementLevels = new int[5] { 1, 1, 1, 1, 1 };
    }
}
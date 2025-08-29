using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
public class LevelConfigSO : ScriptableObject
{
    public int Level;   
    public int PointsRequired;
    public RewardData[] PosibleRewards;
}
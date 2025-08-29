using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RewardData", menuName = "Scriptable Objects/RewardData")]
public class RewardData : ScriptableObject
{
    public enum RewardType
    {
        DamageIncrease,
        HitSpeedIncrease,
        CritDamageIncrease,
        CritChanceIncrease,
        NewHammer
    }

    public RewardType rewardType;
    public float value; 
    public Sprite icon;
    public string description;
    public HammerData newHammer;
}


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSection : MonoBehaviour
{
    [SerializeField] private Image icon;
    //[SerializeField] private TextMeshProUGUI rewardText;

    [SerializeField] private RewardData reward;

    public void SetReward(RewardData rewardData)
    {
        reward = rewardData;
        Debug.Log(reward.rewardType);
        //rewardText.SetText(reward.rewardType.ToString());
        icon.sprite = reward.icon;
    }

    public RewardData GetReward()
    {
        return reward;
    }
}
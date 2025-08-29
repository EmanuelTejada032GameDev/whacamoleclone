using UnityEngine;

[CreateAssetMenu(fileName = "NewHammer", menuName = "Scriptable Objects/HammerData")]
public class HammerData : ScriptableObject
{
    public string hammerName;
    public float damage;
    public float speed;
    public Transform hammerModel;
}

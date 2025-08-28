using UnityEngine;

[CreateAssetMenu(fileName = "MoleSO", menuName = "Scriptable Objects/MoleSO")]
public class MoleConfig : ScriptableObject
{
    public string moleName;
    public GameObject moleModel; 
    public int maxHealth;
    public int scoreValue;

}

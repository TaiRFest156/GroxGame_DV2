using UnityEngine;

public enum ResidueKind
{
    Vidrio,
    Papel,
    Plastico
}

[CreateAssetMenu(fileName = "ResidueData", menuName = "Game/Residue")]
public class ResidueData : ScriptableObject
{
    public string residueName;

    //public Sprite icon;       
    public GameObject prefab; 
    public ResidueKind kind;

    public int scoreValue = 10;
    public int penaltyValue = -5;
}

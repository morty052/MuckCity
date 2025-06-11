using UnityEngine;


[System.Serializable]
public class SubstanceData
{
    public Substance _id;
    public SubstanceType _type;
    public int _count;

    public SubstanceSO Data;

    public SubstanceData(Substance id, SubstanceType type, int count, SubstanceSO data)
    {
        _id = id;
        _type = type;
        _count = count;
        Data = data;
    }
}

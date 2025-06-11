using UnityEngine;

public interface IDamageAble
{
    [SerializeField] public int HP { get; set; }


    public void TakeDamage(int damage);
    public void OnDeath();
}

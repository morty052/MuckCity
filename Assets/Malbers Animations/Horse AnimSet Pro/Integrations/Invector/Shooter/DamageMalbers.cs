using Invector;
using MalbersAnimations.Reactions;
using UnityEngine;

namespace MalbersAnimations
{
    public class DamageMalbers : MonoBehaviour
    {
        [SerializeReference, SubclassSelector]
        public Reaction reaction;
        public StatModifier stat = new();
     

        public void DealDamage(vDamage damage)
        {
            var Damageable = damage.receiver.gameObject.GetComponentInParent<MDamageable>();

            // Instantiate(InstantiateOnHit, damage.hitPosition, Quaternion.identity);

            if (Damageable)
            {
                Damageable.ReceiveDamage(transform.forward, transform.position, gameObject, stat, false, true, reaction, false, null);
            }
        }

        private void Reset()
        {
            stat = new StatModifier()
            {
                ID = MTools.GetInstance<StatID>("Health"),
                modify = StatOption.SubstractValue,
                Value = new MalbersAnimations.Scriptables.FloatReference(10)
            };
        }
    }
}

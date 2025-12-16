using UnityEngine;
using Random = UnityEngine.Random;

namespace Gun.Gun
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
    public class DamageConfigScriptableObject : ScriptableObject
    {
        public Vector2Int DefaultDamageRange;
        public Vector2Int DamageRange;

        private void Reset()
        {
            DamageRange = new Vector2Int(10, 20);
        }

        public int GetDamage()
        {
            return Random.Range(DamageRange.x, DamageRange.y);
        }
    }
}
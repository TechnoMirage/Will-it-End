using System.Collections.Generic;
using UnityEngine;

namespace Gun.Gun
{
    [CreateAssetMenu(fileName = "GunDatabase", menuName = "Database/Gun Database")]
    public class GunDatabase : ScriptableObject
    {
        public List<GunScriptableObject> guns = new();
    }
}
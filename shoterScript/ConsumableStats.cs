using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{

    [CreateAssetMenu(fileName = "ConsumableStats", menuName = "tutoriales/consumable stats")]

    public class ConsumableStats : ItemStats
    {
        public float Duration;
        public float EffectStrength;
        public string EffectStat;

        public int MaxUnits = 1;
        public int ConsumeRate = 1;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace tutoriales
{
    public class ConsumableItem : ItemController
    {

        public int Units = 1;

        private float deltaTime;
        private CharControllers usingChar;


        public ConsumableStats getConsumableStats()
        {
            if (Stats is ConsumableStats)
               return Stats as ConsumableStats;
            ConsumableStats defect = new ConsumableStats();
            Stats = defect;
            return defect;

            
        }

        public override void Use(CharControllers character)
        {

            //por lomenos uno para...
            ConsumableStats consumeStats = getConsumableStats();

            if ((Units - consumeStats.ConsumeRate) >= 0){

                usingChar = character;
                int index = usingChar.anim.GetLayerIndex(Stats.animLayer);
                usingChar.anim.Play(Stats.animation, index);
                usingChar.states.Consuming = true;

                usingChar.anim.SetLayerWeight(index, 1);
            }

           
        }

        private void FixedUpdate()
        {
            if (usingChar !=null)
            {
                deltaTime += Time.deltaTime;
                if (deltaTime >= getConsumableStats().Duration)
                {
                    Finish(false);
                }
                else
                {
                    int index = usingChar.anim.GetLayerIndex(Stats.animLayer);
                    usingChar.anim.SetLayerWeight(index, 1);
                }
            }
        }

        public float GetActualUseState()
        {
            return deltaTime / getConsumableStats().Duration;
        }

        public void Finish (bool interrupt)
        {
            usingChar.states.Consuming = false;
            int index = usingChar.anim.GetLayerIndex(Stats.animLayer);
            usingChar.anim.SetLayerWeight(index, 0);
           
            

            if (!interrupt)
            {

                ConsumableStats consumeStats = getConsumableStats();

                usingChar.Consume(consumeStats.EffectStrength, consumeStats.EffectStat);

                Units -= consumeStats.ConsumeRate;
                if (Units<=0)
                {

                    usingChar.inventary.RemoveItem(this);
                    Destroy(this.gameObject);
                }


            }

            usingChar = null;
            deltaTime = 0;

        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class WeaponController : ItemController
    {


        public virtual bool Attack(CharControllers character)
        {
            return false;
        }


        }
    }

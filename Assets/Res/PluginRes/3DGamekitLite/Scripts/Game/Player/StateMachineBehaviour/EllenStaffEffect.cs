﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class EllenStaffEffect : StateMachineBehaviour
    {
        public int effectIndex;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController ctrl = animator.GetComponent<PlayerController>();
            if(ctrl == null) return;
            ctrl.meleeWeapon.effects[effectIndex].Activate();
        }

    } 
}

using UnityEngine;

namespace RealDream
{
    [System.Serializable]
    public struct AnimationInfo
    {
        //[HorizontalGroup, LabelWidth(40)] 
        public string Name;

        //[HorizontalGroup, LabelWidth(30)] 
        public AnimationClip Clip;
    }
}
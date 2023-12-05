using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RealDream.AI
{
    public enum EDataType
    {
        Bool,
        Int32,
        Float32,
        String,
        Vector3,
        Actor
    }

    public class UData
    {
        public EDataType Type;
        public bool ValueBool;
        public int ValueInt32;
        public float ValueFloat32;
        public string ValueString;
        public Vector3 ValueVector3;
        public Actor ValueActor;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UData(bool v) => new UData(){Type = EDataType.Bool, ValueBool = v};
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UData(int v) => new UData(){Type = EDataType.Int32, ValueInt32 = v};
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UData(float v) => new UData(){Type = EDataType.Float32, ValueFloat32 = v};
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UData(string v) => new UData(){Type = EDataType.String, ValueString = v};
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UData(Vector3 v) => new UData(){Type = EDataType.Vector3, ValueVector3 = v};
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UData(Actor v) => new UData(){Type = EDataType.Actor, ValueActor = v};

        public bool IsBool => Type == EDataType.Bool;
        public bool IsInt32 => Type == EDataType.Int32;
        public bool IsFloat32 => Type == EDataType.Float32;
        public bool IsString => Type == EDataType.String;
        public bool IsVector3 => Type == EDataType.Vector3;
        public bool IsActor => Type == EDataType.Actor;


        public override string ToString()
        {
            switch(Type)
            {
                case EDataType.Bool:
                    return ValueBool.ToString();
                case EDataType.Int32:
                    return ValueInt32.ToString();
                case EDataType.Float32:
                    return ValueFloat32.ToString(CultureInfo.InvariantCulture);
                case EDataType.String:
                    return ValueString;
                case EDataType.Vector3:
                    return ValueVector3.ToString();
                case EDataType.Actor:
                    return ValueActor.ToString();
            }
            return base.ToString();
        }
    }
}
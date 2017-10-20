using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NPCStats", menuName = "SharedConfig/NPCStats")]
public class NPCStats : ScriptableObject
{
    [Serializable]
    public class MovementData
    {
        [Range(0f, 10f)] public float bobSpeed = 5f;
        [Range(0f, 0.5f)] public float bobRange = 0.242f;
    }

    public MovementData movementData;

    [Serializable]
    public class PhysicsData
    {
        [Range(0f, 30f)] public float pushOffForce = 20f;
    }

    public PhysicsData physicsData;
}

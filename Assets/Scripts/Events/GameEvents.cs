using UnityEngine;

namespace Events
{
    public struct LoopStartEvent { }

    public struct LoopEndEvent
    {
        public string Reason; // e.g., "time_expired", "player_death"
    }

    public struct MinutePassedEvent
    {
        public int MinutesRemaining;
    }

    public struct PlayerNoiseEvent
    {
        public float NoiseLevel;
        public bool IsMoving;
        public Vector3 Position;
    }
}

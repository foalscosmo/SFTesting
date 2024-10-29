using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class SW_AIStrength
    {
        public SW_StrengthType strength;
        
        [Header("SW random stats")] 
        [Range(0.5f, 5f)]
        public float minBoostTime = 0.5f;
        [Range(1.0f, 10f)]
        public float maxBoostTime = 2f;
        [Range(25.0f, 50f)]
        public float minBoostAngle = 25f;
        [Range(25.0f, 100f)]
        public float maxBoostAngle = 60f;
        [Range(0.1f, 5f)]
        public float minAITurnDecisionTime = 0.3f;
        [Range(0.5f, 10f)]
        public float maxAITurnDecisionTime = 1.5f;
        [Range(0.1f, 5f)]
        public float targetPickUpDecisionTime = 1f;
        [Range(0.1f, 10f)]
        public float minPlayerLikeActionTimer = 5f;
        [Range(0.1f, 20f)]
        public float maxPlayerLikeActionTimer = 5f;
    }

    [System.Serializable]
    public enum SW_StrengthType
    {
        Weak,
        Normal,
        Strong
    }
}

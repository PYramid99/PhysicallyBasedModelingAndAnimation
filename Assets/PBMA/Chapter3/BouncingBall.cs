using UnityEngine;

namespace PBMA.Chapter3
{
    public class BouncingBall : MonoBehaviour
    {
        [Min(0.001f)]
        public float radius = 0.5f;
        
        [Min(0.001f)]
        public float mass = 1.0f;
        
        [Range(0.0f, 1.0f)]
        public float restitution = 0.5f;

        [Min(0.0f)]
        public float friction = 0.5f;
        
        public Vector3 Velocity { get; set; } = Vector3.zero;
        
        public Vector3 Position { get; set; } = Vector3.zero;

        public float ActiveTime { get; set; }

        private void OnEnable()
        {
            Position = transform.position;
            ActiveTime = 0.0f;

            if (BouncingBallSolution.Instance)
            {
                BouncingBallSolution.Instance.AddBouncingBall(this);
            }
        }

        private void Update()
        {
            Debug.Log($"[{ActiveTime} | {name}] Velocity: {Velocity}; Position: {Position}");
        }

        private void OnDisable()
        {
            if (BouncingBallSolution.Instance)
            {
                BouncingBallSolution.Instance.RemoveBouncingBall(this);
            }
        }
    }
}

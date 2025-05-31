using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBMA.Chapter2
{
    public class FallBody : MonoBehaviour
    {
        public enum SolutionType
        {
            Exact,
            Euler
        }

        public SolutionType Type = SolutionType.Exact;

        public float Mass = 1.0f;

        private Vector3 force = Vector3.zero;

        public Vector3 Force
        {
            get { return force; }
            set { force = value; }
        }

        private float activeTime = 0.0f;

        public float ActiveTime
        {
            get { return activeTime; }
            set { activeTime = value; }
        }

        private Vector3 velocity = Vector3.zero;

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private Vector3 position = Vector3.zero;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        private void OnEnable()
        {
            Position = transform.position;

            if (FallBodySolution.Instance != null)
            {
                FallBodySolution.Instance.AddFallBody(this);
            }
        }

        private void Update()
        {
            Debug.Log($"[{name}] Time: {ActiveTime}; Velocity: {Velocity}; Position: {transform.position}");
        }

        private void OnDisable()
        {
            if (FallBodySolution.Instance != null)
            {
                FallBodySolution.Instance.RemoveFallBody(this);
            }

            activeTime = 0.0f;
        }
    }
}

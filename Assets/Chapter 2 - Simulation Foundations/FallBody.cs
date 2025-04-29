using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chapter2
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

        private Vector3 _force = Vector3.zero;

        public Vector3 force
        {
            get { return _force; }
            set { _force = value; }
        }

        private float _activeTime = 0.0f;

        public float activeTime
        {
            get { return _activeTime; }
            set { _activeTime = value; }
        }

        private Vector3 _velocity = Vector3.zero;

        public Vector3 velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private Vector3 _position = Vector3.zero;

        public Vector3 position
        {
            get { return _position; }
            set { _position = value; }
        }

        private void OnEnable()
        {
            position = transform.position;

            FallBodySolution.instance?.AddFallBody(this);
        }

        private void Update()
        {
            Debug.Log($"[{name}] Time: {activeTime}; Velocity: {velocity}; Position: {transform.position}");
        }

        private void OnDisable()
        {
            FallBodySolution.instance?.RemoveFallBody(this);

            _activeTime = 0.0f;
        }
    }
}

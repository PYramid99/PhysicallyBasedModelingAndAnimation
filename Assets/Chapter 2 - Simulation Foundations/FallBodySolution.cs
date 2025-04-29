using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter2
{
    public class FallBodySolution : SingletonMonoBehaviour<FallBodySolution>
    {
        public Vector3 Grivaty = new(0.0f, -10.0f, 0.0f);

        private readonly List<FallBody> _fallBodies = new();

        private void FixedUpdate()
        {
            foreach (var body in _fallBodies)
            {
                body.ActiveTime += Time.fixedDeltaTime;

                IntegralForce(body);

                switch (body.Type)
                {
                    case FallBody.SolutionType.Exact:
                        SolveExactFall(body);
                        break;
                    case FallBody.SolutionType.Euler:
                        SolveEulurFall(body);
                        break;
                    default:
                        break;
                }

                ClearForce(body);
            }
        }

        public void AddFallBody(FallBody body)
        {
            _fallBodies.Add(body);
        }

        public void RemoveFallBody(FallBody body)
        {
            _fallBodies.Remove(body);
        }

        private void IntegralForce(FallBody body)
        {
            var force = body.Mass * Grivaty;

            if (body.TryGetComponent<FallBodyDrag>(out var dragComp))
            {
                force -= dragComp.Drag * body.Velocity;

                if (body.TryGetComponent<FallBodyWind>(out var windComp))
                {
                    force += dragComp.Drag * windComp.Velocity;
                }
            }

            body.Force = force;
        }

        private void SolveExactFall(FallBody body)
        {
            var acceleration = Grivaty;

            body.Velocity = acceleration * body.ActiveTime;

            body.transform.position = 0.5f * Mathf.Pow(body.ActiveTime, 2.0f) * acceleration + body.Position;
        }

        private void SolveEulurFall(FallBody body)
        {
            var acceleration = body.Force / body.Mass;

            var newVelocity = body.Velocity + acceleration * Time.fixedDeltaTime;

            body.transform.position = body.transform.position + 0.5f * Time.fixedDeltaTime * (body.Velocity + newVelocity);

            body.Velocity = newVelocity;
        }

        private void ClearForce(FallBody body)
        {
            body.Force = Vector3.zero;
        }
    }
}

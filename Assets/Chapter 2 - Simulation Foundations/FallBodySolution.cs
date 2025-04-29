using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter2
{
    public class FallBodySolution : SingletonMonoBehaviour<FallBodySolution>
    {
        public Vector3 Grivaty = new Vector3(0.0f, -10.0f, 0.0f);

        private List<FallBody> _fallBodies = new List<FallBody>();

        private void FixedUpdate()
        {
            foreach (var body in _fallBodies)
            {
                body.activeTime += Time.fixedDeltaTime;

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

            var dragComp = body.GetComponent<FallBodyDrag>();
            if (dragComp != null)
            {
                force -= dragComp.Drag * body.velocity;

                var windComp = body.GetComponent<FallBodyWind>();
                if (windComp != null)
                {
                    force += dragComp.Drag * windComp.Velocity;
                }
            }

            body.force = force;
        }

        private void SolveExactFall(FallBody body)
        {
            var acceleration = Grivaty;

            body.velocity = acceleration * body.activeTime;

            body.transform.position = 0.5f * acceleration * Mathf.Pow(body.activeTime, 2.0f) + body.position;
        }

        private void SolveEulurFall(FallBody body)
        {
            var acceleration = body.force / body.Mass;

            var newVelocity = body.velocity + acceleration * Time.fixedDeltaTime;

            body.transform.position = body.transform.position + 0.5f * (body.velocity + newVelocity) * Time.fixedDeltaTime;

            body.velocity = newVelocity;
        }

        private void ClearForce(FallBody body)
        {
            body.force = Vector3.zero;
        }
    }
}

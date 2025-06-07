using System.Collections.Generic;
using UnityEngine;
using PBMA.Core;
using Unity.Mathematics;

namespace PBMA.Chapter3
{
    public class BouncingBallSolution : SingletonMonoBehaviour<BouncingBallSolution>
    {
        private readonly List<BouncingBall> _bouncingBalls = new();

        private readonly List<Plane> _planes = new();

        private readonly Vector3 _grivaty = new(0.0f, -10.0f, 0.0f);

        private struct BouncingBallWorkspace
        {
            public Vector3 PrevPosition;
            public Vector3 PrevVelocity;
            public Vector3 CurrPosition;
            public Vector3 CurrVelocity;
            public BouncingBall Ball;
            public Plane CollisionPlane;
            public float Alpha;
        }

        private void FixedUpdate()
        {
            foreach (var ball in _bouncingBalls)
            {
                var timeStepRemaining = Time.fixedDeltaTime;

                BouncingBallWorkspace workspace = new()
                {
                    PrevPosition = ball.Position,
                    PrevVelocity = ball.Velocity,
                    CurrPosition = ball.Position,
                    CurrVelocity = ball.Velocity,
                    Ball = ball,
                    CollisionPlane = null,
                    Alpha = 1.0f
                };
                
                while (math.abs(timeStepRemaining) > math.EPSILON)
                {
                    if (workspace.CurrVelocity.magnitude < 1e-4f && workspace.CollisionPlane)
                    {
                        var normal = workspace.CollisionPlane.Normal;
                        var plane = new UnityEngine.Plane(normal, workspace.CollisionPlane.transform.position);

                        var side = plane.GetSide(workspace.PrevPosition);
                        var distance = plane.GetDistanceToPoint(workspace.CurrPosition) +
                                       (side ? -workspace.Ball.radius : workspace.Ball.radius);

                        if (distance < math.EPSILON)
                        {
                            if (Vector3.Dot(Vector3.down, normal) < math.EPSILON)
                            {
                                var force = workspace.Ball.mass * _grivaty;
                                var verticalForce = Vector3.Dot(force, -normal) * -normal;
                                var horizontalForce = force - verticalForce;

                                if (horizontalForce.magnitude < workspace.Ball.friction * verticalForce.magnitude)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    
                    var timeStep = timeStepRemaining;

                    Integrate(ref workspace, timeStep);
                    
                    if (CollisionDetection(ref workspace))
                    {
                        timeStep *= workspace.Alpha;

                        Integrate(ref workspace, timeStep);

                        CollisionResponse(ref workspace);
                    }

                    timeStepRemaining -= timeStep;

                    workspace.PrevPosition = workspace.CurrPosition;
                    workspace.PrevVelocity = workspace.CurrVelocity;
                }

                ball.Position = workspace.CurrPosition;
                ball.Velocity = workspace.CurrVelocity;
                ball.ActiveTime += Time.fixedDeltaTime;

                ball.transform.position = ball.Position;
            }
        }

        public void AddBouncingBall(BouncingBall bouncingBall)
        {
            _bouncingBalls.Add(bouncingBall);

            Debug.Log($"Balls: {_bouncingBalls.Count}");
        }

        public void RemoveBouncingBall(BouncingBall bouncingBall)
        {
            _bouncingBalls.Remove(bouncingBall);

            Debug.Log($"Balls: {_bouncingBalls.Count}");
        }

        public void AddPlane(Plane plane)
        {
            _planes.Add(plane);

            Debug.Log($"Planes: {_planes.Count}");
        }

        public void RemovePlane(Plane plane)
        {
            _planes.Remove(plane);

            Debug.Log($"Plane: {_planes.Count}");
        }

        private void Integrate(ref BouncingBallWorkspace workspace, float timeStep)
        {
            workspace.CurrVelocity = workspace.PrevVelocity + _grivaty * timeStep;

            workspace.CurrPosition = workspace.PrevPosition +
                                     (workspace.PrevVelocity + workspace.CurrVelocity) * (0.5f * timeStep);
        }

        private bool CollisionDetection(ref BouncingBallWorkspace workspace)
        {
            workspace.CollisionPlane = null;
            workspace.Alpha = 1.0f;
            
            foreach (var plane in _planes)
            {
                var uPlane = new UnityEngine.Plane(plane.Normal, plane.transform.position);
                var prevSide = uPlane.GetSide(workspace.PrevPosition);
                var currSide = uPlane.GetSide(workspace.CurrPosition);
                var prevDistance = uPlane.GetDistanceToPoint(workspace.PrevPosition) +
                                   (prevSide ? -workspace.Ball.radius : workspace.Ball.radius);
                var currDistance = uPlane.GetDistanceToPoint(workspace.CurrPosition) +
                                   (currSide ? -workspace.Ball.radius : workspace.Ball.radius);

                if ((prevSide == currSide) && ((!(prevDistance > 0.0f) || !(currDistance < 0.0f)) &&
                                               (!(prevDistance < 0.0f) || !(currDistance > 0.0f))))
                {
                    continue;
                }

                var alpha = prevDistance / (prevDistance - currDistance);

                if (!workspace.CollisionPlane && !(workspace.Alpha > alpha))
                {
                    continue;
                }

                workspace.Alpha = alpha;
                workspace.CollisionPlane = plane;
            }

            if (workspace.CollisionPlane)
            {
                return true;
            }

            return false;
        }

        private void CollisionResponse(ref BouncingBallWorkspace workspace)
        {
            var normal = workspace.CollisionPlane.Normal;

            var currVelocity = workspace.CurrVelocity;
            var currVerticalVelocity = Vector3.Dot(currVelocity, normal) * normal;
            var currHorizontalVelocity = currVelocity - currVerticalVelocity;

            var newVerticalVelocity = -workspace.Ball.restitution * currVerticalVelocity;
            var newHorizontalVelocity = currHorizontalVelocity -
                                        math.min(workspace.Ball.friction * currVerticalVelocity.magnitude,
                                            currHorizontalVelocity.magnitude) * currHorizontalVelocity.normalized;

            workspace.CurrVelocity = newHorizontalVelocity + newVerticalVelocity;
        }
    }
}
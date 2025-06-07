using UnityEngine;

namespace PBMA.Chapter3
{
    public class Plane : MonoBehaviour
    { 
        public Vector3 Normal => transform.up;

        private void OnEnable()
        {
            if (BouncingBallSolution.Instance)
            {
                BouncingBallSolution.Instance.AddPlane(this);
            }
        }

        private void OnDisable()
        {
            if (BouncingBallSolution.Instance)
            {
                BouncingBallSolution.Instance.RemovePlane(this);
            }
        }
    }
}
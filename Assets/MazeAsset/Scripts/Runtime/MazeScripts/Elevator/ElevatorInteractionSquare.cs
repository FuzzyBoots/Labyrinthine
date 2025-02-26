using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    internal class ElevatorInteractionSquare : MonoBehaviour
    {
        [SerializeField] internal SquarePlatformElevator platformReference;

        public void SetPlatform(SquarePlatformElevator platform)
        {
            platformReference = platform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                platformReference.Interact(this.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                platformReference.StopInteract(this.gameObject);
            }
        }
    }
}
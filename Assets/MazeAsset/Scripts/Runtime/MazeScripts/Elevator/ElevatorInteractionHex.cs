using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    internal class ElevatorInteractionHex : MonoBehaviour
    {
        [SerializeField] internal HexagonPlatformElevator platformReference;

        public void SetPlatform(HexagonPlatformElevator platform)
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

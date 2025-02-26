
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public interface IElevatorPlatform
    {
        public float HeightBoxForInteract => 0.5f;
        public float GetOffsetY => 0.005f;

        public GameObject GetPlatform(float scalerY);
        public void DeleteElevator();
        void AddGOToGOManager(GameObject elevator, int x, int y, int z);
        internal bool RemoveGOToGOManager(int x, int y, int z);
        internal GameObject GetGOFromManager(int x, int y, int z);

        void ReinitData();
    }
}
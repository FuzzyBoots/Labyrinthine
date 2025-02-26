using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif
namespace MazeAsset.MazeGenerator
{
    [RequireComponent(typeof(MazeGeneratorManager))]
    [ExecuteInEditMode]
    public class MazeColor : MonoBehaviour
    {
        [SerializeField]
        internal Material mazeMaterial;
        [SerializeField]
        internal Material elevatorMaterial;
        [SerializeField]
        internal Material editMaterial;
        [SerializeField]
        internal ShaderSpecificadion[] specificadions;
        [SerializeField, HideInInspector] internal bool hex;
        [SerializeField, HideInInspector] internal Vector2 scaler;

        private T[] FillArray<T>(T[] inputArray, int targetLength = 12)
        {
            T[] resultArray = new T[targetLength];
            for (int x = 0; x < inputArray.Length && x < targetLength; x++)
            {
                resultArray[x] = inputArray[x];
            }
            return resultArray;
        }

#if UNITY_EDITOR
        private void OnSceneSaved(Scene scene)
        {
            SetMaterial();
        }
#endif
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
            EditorSceneManager.sceneSaved += OnSceneSaved;
#endif
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
#if UNITY_EDITOR
            EditorSceneManager.sceneSaved -= OnSceneSaved;
#endif
        }
        internal void UpdateMaterial()
        {
            SetMaterial();
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetMaterial();
        }
        internal void UpdateSpecificationArray()
        {
            var mazeGenerator = GetComponent<MazeGeneratorManager>();
            var size = mazeGenerator.sizeArray;
            specificadions = FillArray(specificadions, size);
        }
        private void OnValidate()
        {
            SetMaterial();
        }
        void SetMaterial()
        {
            if (mazeMaterial != null && specificadions.Length > 0)
            {
                mazeMaterial.SetFloatArray("_HeightFloorTexture", FillArray(specificadions.Select(o => o.heightFloorTexture).ToArray()));
                mazeMaterial.SetFloatArray("_Outline", FillArray(specificadions.Select(o => o.outline).ToArray()));
                mazeMaterial.SetFloatArray("_HeightAnotherColor", FillArray(specificadions.Select(o => o.heightWallColor).ToArray()));
                mazeMaterial.SetFloatArray("_CellSize", FillArray(specificadions.Select(o => o.scalerTexture).ToArray()));
                mazeMaterial.SetColorArray("_Color", FillArray(specificadions.Select(o => o.colorWall).ToArray()));
                mazeMaterial.SetColorArray("_ColorOutline", FillArray(specificadions.Select(o => o.colorOutline).ToArray()));

                mazeMaterial.SetFloat("_ScalerWidth", scaler.x);
                mazeMaterial.SetFloat("_ScalerHeight", scaler.y);

                mazeMaterial.SetInt("_Hex", hex ? 1 : 0);
                for (int i = 0; i < specificadions.Length; i++)
                {
                    mazeMaterial.SetTexture("TexturesArray" + i, specificadions[i].floorTexture);
                }
            }
        }
    }
}
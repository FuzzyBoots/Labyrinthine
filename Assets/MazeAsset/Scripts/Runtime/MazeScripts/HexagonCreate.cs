using UnityEditor;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    internal class HexagonCreate
    {

        internal Mesh CreateMeshHexagon(float height, float radius)
        {
            int sides = 6;
            int verticesCount = sides * 2 + 2;
            int trianglesCount = sides * 12;
            Vector3[] vertices = new Vector3[verticesCount];
            int[] triangles = new int[trianglesCount];
            vertices[0] = new Vector3(0, 0, 0); // Center of hexagon down
            vertices[verticesCount / 2] = new Vector3(0, height, 0); // Center of hexagon up

            for (int i = 0; i < sides; i++)
            {
                float angle = 2 * Mathf.PI * i / sides;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                // Down hexagon
                vertices[i + 1] = new Vector3(x, 0, z);

                // Up hexagon
                vertices[i + 1 + sides] = new Vector3(x, height, z);
            }
            // down hexagon
            for (int i = 0; i < sides; i++)
            {
                int nextIndex = (i + 1) % sides;

                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = nextIndex + 1;
            }
            // Up hexagon
            for (int i = 0; i < sides; i++)
            {
                int nextIndex = (i + 1) % sides;

                triangles[(sides + i) * 3] = verticesCount / 2;
                triangles[(sides + i) * 3 + 1] = nextIndex + 1 + sides;
                triangles[(sides + i) * 3 + 2] = i + 1 + sides;
            }
            // Side walls - visible from outside
            for (int i = 0; i < sides; i++)
            {
                int nextIndex = (i + 1) % sides;

                triangles[sides * 6 + i * 6] = i + 1;
                triangles[sides * 6 + i * 6 + 1] = i + 1 + sides;
                triangles[sides * 6 + i * 6 + 2] = nextIndex + 1;

                triangles[sides * 6 + i * 6 + 3] = nextIndex + 1;
                triangles[sides * 6 + i * 6 + 4] = i + 1 + sides;
                triangles[sides * 6 + i * 6 + 5] = nextIndex + 1 + sides;
            }


            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}

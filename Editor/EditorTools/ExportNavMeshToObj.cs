//-----------------------------------------------------------------------
// <copyright file="ExportNavMeshToObj.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// Code found here: https://forum.unity.com/threads/accessing-navmesh-vertices.130883
// Obj exporter component based on: http://wiki.unity3d.com/index.php?title=ObjExporter

#if UNITY_EDITOR

namespace Lost
{
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public static class ExportNavMeshToObj
    {
        [MenuItem("Tools/Lost/Tools/Export Scene NavMesh to Mesh")]
        private static void Export()
        {
            UnityEngine.AI.NavMeshTriangulation triangulatedNavMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();

            Mesh mesh = new Mesh();
            mesh.name = "ExportedNavMesh";

            // Flipping along x-axis
            var navMeshVerts = triangulatedNavMesh.vertices;

            for (int i = 0; i < navMeshVerts.Length; i++)
            {
                navMeshVerts[i] = navMeshVerts[i].SetX(-navMeshVerts[i].x);
            }

            // Changing triangle winding order
            var navMeshTriangles = triangulatedNavMesh.indices;

            for (int i = 0; i < navMeshTriangles.Length; i += 3)
            {
                int index1 = navMeshTriangles[i + 0];
                int index2 = navMeshTriangles[i + 1];
                int index3 = navMeshTriangles[i + 2];

                navMeshTriangles[i + 0] = index3;
                navMeshTriangles[i + 1] = index2;
                navMeshTriangles[i + 2] = index1;
            }

            mesh.vertices = navMeshVerts;
            mesh.triangles = navMeshTriangles;

            string filename = EditorSceneManager.GetActiveScene().path;
            filename = filename.Substring(0, filename.Length - 6); // Removing ".scene"
            filename = filename + ".obj";

            MeshToFile(mesh, filename);
            Debug.Log("NavMesh exported to '" + filename + "'");
            AssetDatabase.Refresh();
        }

        private static string MeshToString(Mesh mesh)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mesh.name).Append("\n");
            foreach (Vector3 v in mesh.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }

            sb.Append("\n");

            foreach (Vector3 v in mesh.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }

            sb.Append("\n");

            foreach (Vector3 v in mesh.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }

            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                sb.Append("\n");

                int[] triangles = mesh.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }

            return sb.ToString();
        }

        private static void MeshToFile(Mesh mesh, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString(mesh));
            }
        }
    }
}

#endif

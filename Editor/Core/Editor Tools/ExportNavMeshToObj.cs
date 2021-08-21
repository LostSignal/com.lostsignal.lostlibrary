#pragma warning disable

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
        [MenuItem("Tools/Lost/Tools/Export Scene NavMesh")]
        public static void ExportNavMeshForCurrentScene()
        {
            var sceneNavMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();

            Mesh mesh = new Mesh();
            mesh.name = "ExportedNavMesh";
            mesh.vertices = sceneNavMesh.vertices;
            mesh.triangles = sceneNavMesh.indices;

            string scenPath = EditorSceneManager.GetActiveScene().path;
            string fileName = Path.GetFileNameWithoutExtension(scenPath) + " Nav Mesh.obj";

            SimpleMeshToObjFile(mesh, fileName);
            Debug.Log("Nav Mesh exported to '" + scenPath + "'");
            AssetDatabase.Refresh();
        }

        public static void SimpleMeshToObjFile(Mesh mesh, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString(mesh));
            }

            string MeshToString(Mesh mesh)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("g ").Append(mesh.name).Append("\n");

                foreach (Vector3 v in mesh.vertices)
                {
                    // NOTE [bgish]: Flipped X-Axis
                    stringBuilder.Append(string.Format("v {0} {1} {2}\n", -v.x, v.y, v.z));
                }

                stringBuilder.Append("\n");

                foreach (Vector3 v in mesh.normals)
                {
                    // NOTE [bgish]: Flipped X-Axis
                    stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -v.x, v.y, v.z));
                }

                stringBuilder.Append("\n");

                foreach (Vector3 v in mesh.uv)
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", v.x, v.y));
                }

                for (int material = 0; material < mesh.subMeshCount; material++)
                {
                    stringBuilder.Append("\n");

                    int[] triangles = mesh.GetTriangles(material);

                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        // NOTE [bgish]: Reversed Winding Order
                        stringBuilder.Append(string.Format(
                            "f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                            triangles[i + 2] + 1,
                            triangles[i + 1] + 1,
                            triangles[i + 0] + 1));
                    }
                }

                return stringBuilder.ToString();
            }
        }
    }
}

#endif

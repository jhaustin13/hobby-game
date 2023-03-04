using System.Collections.Generic;
using UnityEngine;


public class MeshHelper
{
    public static Mesh ConvertSkeletonToMesh(MeshSkeleton meshSkeleton)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = meshSkeleton.Vertices.ToArray();
        mesh.triangles = meshSkeleton.Triangles.ToArray();


        Vector2[] uvs = new Vector2[meshSkeleton.Vertices.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(meshSkeleton.Vertices[i].x, meshSkeleton.Vertices[i].z);
        }

        mesh.uv = uvs;

        return mesh;
    }

    public static float VolumeOfMesh(MeshSkeleton mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.Vertices.ToArray();
        int[] triangles = mesh.Triangles.ToArray();

        for (int i = 0; i < mesh.Triangles.Count; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }
    public static float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }

    private static Vector3 GetRandomPointOnTriangle(MeshSkeleton mesh, int idx)
    {
        Vector3[] v = new Vector3[3];

        v[0] = mesh.Vertices[mesh.Triangles[3 * idx + 0]];
        v[1] = mesh.Vertices[mesh.Triangles[3 * idx + 1]];
        v[2] = mesh.Vertices[mesh.Triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        // Generate a random point in the trapezoid
        Vector3 result = v[0] + Random.Range(0f, 1f) * a + Random.Range(0f, 1f) * b;

        // Barycentric coordinates on triangles
        float alpha = ((v[1].z - v[2].z) * (result.x - v[2].x) + (v[2].x - v[1].x) * (result.z - v[2].z)) /
                ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float beta = ((v[2].z - v[0].z) * (result.x - v[2].x) + (v[0].x - v[2].x) * (result.z - v[2].z)) /
               ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float gamma = 1.0f - alpha - beta;

        // The selected point is outside of the triangle (wrong side of the trapezoid), project it inside through the center.
        if (alpha < 0 || beta < 0 || gamma < 0)
        {
            Vector3 center = v[0] + c / 2;
            center = center - result;
            result += 2 * center;
        }

        return result;
    }

    private static Vector3 GetRandomPointOnTriangle(Mesh mesh, int idx)
    {
        Vector3[] v = new Vector3[3];

        v[0] = mesh.vertices[mesh.triangles[3 * idx + 0]];
        v[1] = mesh.vertices[mesh.triangles[3 * idx + 1]];
        v[2] = mesh.vertices[mesh.triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        // Generate a random point in the trapezoid
        Vector3 result = v[0] + Random.Range(0f, 1f) * a + Random.Range(0f, 1f) * b;

        // Barycentric coordinates on triangles
        float alpha = ((v[1].z - v[2].z) * (result.x - v[2].x) + (v[2].x - v[1].x) * (result.z - v[2].z)) /
                ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float beta = ((v[2].z - v[0].z) * (result.x - v[2].x) + (v[0].x - v[2].x) * (result.z - v[2].z)) /
               ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float gamma = 1.0f - alpha - beta;

        // The selected point is outside of the triangle (wrong side of the trapezoid), project it inside through the center.
        if (alpha < 0 || beta < 0 || gamma < 0)
        {
            Vector3 center = v[0] + c / 2;
            center = center - result;
            result += 2 * center;
        }

        return result;
    }

    private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }


    private static float GetTriangleArea(MeshSkeleton mesh, int idx)
    {
        Vector3[] v = new Vector3[3];


        v[0] = mesh.Vertices[mesh.Triangles[3 * idx + 0]];
        v[1] = mesh.Vertices[mesh.Triangles[3 * idx + 1]];
        v[2] = mesh.Vertices[mesh.Triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        float ma = a.magnitude;
        float mb = b.magnitude;
        float mc = c.magnitude;

        float area = 0f;

        float S = (ma + mb + mc) / 2;
        area = Mathf.Sqrt(S * (S - ma) * (S - mb) * (S - mc));

        return area;
    }

    private static float GetTriangleArea(Mesh mesh, int idx)
    {
        Vector3[] v = new Vector3[3];


        v[0] = mesh.vertices[mesh.triangles[3 * idx + 0]];
        v[1] = mesh.vertices[mesh.triangles[3 * idx + 1]];
        v[2] = mesh.vertices[mesh.triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        float ma = a.magnitude;
        float mb = b.magnitude;
        float mc = c.magnitude;

        float area = 0f;

        float S = (ma + mb + mc) / 2;
        area = Mathf.Sqrt(S * (S - ma) * (S - mb) * (S - mc));

        return area;
    }

    public static Vector3 GetRandomPointOnMesh(MeshSkeleton mesh)
    {
        float totalArea = 0.0f;
        for (int i = 0; i < mesh.Triangles.Count / 3; i++)
        {
            totalArea += GetTriangleArea(mesh, i);
        }

        int triangle = GetRandomTriangleOnMesh(mesh, totalArea);
        return GetRandomPointOnTriangle(mesh, triangle);
    }

    public static Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        float totalArea = 0.0f;
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            totalArea += GetTriangleArea(mesh, i);
        }

        int triangle = GetRandomTriangleOnMesh(mesh, totalArea);
        return GetRandomPointOnTriangle(mesh, triangle);
    }

    private static int GetRandomTriangleOnMesh(MeshSkeleton mesh, float totalArea)
    {
        float rnd = Random.Range(0, totalArea);
        int nTriangles = mesh.Triangles.Count / 3;
        for (int i = 0; i < nTriangles; i++)
        {
            rnd -= GetTriangleArea(mesh, i);
            if (rnd <= 0)
                return i;
        }
        return 0;
    }

    private static int GetRandomTriangleOnMesh(Mesh mesh, float totalArea)
    {
        float rnd = Random.Range(0, totalArea);
        int nTriangles = mesh.triangles.Length / 3;
        for (int i = 0; i < nTriangles; i++)
        {
            rnd -= GetTriangleArea(mesh, i);
            if (rnd <= 0)
                return i;
        }
        return 0;
    }

    public static Vector3 GetMidpoint(Vector3 point1, Vector3 point2)
    {
        return new Vector3((point1.x + point2.x) / 2, (point1.y + point2.y) / 2, (point1.z + point2.z) / 2);
    }

    public static Vector3 GetCentroid(params Vector3[] points)
    {
        float centroidX = 0f;
        float centroidY = 0f;
        float centroidZ = 0f;

        int pointCount = points.Length;

        foreach (var point in points)
        {
            centroidX += point.x;
            centroidY += point.y;
            centroidZ += point.z;
        }

        return new Vector3(centroidX / pointCount, centroidY / pointCount, centroidZ / pointCount);
    }


    public static Bounds GetRotatedBounds(Bounds bounds, Quaternion rotation)
    {
        var aabb = new List<Vector3>();
        var extents = bounds.extents;

        aabb.Add(extents);
        aabb.Add(Vector3.Reflect(extents, Vector3.forward));
        aabb.Add(Vector3.Reflect(extents, Vector3.back));
        aabb.Add(Vector3.Reflect(extents, Vector3.up));
        aabb.Add(Vector3.Reflect(extents, Vector3.down));
        aabb.Add(Vector3.Reflect(extents, Vector3.right));
        aabb.Add(Vector3.Reflect(extents, Vector3.left));
        aabb.Add(Vector3.Reflect(extents, Vector3.one * -1));

        var newAabb = new List<Vector3>();

        foreach (var point in aabb)
        {
            newAabb.Add(rotation * point);
        }

        var maxX = 0f;
        var maxY = 0f;
        var maxZ = 0f;

        foreach(var point in newAabb)
        {
            if(point.x > maxX)
            {
                maxX = point.x;
            }

            if(point.y > maxY)
            {
                maxY = point.y;
            }

            if (point.z > maxZ)
            {
                maxZ = point.z;
            }
        }

        return new Bounds(bounds.center, new Vector3(maxX * 2, maxY * 2, maxZ * 2));
    }

    public static void SnapObjectToHitPoint(RaycastHit hit, GameObject targetObject)
    {
        // Get the closest point on the mesh to the hit point
        Mesh mesh = null;
        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (renderer is SkinnedMeshRenderer)
            {
                mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
            }
            else if (renderer is MeshRenderer)
            {
                MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    mesh = meshFilter.sharedMesh;
                }
            }
        }
        if (mesh == null)
        {
            Debug.LogError("Target object does not have a valid mesh!");
            return;
        }

        Vector3 closestVertex = mesh.vertices[0];
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vertex = targetObject.transform.TransformPoint(mesh.vertices[i]);
            float distance = Vector3.Distance(hit.point, vertex);
            if (distance < closestDistance)
            {
                closestVertex = vertex;
                closestDistance = distance;
            }
        }

        // Get the orientation and size of the target object
        Quaternion targetRotation = targetObject.transform.rotation;
        Vector3 targetScale = targetObject.transform.localScale;
        Vector3 targetSize = Vector3.Scale(targetObject.GetComponent<MeshFilter>().sharedMesh.bounds.size, targetScale);

        // Calculate the offset between the center of the target object and the closest vertex
        Vector3 targetOffset = closestVertex - targetObject.transform.position;

        // Calculate the position of the target object so that its edge is touching the edge of the hit object
        Vector3 hitNormal = hit.normal;
        Vector3 hitPoint = hit.point;
        Vector3 targetPosition = hitPoint + (hitNormal * (targetSize.magnitude / 2));

        // Apply the calculated position and orientation to the target object
        targetObject.transform.position = targetPosition + (targetRotation * targetOffset);
        targetObject.transform.rotation = targetRotation;

        // Move the target object slightly along the hit normal to ensure it is touching the surface
        targetObject.transform.position += hitNormal * 0.01f;
    }

    public static bool IsPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 pointToTest)
    {
        // Solve for d
        float d = -Vector3.Dot(planeNormal, planePoint);

        // Test if the point is on the plane
        float result = Vector3.Dot(planeNormal, pointToTest) + d;
        return Mathf.Approximately(result, 0f);
    }



    private static int GetOverlap(Mesh mesh, Transform transform, Vector3 position, Quaternion rotation)
    {
        int overlap = 0;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vertex = transform.TransformPoint(mesh.vertices[i]);
            Vector3 direction = (vertex - position).normalized;
            Vector3 rotatedVertex = rotation * mesh.vertices[i];
            Vector3 direction2 = (position + rotatedVertex - position).normalized;
            if (Vector3.Dot(direction, direction2) > 0.9f)
            {
                overlap++;
            }
        }
        return overlap;
    }
}


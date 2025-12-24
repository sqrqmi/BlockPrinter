using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class FieldLayout : MonoBehaviour
    {
        public Vector3 AxisX;
        public Vector3 AxisY;
        public Vector3 AxisZ;
        public Vector3 Pivot;

        public float TickTime;
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(FieldToWorld(Vector2Int.zero), FieldToWorld(Vector2Int.right));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(FieldToWorld(Vector2Int.zero), FieldToWorld(Vector2Int.up));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(FieldToWorld(Vector3Int.zero), FieldToWorld(Vector3Int.forward));
        }

        public Vector3 FieldToWorld(Vector2Int pos)
        {
            return AxisX * pos.x + AxisY * pos.y + Pivot;
        }

        public Vector3 FieldToWorld(Vector3Int pos)
        {
            return AxisX * pos.x + AxisY * pos.y + AxisZ * pos.z + Pivot;
        }

        public Vector2Int WorldToField(Vector3 pos)
        {
            Vector3 DistanceFromOrigin = pos - Pivot;
            return new Vector2Int(Mathf.RoundToInt(Vector3.Dot(DistanceFromOrigin, AxisX) / AxisX.sqrMagnitude),
                                    Mathf.RoundToInt(Vector3.Dot(DistanceFromOrigin, AxisY) / AxisY.sqrMagnitude));
        }
    }
}

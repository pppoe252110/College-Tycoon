using Pathfinding.RVO;
using UnityEditor;

namespace Pathfinding
{
    [CustomEditor(typeof(RVONavmesh))]
    public class RVONavmeshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}

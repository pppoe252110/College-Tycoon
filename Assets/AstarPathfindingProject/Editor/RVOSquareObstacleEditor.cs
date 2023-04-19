using Pathfinding.RVO;
using UnityEditor;

namespace Pathfinding
{
    [CustomEditor(typeof(RVOSquareObstacle))]
    [CanEditMultipleObjects]
    public class RVOSquareObstacleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}

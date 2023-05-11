
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    using Pathfinding.Util;

    /// <summary>Base class for the NavmeshCut and NavmeshAdd components</summary>
    public abstract class NavmeshClipper : VersionedMonoBehaviour
    {
        /// <summary>Called every time a NavmeshCut/NavmeshAdd component is enabled.</summary>
        static System.Action<NavmeshClipper> OnEnableCallback;

        /// <summary>Called every time a NavmeshCut/NavmeshAdd component is disabled.</summary>
        static System.Action<NavmeshClipper> OnDisableCallback;

        static readonly List<NavmeshClipper> all = new List<NavmeshClipper>();
        int listIndex = -1;

        /// <summary>
        /// Which graphs that are affected by this component.
        ///
        /// You can use this to make a graph ignore a particular navmesh cut altogether.
        ///
        /// Note that navmesh cuts can only affect navmesh/recast graphs.
        ///
        /// If you change this field during runtime you must disable the component and enable it again for the changes to be detected.
        ///
        /// See: <see cref="NavmeshBase.enableNavmeshCutting"/>
        /// </summary>
        public GraphMask graphMask = GraphMask.everything;

        public static void AddEnableCallback(System.Action<NavmeshClipper> onEnable, System.Action<NavmeshClipper> onDisable)
        {
            OnEnableCallback += onEnable;
            OnDisableCallback += onDisable;
        }

        public static void RemoveEnableCallback(System.Action<NavmeshClipper> onEnable, System.Action<NavmeshClipper> onDisable)
        {
            OnEnableCallback -= onEnable;
            OnDisableCallback -= onDisable;
        }

        /// <summary>
        /// All navmesh clipper components in the scene.
        /// Not ordered in any particular way.
        /// Warning: Do not modify this list
        /// </summary>
        public static List<NavmeshClipper> allEnabled { get { return all; } }

        protected virtual void OnEnable()
        {
            if (OnEnableCallback != null) OnEnableCallback(this);
            listIndex = all.Count;
            all.Add(this);
        }

        protected virtual void OnDisable()
        {
            // Efficient removal (the list doesn't need to be ordered).
            // Move the last item in the list to the slot occupied by this item
            // and then remove the last slot.
            all[listIndex] = all[all.Count - 1];
            all[listIndex].listIndex = listIndex;
            all.RemoveAt(all.Count - 1);
            listIndex = -1;
            if (OnDisableCallback != null) OnDisableCallback(this);
        }

        internal abstract void NotifyUpdated();
        public abstract Rect GetBounds(GraphTransform transform);
        public abstract bool RequiresUpdate();
        public abstract void ForceUpdate();
    }

    /// <summary>
    /// Navmesh cutting is used for fast recast/navmesh graph updates.
    ///
    /// Navmesh cutting is used to cut holes into an existing navmesh generated by a recast or navmesh graph.
    /// Recast/navmesh graphs usually only allow either just changing parameters on existing nodes (e.g make a whole triangle unwalkable) which is not very flexible or recalculate a whole tile which is pretty slow.
    /// With navmesh cutting you can remove (cut) parts of the navmesh that is blocked by obstacles such as a new building in an RTS game however you cannot add anything new to the navmesh or change
    /// the positions of the nodes. This is significantly faster than recalculating whole tiles from scratch in a recast graph.
    ///
    /// Video: https://www.youtube.com/watch?v=qXi5qhhGNIw
    ///
    /// [Open online documentation to see images]
    ///
    /// The NavmeshCut component uses a 2D shape to cut the navmesh with. A rectangle and a circle shape is built in, but you can also specify a custom mesh to use.
    /// [Open online documentation to see images]
    ///
    /// Note that the shape is not 3D so if you rotate the cut you will see that the 2D shape will be rotated and then just projected down on the XZ plane.
    ///
    /// In the scene view the NavmeshCut looks like an extruded 2D shape because a navmesh cut also has a height. It will only cut the part of the
    /// navmesh which it touches. For performance reasons it only checks the bounding boxes of the triangles in the navmesh, so it may cut triangles
    /// whoose bounding boxes it intersects even if the triangle does not intersect the extruded shape. However in most cases this does not make a large difference.
    ///
    /// It is also possible to set the navmesh cut to dual mode by setting the <see cref="isDual"/> field to true. This will prevent it from cutting a hole in the navmesh
    /// and it will instead just split the navmesh along the border but keep both the interior and the exterior. This can be useful if you for example
    /// want to change the penalty of some region which does not neatly line up with the navmesh triangles. It is often combined with the GraphUpdateScene component
    /// (however note that the GraphUpdateScene component will not automatically reapply the penalty if the graph is updated again).
    ///
    /// By default the navmesh cut does not take rotation or scaling into account. If you want to do that, you can set the <see cref="useRotationAndScale"/> field to true.
    /// This is a bit slower, but it is not a very large difference.
    ///
    /// Version: In 3.x navmesh cutting could only be used with recast graphs, but in 4.x they can be used with both recast and navmesh graphs.
    ///
    /// <b>Custom meshes</b>
    /// For most purposes you can use the built-in shapes, however in some cases a custom cutting mesh may be useful.
    /// The custom mesh should be a flat 2D shape like in the image below. The script will then find the contour of that mesh and use that shape as the cut.
    /// Make sure that all normals are smooth and that the mesh contains no UV information. Otherwise Unity might split a vertex and then the script will not
    /// find the correct contour. You should not use a very high polygon mesh since that will create a lot of nodes in the navmesh graph and slow
    /// down pathfinding because of that. For very high polygon meshes it might even cause more suboptimal paths to be generated if it causes many
    /// thin triangles to be added to the navmesh.
    /// [Open online documentation to see images]
    ///
    /// <b>Control updates through code</b>
    /// Navmesh cuts are applied periodically, but sometimes you may want to ensure the graph is up to date right now.
    /// Then you can use the following code.
    /// <code>
    /// // Schedule pending updates to be done as soon as the pathfinding threads
    /// // are done with what they are currently doing.
    /// AstarPath.active.navmeshUpdates.ForceUpdate();
    /// // Block until the updates have finished
    /// AstarPath.active.FlushGraphUpdates();
    /// </code>
    ///
    /// You can also control how often the scripts check for if any navmesh cut has changed.
    /// If you have a very large number of cuts it may be good for performance to not check it as often.
    /// <code>
    /// // Check every frame (the default)
    /// AstarPath.active.navmeshUpdates.updateInterval = 0;
    ///
    /// // Check every 0.1 seconds
    /// AstarPath.active.navmeshUpdates.updateInterval = 0.1f;
    ///
    /// // Never check for changes
    /// AstarPath.active.navmeshUpdates.updateInterval = -1;
    /// // You will have to schedule updates manually using
    /// AstarPath.active.navmeshUpdates.ForceUpdate();
    /// </code>
    ///
    /// You can also find this setting in the AstarPath inspector under Settings.
    /// [Open online documentation to see images]
    ///
    /// <b>Navmesh cutting and tags/penalties</b>
    /// Because navmesh cutting can modify the triangles in the navmesh pretty much abitrarily it is not possible to keep tags and penalties when updating the graph.
    /// The tags and penalties will be preserved for nodes which stay exactly the same when an update is applied though.
    ///
    /// If you need to use tags, the only stable way to keep them is to apply all the graph updates that set them every time a navmesh cut update has been done.
    /// This is of course relatively slow, but it will at least work.
    ///
    /// See: http://www.arongranberg.com/2013/08/navmesh-cutting/
    /// </summary>
    [AddComponentMenu("Pathfinding/Navmesh/Navmesh Cut")]
    [HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_navmesh_cut.php")]
    public class NavmeshCut : NavmeshClipper
    {
        public enum MeshType
        {
            Rectangle,
            Circle,
            CustomMesh
        }

        /// <summary>Shape of the cut</summary>
        [Tooltip("Shape of the cut")]
        public MeshType type;

        /// <summary>
        /// Custom mesh to use.
        /// The contour(s) of the mesh will be extracted.
        /// If you get the "max perturbations" error when cutting with this, check the normals on the mesh.
        /// They should all point in the same direction. Try flipping them if that does not help.
        ///
        /// This mesh should only be a 2D surface, not a volume.
        /// </summary>
        [Tooltip("The contour(s) of the mesh will be extracted. This mesh should only be a 2D surface, not a volume (see documentation).")]
        public Mesh mesh;

        /// <summary>Size of the rectangle</summary>
        public Vector2 rectangleSize = new Vector2(1, 1);

        /// <summary>Radius of the circle</summary>
        public float circleRadius = 1;

        /// <summary>Number of vertices on the circle</summary>
        public int circleResolution = 6;

        /// <summary>The cut will be extruded to this height</summary>
        public float height = 1;

        /// <summary>Scale of the custom mesh, if used</summary>
        [Tooltip("Scale of the custom mesh")]
        public float meshScale = 1;

        public Vector3 center;

        /// <summary>
        /// Distance between positions to require an update of the navmesh.
        /// A smaller distance gives better accuracy, but requires more updates when moving the object over time,
        /// so it is often slower.
        /// </summary>
        [Tooltip("Distance between positions to require an update of the navmesh\nA smaller distance gives better accuracy, but requires more updates when moving the object over time, so it is often slower.")]
        public float updateDistance = 0.4f;

        /// <summary>
        /// Only makes a split in the navmesh, but does not remove the geometry to make a hole.
        /// This is slower than a normal cut
        /// </summary>
        [Tooltip("Only makes a split in the navmesh, but does not remove the geometry to make a hole")]
        public bool isDual;

        /// <summary>
        /// Cuts geometry added by a NavmeshAdd component.
        /// You rarely need to change this
        /// </summary>
        public bool cutsAddedGeom = true;

        /// <summary>
        /// How many degrees rotation that is required for an update to the navmesh.
        /// Should be between 0 and 180.
        ///
        /// Note: Dynamic updating requires a Tile Handler Helper somewhere in the scene.
        /// </summary>
        [Tooltip("How many degrees rotation that is required for an update to the navmesh. Should be between 0 and 180.")]
        public float updateRotationDistance = 10;

        /// <summary>
        /// Includes rotation and scale in calculations.
        /// This is slower since a lot more matrix multiplications are needed but gives more flexibility.
        /// </summary>
        [Tooltip("Includes rotation in calculations. This is slower since a lot more matrix multiplications are needed but gives more flexibility.")]
        [UnityEngine.Serialization.FormerlySerializedAsAttribute("useRotation")]
        public bool useRotationAndScale;

        Vector3[][] contours;

        /// <summary>cached transform component</summary>
        protected Transform tr;
        Mesh lastMesh;
        Vector3 lastPosition;
        Quaternion lastRotation;

        protected override void Awake()
        {
            base.Awake();
            tr = transform;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            lastRotation = tr.rotation;
            ForceUpdate();
        }

        void Reset()
        {
            if(TryGetComponent(out MeshFilter mf))
            {
                var b = mf.sharedMesh.bounds;
                rectangleSize = new Vector2(b.size.x, b.size.z);
                height = 10;
                center = b.center;
                center.y = 0;
            }
        }

        /// <summary>Cached variable, to avoid allocations</summary>
        static readonly Dictionary<Int2, int> edges = new Dictionary<Int2, int>();
        /// <summary>Cached variable, to avoid allocations</summary>
        static readonly Dictionary<int, int> pointers = new Dictionary<int, int>();

        /// <summary>
        /// Forces this navmesh cut to update the navmesh.
        ///
        /// This update is not instant, it is done the next time it is checked if it needs updating.
        /// See: <see cref="Pathfinding.NavmeshUpdates.updateInterval"/>
        /// See: <see cref="Pathfinding.NavmeshUpdates.ForceUpdate()"/>
        /// </summary>
        public override void ForceUpdate()
        {
            lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        /// <summary>
        /// Returns true if this object has moved so much that it requires an update.
        /// When an update to the navmesh has been done, call NotifyUpdated to be able to get
        /// relavant output from this method again.
        /// </summary>
        public override bool RequiresUpdate()
        {
            return (tr.position - lastPosition).sqrMagnitude > updateDistance * updateDistance || (useRotationAndScale && (Quaternion.Angle(lastRotation, tr.rotation) > updateRotationDistance));
        }

        /// <summary>
        /// Called whenever this navmesh cut is used to update the navmesh.
        /// Called once for each tile the navmesh cut is in.
        /// You can override this method to execute custom actions whenever this happens.
        /// </summary>
        public virtual void UsedForCut()
        {
        }

        /// <summary>Internal method to notify the NavmeshCut that it has just been used to update the navmesh</summary>
        internal override void NotifyUpdated()
        {
            lastPosition = tr.position;

            if (useRotationAndScale)
            {
                lastRotation = tr.rotation;
            }
        }

        void CalculateMeshContour()
        {
            if (mesh == null) return;

            edges.Clear();
            pointers.Clear();

            Vector3[] verts = mesh.vertices;
            int[] tris = mesh.triangles;
            for (int i = 0; i < tris.Length; i += 3)
            {
                // Make sure it is clockwise
                if (VectorMath.IsClockwiseXZ(verts[tris[i + 0]], verts[tris[i + 1]], verts[tris[i + 2]]))
                {
                    int tmp = tris[i + 0];
                    tris[i + 0] = tris[i + 2];
                    tris[i + 2] = tmp;
                }

                edges[new Int2(tris[i + 0], tris[i + 1])] = i;
                edges[new Int2(tris[i + 1], tris[i + 2])] = i;
                edges[new Int2(tris[i + 2], tris[i + 0])] = i;
            }

            // Construct a list of pointers along all edges
            for (int i = 0; i < tris.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!edges.ContainsKey(new Int2(tris[i + ((j + 1) % 3)], tris[i + ((j + 0) % 3)])))
                    {
                        pointers[tris[i + ((j + 0) % 3)]] = tris[i + ((j + 1) % 3)];
                    }
                }
            }

            var contourBuffer = new List<Vector3[]>();

            List<Vector3> buffer = Pathfinding.Util.ListPool<Vector3>.Claim();

            // Follow edge pointers to generate the contours
            for (int i = 0; i < verts.Length; i++)
            {
                if (pointers.ContainsKey(i))
                {
                    buffer.Clear();

                    int s = i;
                    do
                    {
                        int tmp = pointers[s];

                        //This path has been taken before
                        if (tmp == -1) break;

                        pointers[s] = -1;
                        buffer.Add(verts[s]);
                        s = tmp;

                        if (s == -1)
                        {
                            Debug.LogError("Invalid Mesh '" + mesh.name + " in " + gameObject.name);
                            break;
                        }
                    } while (s != i);

                    if (buffer.Count > 0) contourBuffer.Add(buffer.ToArray());
                }
            }

            // Return lists to the pool
            Pathfinding.Util.ListPool<Vector3>.Release(ref buffer);

            contours = contourBuffer.ToArray();
        }

        /// <summary>
        /// Bounds in XZ space after transforming using the *inverse* transform of the inverseTransform parameter.
        /// The transformation will typically transform the vertices to graph space and this is used to
        /// figure out which tiles the cut intersects.
        /// </summary>
        public override Rect GetBounds(GraphTransform inverseTransform)
        {
            var buffers = Pathfinding.Util.ListPool<List<Vector3>>.Claim();

            GetContour(buffers);

            Rect r = new Rect();
            for (int i = 0; i < buffers.Count; i++)
            {
                var buffer = buffers[i];
                for (int k = 0; k < buffer.Count; k++)
                {
                    var p = inverseTransform.InverseTransform(buffer[k]);
                    if (k == 0)
                    {
                        r = new Rect(p.x, p.z, 0, 0);
                    }
                    else
                    {
                        r.xMax = System.Math.Max(r.xMax, p.x);
                        r.yMax = System.Math.Max(r.yMax, p.z);
                        r.xMin = System.Math.Min(r.xMin, p.x);
                        r.yMin = System.Math.Min(r.yMin, p.z);
                    }
                }
            }

            Pathfinding.Util.ListPool<List<Vector3>>.Release(ref buffers);
            return r;
        }

        /// <summary>
        /// World space contour of the navmesh cut.
        /// Fills the specified buffer with all contours.
        /// The cut may contain several contours which is why the buffer is a list of lists.
        /// </summary>
        public void GetContour(List<List<Vector3>> buffer)
        {
            if (circleResolution < 3) circleResolution = 3;

            bool reverse;
            switch (type)
            {
                case MeshType.Rectangle:
                    List<Vector3> buffer0 = Pathfinding.Util.ListPool<Vector3>.Claim();

                    buffer0.Add(new Vector3(-rectangleSize.x, 0, -rectangleSize.y) * 0.5f);
                    buffer0.Add(new Vector3(rectangleSize.x, 0, -rectangleSize.y) * 0.5f);
                    buffer0.Add(new Vector3(rectangleSize.x, 0, rectangleSize.y) * 0.5f);
                    buffer0.Add(new Vector3(-rectangleSize.x, 0, rectangleSize.y) * 0.5f);

                    reverse = (rectangleSize.x < 0) ^ (rectangleSize.y < 0);
                    TransformBuffer(buffer0, reverse);
                    buffer.Add(buffer0);
                    break;
                case MeshType.Circle:
                    buffer0 = Pathfinding.Util.ListPool<Vector3>.Claim(circleResolution);

                    for (int i = 0; i < circleResolution; i++)
                    {
                        buffer0.Add(new Vector3(Mathf.Cos((i * 2 * Mathf.PI) / circleResolution), 0, Mathf.Sin((i * 2 * Mathf.PI) / circleResolution)) * circleRadius);
                    }

                    reverse = circleRadius < 0;
                    TransformBuffer(buffer0, reverse);
                    buffer.Add(buffer0);
                    break;
                case MeshType.CustomMesh:
                    if (mesh != lastMesh || contours == null)
                    {
                        CalculateMeshContour();
                        lastMesh = mesh;
                    }

                    if (contours != null)
                    {
                        reverse = meshScale < 0;

                        for (int i = 0; i < contours.Length; i++)
                        {
                            Vector3[] contour = contours[i];

                            buffer0 = Pathfinding.Util.ListPool<Vector3>.Claim(contour.Length);
                            for (int x = 0; x < contour.Length; x++)
                            {
                                buffer0.Add(contour[x] * meshScale);
                            }

                            TransformBuffer(buffer0, reverse);
                            buffer.Add(buffer0);
                        }
                    }
                    break;
            }
        }

        void TransformBuffer(List<Vector3> buffer, bool reverse)
        {
            var offset = center;

            // Take rotation and scaling into account
            if (useRotationAndScale)
            {
                var local2world = tr.localToWorldMatrix;
                for (int i = 0; i < buffer.Count; i++) buffer[i] = local2world.MultiplyPoint3x4(buffer[i] + offset);
                reverse ^= VectorMath.ReversesFaceOrientationsXZ(local2world);
            }
            else
            {
                offset += tr.position;
                for (int i = 0; i < buffer.Count; i++) buffer[i] += offset;
            }

            if (reverse) buffer.Reverse();
        }

        public static readonly Color GizmoColor = new Color(37.0f / 255, 184.0f / 255, 239.0f / 255);

        public void OnDrawGizmos()
        {
            if (tr == null) tr = transform;

            var buffer = Pathfinding.Util.ListPool<List<Vector3>>.Claim();
            GetContour(buffer);
            Gizmos.color = GizmoColor;

            // Draw all contours
            for (int i = 0; i < buffer.Count; i++)
            {
                var cont = buffer[i];
                for (int j = 0; j < cont.Count; j++)
                {
                    Vector3 p1 = cont[j];
                    Vector3 p2 = cont[(j + 1) % cont.Count];
                    Gizmos.DrawLine(p1, p2);
                }
            }

            Pathfinding.Util.ListPool<List<Vector3>>.Release(ref buffer);
        }

        /// <summary>Y coordinate of the center of the bounding box in graph space</summary>
        internal float GetY(Pathfinding.Util.GraphTransform transform)
        {
            return transform.InverseTransform(useRotationAndScale ? tr.TransformPoint(center) : tr.position + center).y;
        }

        public void OnDrawGizmosSelected()
        {
            var buffer = Pathfinding.Util.ListPool<List<Vector3>>.Claim();

            GetContour(buffer);
            var col = Color.Lerp(GizmoColor, Color.white, 0.5f);
            col.a *= 0.5f;
            Gizmos.color = col;

            var graph = AstarPath.active != null ? (AstarPath.active.data.recastGraph as NavmeshBase ?? AstarPath.active.data.navmesh) : null;
            var transform = graph != null ? graph.transform : Pathfinding.Util.GraphTransform.identityTransform;
            float ymid = GetY(transform);
            float ymin = ymid - height * 0.5f;
            float ymax = ymid + height * 0.5f;

            // Draw all contours
            for (int i = 0; i < buffer.Count; i++)
            {
                var cont = buffer[i];
                for (int j = 0; j < cont.Count; j++)
                {
                    Vector3 p1 = transform.InverseTransform(cont[j]);
                    Vector3 p2 = transform.InverseTransform(cont[(j + 1) % cont.Count]);

                    Vector3 p1low = p1, p2low = p2, p1high = p1, p2high = p2;
                    p1low.y = p2low.y = ymin;
                    p1high.y = p2high.y = ymax;

                    Gizmos.DrawLine(transform.Transform(p1low), transform.Transform(p2low));
                    Gizmos.DrawLine(transform.Transform(p1high), transform.Transform(p2high));
                    Gizmos.DrawLine(transform.Transform(p1low), transform.Transform(p1high));
                }
            }

            Pathfinding.Util.ListPool<List<Vector3>>.Release(ref buffer);
        }
    }
}

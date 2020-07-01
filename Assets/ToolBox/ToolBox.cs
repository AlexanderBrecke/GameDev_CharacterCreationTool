using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToolBox
{


    namespace Tools
    {
        // --- Field of view ---
        #region FieldOfView
        public class FieldOfView
        {
            // --- INITIALIZING THE FIELD OF VIEW ---


                // --- Variables ---

                    float viewRadius;
                    float viewAngle;
                    float meshResolution;

                    float delayTime;
                    float edgeDistanceThreshold;
                    int edgeResolveIterations;

                    //Need a transform to know who is viewing
                    Transform ownersTransform;

                    //Need Layermasks to know what are obstacles and what are targets
                    //Obstacles are the first one, targets are the second one.
                    LayerMask[] obstacleAndTargetMask;

                    //Needed to draw the field of view as a mesh in gameview
                    MeshFilter viewMeshFilter;
                    Mesh viewMesh;

                    //The list of all the visible targets, this will also be sorted by distance, shortest first, longest last.
                    public List<Transform> visibleTargets = new List<Transform>();
                
                // ---


                // --- Creating a constructor for the field of view ---

                    //We give vector3 for the float and int variables because this takes less space and is easier than passing all seperately
                    //This could prove to be a bit diffuse, but I hope the documentation will make up for it.
                    private FieldOfView(Vector3 viewRadAngleRes, Transform transform, MeshFilter filter, LayerMask[] obstacleAndTargetMasks, Vector3 delayDistanceTrhesholdAndEdgeIterations)
                    {
                        this.viewRadius = viewRadAngleRes.x;
                        this.viewAngle = viewRadAngleRes.y;
                        this.meshResolution = viewRadAngleRes.z;
                        this.ownersTransform = transform;
                        this.viewMeshFilter = filter;
                        this.obstacleAndTargetMask = obstacleAndTargetMasks;
                        this.delayTime = delayDistanceTrhesholdAndEdgeIterations.x;
                        this.edgeDistanceThreshold = delayDistanceTrhesholdAndEdgeIterations.y;
                        this.edgeResolveIterations = Mathf.RoundToInt(delayDistanceTrhesholdAndEdgeIterations.z);

                        viewMesh = new Mesh();
                        viewMesh.name = "View Mesh";
                        viewMeshFilter.mesh = viewMesh;
                    }

                    //Here are some extra ways to construct the field of view so we don't have to pass all the things every time.
                    //These are the only ones able to be called.
                    //The edgeDistanceThreshold and edgeIterations should not be set other than here.
                    public FieldOfView(Transform transform, MeshFilter filter, LayerMask[] obstacleAndTargetMasks) : this(new Vector3(10, 75, 5), transform, filter, obstacleAndTargetMasks, new Vector3(0.3f, 0.75f, 5)) { }
                    public FieldOfView(Transform transform, MeshFilter filter, LayerMask[] obstacleAndTargetMasks, float delay) : this(new Vector3(10, 75, 5), transform, filter, obstacleAndTargetMasks, new Vector3(delay, 0.75f, 5)) { }
                    public FieldOfView(Vector3 viewRadAngleRes, Transform transform, MeshFilter filter, LayerMask[] obstacleAndTargetMasks) : this(viewRadAngleRes, transform, filter, obstacleAndTargetMasks, new Vector3(0.3f, 0.75f, 5)) { }
                    public FieldOfView(Vector3 viewRadAngleRes, Transform transform, MeshFilter filter, LayerMask[] obstacleAndTargetMasks, float delay) : this(viewRadAngleRes, transform, filter, obstacleAndTargetMasks, new Vector3(delay, 0.75f, 5)) { }

            // ---


            // --- ---

            // Needed functions for the FieldOfView
            #region FOWFunctions
            // --- FUNCTION TIME ---


            // --- Finding visible targets and adding them to the visible targets list ---

            void FindVisibleTargets()
                    {
                        visibleTargets.Clear();
                        Collider[] targetsInViewRadius = Physics.OverlapSphere(ownersTransform.position, viewRadius, obstacleAndTargetMask[1]);

                        for (int i = 0; i < targetsInViewRadius.Length; i++)
                        {
                            Transform target = targetsInViewRadius[i].transform;
                            Vector3 dirToTarget = (target.position - ownersTransform.position).normalized;
                            if (Vector3.Angle(ownersTransform.forward, dirToTarget) < viewAngle / 2)
                            {
                                float distanceToTarget = Vector3.Distance(ownersTransform.position, target.position);

                                if (!Physics.Raycast(ownersTransform.position, dirToTarget, distanceToTarget, obstacleAndTargetMask[0]) && target.tag != "Noise")
                                {
                                    visibleTargets.Add(target);
                                }
                            }
                        }
                        visibleTargets.Sort(SortVisibleTargetsByDistance);
                    }

                // ---

                // --- Do it after a delay ---

                    public IEnumerator FindTargetWithDelay()
                    {
                        while (true)
                        {
                            yield return new WaitForSeconds(delayTime);
                            FindVisibleTargets();
                        }
                    }

                // ---

                // --- Sort targets by distance ---

                    public int SortVisibleTargetsByDistance(Transform targetA, Transform targetB)
                    {
                        if (Vector3.Distance(ownersTransform.position, targetA.position) < Vector3.Distance(ownersTransform.position, targetB.position))
                            return -1;
                        else if (Vector3.Distance(ownersTransform.position, targetA.position) > Vector3.Distance(ownersTransform.position, targetB.position))
                            return 1;

                        return 0;
                    }

                // ---

                // --- Update the info of the Field of View, and draw it ---

                    // *** This NEEDS to be called in the LATEUPDATE! ***
                    public void UpdateFieldOfView(Vector3 viewRadAngleRes)
                    {
                        if(viewRadAngleRes != Vector3.zero)
                        {
                            viewRadius = viewRadAngleRes.x;
                            viewAngle = viewRadAngleRes.y;
                            meshResolution = viewRadAngleRes.z;
                        }
                        DrawFieldOfView();
                    }
                    // *** ***

                // ---

                // --- Drawing the actual Field Of View ---

                    public void DrawFieldOfView()
                    {
                        //Find how many rays we need to shoot, this depends on the view angle and how much resolution we have.
                        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
                        float rayAngleSize = viewAngle / rayCount;

                        //List of points hit by our raycast, if no point was hit it's at the edge of viewRadius
                        List<Vector3> viewPoints = new List<Vector3>();
                        
                        //Stored to gain access within the loop.
                        ViewCastInfo oldViewCast = new ViewCastInfo();

                        //Loop through the amount of rays we should cast
                        for (int i = 0; i < rayCount; i++)
                        {
                            //Give the angle of the ray
                            float angle = ownersTransform.eulerAngles.y - viewAngle / 2 + rayAngleSize * i;
                            //Cast the ray
                            ViewCastInfo newViewCast = ViewCast(angle);
                        
                            //Check for edges
                            if (i > 0)
                            {
                                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                                {
                                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                                    if (edge.pointA != Vector3.zero)
                                    {
                                        viewPoints.Add(edge.pointA);
                                    }
                                    if (edge.pointB != Vector3.zero)
                                    {
                                        viewPoints.Add(edge.pointB);
                                    }
                                }
                            }

                            //Add the point to our list of points
                            viewPoints.Add(newViewCast.point);
                            
                            //Set old cast to this one before going to the next iteration in the loop
                            oldViewCast = newViewCast;
                        }
                        //Now we have the information for drawing a mesh of the Field of View


                        //Setting up the information for our mesh
                        //The mesh need vertecies and triangles
                        int vertexCount = viewPoints.Count;
                        Vector3[] vertecies = new Vector3[vertexCount];
                        int[] triangles = new int[(vertexCount - 2) * 3];
                        
                        //Set the first one to the center of the local space (the game objects center)
                        vertecies[0] = Vector3.zero;

                        //Loop through the vertecies and set their values
                        for (int i = 1; i < vertexCount; i++)
                        {   
                            
                            vertecies[i] = ownersTransform.InverseTransformPoint(viewPoints[i]);

                            if (i < vertexCount - 2)
                            {
                                triangles[i * 3] = 0;
                                triangles[i * 3 + 1] = i + 1;
                                triangles[i * 3 + 2] = i + 2;
                            }
                        }

                        //Clear so we can set the new mesh
                        viewMesh.Clear();
                        
                        //set the new mesh
                        viewMesh.vertices = vertecies;
                        viewMesh.triangles = triangles;
                        viewMesh.RecalculateNormals();
                    }

                // ---

                // --- Find the info of the casted view ---

                    ViewCastInfo ViewCast(float globalAngle)
                    {
                        Vector3 direction = Misc.MiscFunctions.DirFromAngle(globalAngle, true, ownersTransform);
                        RaycastHit hit;

                        if (Physics.Raycast(ownersTransform.position, direction, out hit, viewRadius, obstacleAndTargetMask[0]))
                        {
                            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
                        }
                        else
                        {
                            return new ViewCastInfo(false, ownersTransform.position + direction * viewRadius, viewRadius, globalAngle);
                        }
                    }

                // ---

                // --- Find edge info ---

                    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
                    {
                        float minAngle = minViewCast.angle;
                        float maxAngle = maxViewCast.angle;
                        Vector3 minPoint = Vector3.zero;
                        Vector3 maxPoint = Vector3.zero;

                        for (int i = 0; i < edgeResolveIterations; i++)
                        {
                            float angle = (minAngle + maxAngle) / 2;
                            ViewCastInfo newViewCast = ViewCast(angle);

                            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
                            {
                                minAngle = angle;
                                minPoint = newViewCast.point;
                            }
                            else
                            {
                                maxAngle = angle;
                                maxPoint = newViewCast.point;
                            }
                        }
                        return new EdgeInfo(minPoint, maxPoint);
                    }

            // ---


            // --- ---
            #endregion 

            // Needed structs for the FieldOfView
            #region FOWStructs
            // --- STRUCTS ---

            public struct ViewCastInfo
                {
                    public bool hit;
                    public Vector3 point;
                    public float distance;
                    public float angle;

                    public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
                    {
                        hit = _hit;
                        point = _point;
                        distance = _distance;
                        angle = _angle;
                    }
                }

                public struct EdgeInfo
                {
                    public Vector3 pointA;
                    public Vector3 pointB;

                    public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
                    {
                        pointA = _pointA;
                        pointB = _pointB;
                    }
                }


            // --- ---
            #endregion 
        }
        #endregion
        // --- ---




        // --- GRID ---

        public class CustomGrid<TGridObject>
        {

            public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
            public class OnGridValueChangedEventArgs : EventArgs
            {
                public int x;
                public int z;
            }

            private int width;
            private int height;
            private float cellSize;
            private Vector3 originPosition;
            private bool alignOnZ;

            private TGridObject[,] gridArray;

            public CustomGrid(int width, int height, float cellSize, Vector3 originPosition, Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject, bool alignOnZ)
            {
                this.width = width;
                this.height = height;
                this.cellSize = cellSize;
                if(alignOnZ)
                    this.originPosition = new Vector3(originPosition.x - ((width * 0.5f) * cellSize), originPosition.y, originPosition.z - ((height * 0.5f) * cellSize));
                else
                    this.originPosition = new Vector3(originPosition.x - ((width* 0.5f) * cellSize), originPosition.y - ((height*0.5f)*cellSize), originPosition.z);
                this.alignOnZ = alignOnZ;

                gridArray = new TGridObject[width, height];

                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int z = 0; z < gridArray.GetLength(1); z++)
                    {
                        gridArray[x, z] = createGridObject(this, x, z);
                    }
                }

                bool showDebug = true;
                if (showDebug && alignOnZ)
                {
                    TextMesh[,] debugTextArray = new TextMesh[width, height];

                    for (int x = 0; x < gridArray.GetLength(0); x++)
                    {
                        for (int z = 0; z < gridArray.GetLength(1); z++)
                        {
                            debugTextArray[x,z] =  Misc.MiscFunctions.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f, 12, Color.white, TextAnchor.MiddleCenter);
                            Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100);
                            Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100);
                        }
                    }
                    Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100);
                    Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100);

                    OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
                    {
                        debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
                    };
                }

                //SetValue(0, 1, 56);
            }

            public Vector3 GetWorldPosition(int x, int z)
            {
                if (alignOnZ)
                    return new Vector3(x, 0, z) * cellSize + originPosition;
                else
                    return new Vector3(x, z) * cellSize + originPosition;
            }

            public void GetXZ(Vector3 worldPosition, out int x, out int z)
            {
                if (alignOnZ)
                {
                    x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
                    z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
                } else
                {
                    x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
                    z = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
                }
                //Debug.Log(x + ", " + z);

            }

            public void SetGridObject(int x, int z, TGridObject value)
            {
                if(x >= 0 && z >= 0 && x < width && z < height)
                {
                    gridArray[x, z] = value;
                    if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
                }
            }

            public void SetGridObject(Vector3 worldPosition, TGridObject value)
            {
                int x, z;
                
                GetXZ(worldPosition, out x, out z);
                SetGridObject(x, z, value);
            }

            public void TriggerGridObjectChanged(int x, int z)
            {

                if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
            }

            public TGridObject GetGridObject(int x, int z)
            {
                if (x >= 0 && z >= 0 && x < width && z < height)
                {
                    return gridArray[x, z];
                }
                else
                    return default(TGridObject);
            }

            public TGridObject GetGridObject(Vector3 worldPosition)
            {
                int x, z;
                GetXZ(worldPosition, out x, out z);
                return GetGridObject(x, z);
            }

            public int GetWidth()
            {
                return gridArray.GetLength(0);
            }
            public int GetHeight()
            {
                return gridArray.GetLength(1);
            }

            public Vector3 GetOriginPosition()
            {
                return originPosition;
            }


        }

        // --- ---

        // --- PATHFINDING ---

        public class PathNode
        {
            private CustomGrid<PathNode> grid;
            public int x, z;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;
            public PathNode previousNode;
            public PathNode(CustomGrid<PathNode> grid, int x, int z)
            {
                this.grid = grid;
                this.x = x;
                this.z = z;
                isWalkable = true;
            }

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }

            public void SetIsWalkable(bool isWalkable)
            {
                this.isWalkable = isWalkable;
                grid.TriggerGridObjectChanged(x,z);
            }

            public override string ToString()
            {
                return x + ", " + z;
            }

        }

        public class Pathfinding
        {

            private const int MOVE_STRAIGHT_COST = 10;
            private const int MOVE_DIAGONAL_COST = 14;

            public static Pathfinding Instance { get; private set; }

            private CustomGrid<PathNode> grid;
            private List<PathNode> openList;
            private List<PathNode> closedList;

            public Pathfinding(int width, int height, float cellSize, Vector3 position)
            {
                Instance = this;
                grid = new CustomGrid<PathNode>(width, height, cellSize, position, (CustomGrid<PathNode> g, int x, int z) => new PathNode(g, x, z), true);
            }

            public CustomGrid<PathNode> GetGrid()
            {
                return grid;
            }

            public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
            {
                grid.GetXZ(startWorldPosition, out int startX, out int startY);
                grid.GetXZ(endWorldPosition, out int endX, out int endY);

                List<PathNode> path = FindPath(startX, startY, endX, endY);

                if(path == null)
                {
                    return null;
                } else
                {
                    List<Vector3> vectorPath = new List<Vector3>();
                    foreach(PathNode node in path)
                    {
                        vectorPath.Add(grid.GetWorldPosition(node.x, node.z));
                    }
                    return vectorPath;
                }
            }

            public List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
            {
                PathNode startNode = grid.GetGridObject(startX, startZ);
                PathNode endNode = grid.GetGridObject(endX, endZ);

                openList = new List<PathNode> { startNode };
                closedList = new List<PathNode>();

                for (int x = 0; x < grid.GetWidth(); x++)
                {
                    for (int z = 0; z < grid.GetHeight(); z++)
                    {
                        PathNode pathNode = grid.GetGridObject(x, z);
                        pathNode.gCost = int.MaxValue;
                        pathNode.CalculateFCost();
                        pathNode.previousNode = null;
                    }
                }

                startNode.gCost = 0;
                startNode.hCost = CalculateDistance(startNode, endNode);
                startNode.CalculateFCost();

                while (openList.Count > 0)
                {
                    PathNode currentNode = GetLowestFCostNode(openList);
                    if(currentNode == endNode)
                    {
                        //Reached final node
                        return CalculatePath(endNode);
                    }

                    openList.Remove(currentNode);
                    closedList.Add(currentNode);

                    foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                    {
                        if (closedList.Contains(neighbourNode)) continue;
                        if (!neighbourNode.isWalkable)
                        {
                            closedList.Add(neighbourNode);
                            continue;
                        }

                        int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.previousNode = currentNode;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                            neighbourNode.CalculateFCost();

                            if (!openList.Contains(neighbourNode))
                            {
                                openList.Add(neighbourNode);
                            }
                        }

                    }
                }

                //Out of Nodes on the openList
                Debug.LogError("No path found");
                return null;

            }

            private List<PathNode> GetNeighbourList(PathNode currentNode)
            {
                List<PathNode> neighbourList = new List<PathNode>();

                if(currentNode.x -1 >= 0)
                {
                    //Left
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z));
                    //Left Down
                    if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
                    //Left Up
                    if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
                }
                if(currentNode.x + 1 < grid.GetWidth())
                {
                    //Right
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z));
                    //Right Down
                    if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
                    //Right Up
                    if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));
                }
                //Down
                if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.z - 1));
                //Up
                if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.z + 1));

                return neighbourList;
            }

            public PathNode GetNode(int x, int z)
            {
                return grid.GetGridObject(x, z);
            }

            private List<PathNode> CalculatePath(PathNode endNode)
            {
                List<PathNode> path = new List<PathNode>();
                path.Add(endNode);
                PathNode currentNode = endNode;

                while (currentNode.previousNode != null)
                {
                    path.Add(currentNode.previousNode);
                    currentNode = currentNode.previousNode;
                }
                path.Reverse();
                return path;
            }

            private int CalculateDistance(PathNode a, PathNode b)
            {
                int xDistance = Mathf.Abs(a.x - b.x);
                int yDistance = Mathf.Abs(a.z - b.z);
                int remaining = Mathf.Abs(xDistance - yDistance);
                return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
            }

            private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
            {
                PathNode lowestFCostNode = pathNodeList[0];
                for (int i = 0; i < pathNodeList.Count; i++)
                {
                    if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                        lowestFCostNode = pathNodeList[i];
                }
                return lowestFCostNode;
            }

        }


        // --- ---


    }

    namespace Misc
    {

        public class MiscFunctions
        {
            // --- Directional things ---

            public static Vector3 GetRandomDirection(bool shouldUseY)
            {
                if (!shouldUseY)
                    return new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1)).normalized;
                else
                    return new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;

            }

            public static Vector3 GetVectorFromAngle(float angle)
            {
                float angleRad = angle * (Mathf.PI / 180f);
                return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
            }

            public static Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, Transform transform)
            {
                if (!angleIsGlobal)
                {
                    angleInDegrees += transform.eulerAngles.y;
                }
                return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
            }

            public static Vector3 FindMousePositionIn3DSpace()
            {
                Vector3 worldPosition;
                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float hitdist;

                if (plane.Raycast(ray, out hitdist))
                {
                    worldPosition = ray.GetPoint(hitdist);
                    return worldPosition;
                }
                return Vector3.zero;
            }


            public static Vector3 GetMouseWorldPosition()
            {
                Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
                vec.y = 0;
                return vec;
            }
            public static Vector3 GetMouseWorldPositionWithZ()
            {
                return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            }
            public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
            {
                return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
            }
            public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
            {
                Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
                return worldPosition;
            }


            // --- ---

            public static void LookAtPosition(Transform transform, Vector3 lookingPosition, float rotationSpeed = 7.5f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookingPosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            public static bool IsLookingAtPosition(Transform transform, Vector3 lookingPosition, float rotationSpeed = 7.5f)
            {
                bool lookingAt = false;
                Quaternion targetRotation = Quaternion.LookRotation(lookingPosition - transform.position);
                Vector3 targetRotationVector = targetRotation.eulerAngles;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                Vector3 rotationVector = transform.rotation.eulerAngles;
                if (transform.rotation == targetRotation)
                    lookingAt = true;
                if (rotationVector.y >= targetRotationVector.y - 3.5f && rotationVector.y <= targetRotationVector.y + 3.5f)
                    lookingAt = true;
                if (Mathf.RoundToInt(rotationVector.y) == Mathf.RoundToInt(targetRotationVector.y))
                    lookingAt = true;

                //Debug.Log("rotation = " + rotationVector + ", target rotation = " + targetRotationVector);
                return lookingAt;
            }

            public static bool IsLookingAt(Transform transform, Vector3 toLookAt, float angles = 15)
            {
                bool lookingAt = false;

                Quaternion targetRotation = Quaternion.LookRotation(toLookAt - transform.position);
                Vector3 targetRotationVector = targetRotation.eulerAngles;
                Vector3 rotationVector = transform.rotation.eulerAngles;

                if (rotationVector.y >= targetRotationVector.y - (angles * 0.5f) && rotationVector.y <= targetRotationVector.y + (angles * 0.5f))
                    lookingAt = true;

                return lookingAt;
            }

            public static void LookAtMouse(Transform transform, float rotationSpeed = 7.5f)
            {
                LookAtPosition(transform, FindMousePositionIn3DSpace(), rotationSpeed);
            }

            public static void LookAtTarget(Transform ownTransform, Transform targetTransform, float rotationSpeed = 7.5f)
            {
                LookAtPosition(ownTransform, targetTransform.position, rotationSpeed);
            }

            public static bool HasReachedDestination(Vector3 ownPosition, Vector3 destinationToReach, float reachRadius)
            {
                bool reached = false;
                if (Vector3.Distance(ownPosition, destinationToReach) <= reachRadius)
                    reached = true;
                return reached;
            }


            protected static IEnumerator DoFunctionAfterWait(float waitTime, Action callback)
            {
                yield return new WaitForSeconds(waitTime);
                callback();
                yield break;
            }
            public static void DelayedFunction(GameObject monoBehaviourObject, float waitTime, Action callback)
            {
                
                MonoBehaviour mono = monoBehaviourObject.GetComponent<MonoBehaviour>();
                mono.StartCoroutine(DoFunctionAfterWait(waitTime, callback));
            }




            public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder, bool alignOnZ)
            {
                GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
                if(alignOnZ)
                    gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                Transform transform = gameObject.transform;
                transform.SetParent(parent, false);
                transform.localPosition = localPosition;
                TextMesh textMesh = gameObject.GetComponent<TextMesh>();
                textMesh.anchor = textAnchor;
                textMesh.alignment = textAlignment;
                textMesh.text = text;
                textMesh.fontSize = fontSize;
                textMesh.color = color;
                textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
                return textMesh;
            }

            public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000, bool alignOnZ = true)
            {
                if (color == null) color = Color.white;
                return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder, alignOnZ);
            }

            public static void SetUnwalkablePath(List<GameObject> obstacles, Tools.Pathfinding instance)
            {
                foreach (GameObject obj in obstacles)
                {
                    Bounds bounds = obj.GetComponent<Renderer>().bounds;
                    for (int x1 = Mathf.RoundToInt(bounds.min.x); x1 < Mathf.RoundToInt(bounds.max.x); x1++)
                    {
                        for (int z1 = Mathf.RoundToInt(bounds.min.z); z1 < Mathf.RoundToInt(bounds.max.z); z1++)
                        {
                            instance.GetGrid().GetXZ(new Vector3(x1, 0, z1), out int x, out int z);
                            instance.GetNode(x, z).SetIsWalkable(!instance.GetNode(x, z).isWalkable);
                        }
                    }
                    
                }
            }


            public static bool MouseIsHoveringUI()
            {

                //bool isHovering = EventSystem.current.IsPointerOverGameObject();
                bool isHovering = false;
                var eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject.tag == "IgnoreUIClick")
                    {
                        results.RemoveAt(i);
                        i--;
                    }

                }
                if (results.Count > 0)
                    isHovering = true;

                return isHovering;
            }

            public static GameObject HoveredUIElement()
            {
                GameObject hoveredElement = null;

                var eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                if(results.Count > 0)
                    hoveredElement = results[0].gameObject;
                if (hoveredElement != null)
                    Debug.Log(hoveredElement.name);

                //if (EventSystem.current.IsPointerOverGameObject())
                //{
                //    //Debug.Log("FOO");
                //}

                //var eventData = new PointerEventData(EventSystem.current);
                //eventData.position = Input.mousePosition;
                //var results = new List<RaycastResult>();
                ////Debug.Log(EventSystem.current.currentSelectedGameObject);//RaycastAll(eventData, results);
                //if (EventSystem.current.IsPointerOverGameObject())
                //{
                //    if(EventSystem.current.currentSelectedGameObject == null)
                //    {

                //    }
                //    GameObject obj = EventSystem.current.currentSelectedGameObject;
                    
                //    Debug.Log(obj.name);


                //}
                ////Debug.Log(results.Count);
                //foreach (var item in results)
                //{
                //    //Debug.Log(item.gameObject.name);
                //}


                ////Debug.Log(eventData.pointerCurrentRaycast);
                //if (eventData.pointerCurrentRaycast.gameObject != null)
                //{
                //    hoveredElement = eventData.pointerCurrentRaycast.gameObject;
                //}

                //if(hoveredElement != null)
                //    Debug.Log(hoveredElement.name);
                return hoveredElement;
            }




        }



    }


}
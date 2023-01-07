using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Map
{
    public class MapView : MonoBehaviour
    {
        public enum MapOrientation
        {
            BottomToTop,
            TopToBottom,
            LeftToRight,
            RightToLeft
        }

        public MapManager mapManager;
        public MapOrientation mapOrientation;

        // 맵을 구성하는데 사용할 수 있는 MapConfig 스크립트 목록
        public List<MapConfig> allMapConfigs;
        public GameObject nodePrefab;
        public float orientationOffset;                 //화면 가장자리에서 지도의 시작/끝 노드 offset

        [Header("Background Set")]
        public Sprite background;     // null이면 배경이 안보임
        public Color backgroundColor = Color.white;
        public float xSize;
        public float yOffset;

        [Header("Line Set")]
        public GameObject linePrefab;
        [Range(3, 10)] public int linePointsCount = 10;
        public float offsetFromNodes = 0.5f;              // 노드에서 선 시작점 까지의 거리

        [Header("Colors")]
        public Color visitedColor = Color.white;      //노드 방문 or 갈 수 있는곳 색상
        public Color lockedColor = Color.gray;        //잠긴 노드 색상
        public Color lineVisitedColor = Color.white;  //방문 or 사용 가능 색상
        public Color lineLockedColor = Color.gray;    //잠긴 길 색상

        private GameObject firstParent;
        private GameObject mapParent;
        private List<List<Point>> paths;
        private Camera cam;

        //모든 노드 :
        public readonly List<MapNode> mapNodes = new List<MapNode>();
        private readonly List<LineConnection> lineConnections = new List<LineConnection>();

        public static MapView Instance;

        private void Awake()
        {
            Instance = this;
            cam = Camera.main;
        }

        private void ClearMap()
        {
            if (firstParent != null)
                Destroy(firstParent);

            mapNodes.Clear();
            lineConnections.Clear();
        }
        public void ShowMap(Map map)
        {
            if (map == null)
            {
                Debug.LogWarning("Map was null in MapView.ShowMap()");
                return;
            }

            ClearMap();

            CreateMapParent();

            CreateNodes(map.nodes);

            DrawLines();

            SetOrientation();

            ResetNodesRotation();

            SetAttainableNodes();

            SetLineColors();

            CreateMapBackground(map);
        }
        private void CreateMapBackground(Map map)
        {
            if (background == null) return;

            var backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(mapParent.transform);
            var bossNode = mapNodes.FirstOrDefault(node => node.node.nodeType == NodeType.Boss);
            var span = map.DistanceBetweenFirstAndLastLayers();
            backgroundObject.transform.localPosition = new Vector3(bossNode.transform.localPosition.x, span / 2f, 0f);
            backgroundObject.transform.localRotation = Quaternion.identity;
            var sr = backgroundObject.AddComponent<SpriteRenderer>();
            sr.color = backgroundColor;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.sprite = background;
            sr.size = new Vector2(xSize, span + yOffset * 2f);
        }
        private void CreateMapParent()
        {
            firstParent = new GameObject("OuterMapParent");
            mapParent = new GameObject("MapParentWithAScroll");
            mapParent.transform.SetParent(firstParent.transform);
            var scrollNonUi = mapParent.AddComponent<ScrollNonUI>();
            scrollNonUi.freezeX = mapOrientation == MapOrientation.BottomToTop || mapOrientation == MapOrientation.TopToBottom;
            scrollNonUi.freezeY = mapOrientation == MapOrientation.LeftToRight || mapOrientation == MapOrientation.RightToLeft;
            var boxCollider = mapParent.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(100, 100, 1);
        }

        private void CreateNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var mapNode = CreateMapNode(node);
                mapNodes.Add(mapNode);
            }
        }
        private MapNode CreateMapNode(Node node)
        {
            var mapNodeObject = Instantiate(nodePrefab, mapParent.transform);
            var mapNode = mapNodeObject.GetComponent<MapNode>();
            var blueprint = GetBlueprint(node.blueprintName);
            mapNode.SetUp(node, blueprint);
            mapNode.transform.localPosition = node.position;
            return mapNode;
        }
        //갈 수 있는 노드
        public void SetAttainableNodes()
        {
            // 1) 모든 노드를 접근 불가능/잠김으로 설정
            foreach (var node in mapNodes)
                node.SetState(NodeStates.Locked);
            if (mapManager.currentMap.path.Count == 0)
            {
                // 아직 지도에서 출발하지 않았으니, 전체 첫번째 레이어를 도달 가능으로 설정
                foreach (var node in mapNodes.Where(n => n.node.point.y == 0))
                    node.SetState(NodeStates.Attainable);
            }
            else
            {
                // 맵에서 이동하기 시작했으니, 방문한 경로를 강조 표시
                foreach (var point in mapManager.currentMap.path)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeStates.Visited);
                }

                var currentPoint = mapManager.currentMap.path[mapManager.currentMap.path.Count - 1];
                var currentNode = mapManager.currentMap.GetNode(currentPoint);

                // 이동가능한 모든 노드를 도달 가능으로 설정
                foreach (var point in currentNode.outgoing)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeStates.Attainable);
                }
            }
        }
        public void SetLineColors()
        {
            // 처음엔 모든라인을 회색으로 설정
            foreach (var connection in lineConnections)
                connection.SetColor(lineLockedColor);

            //아직 지도에서 이동을 시작하지 않은 경우
            if (mapManager.currentMap.path.Count == 0)
                return;

            // 다음 노드로 나가는 연결라인을 갈 수 있는 색상으로 표시
            var currentPoint = mapManager.currentMap.path[mapManager.currentMap.path.Count - 1];
            var currentNode = mapManager.currentMap.GetNode(currentPoint);

            foreach (var point in currentNode.outgoing)
            {
                var lineConnection = lineConnections.FirstOrDefault(connect => connect.from.node == currentNode && connect.to.node.point.Equals(point));
                lineConnection?.SetColor(lineVisitedColor);
            }

            if (mapManager.currentMap.path.Count <= 1) return;

            for (var i = 0; i < mapManager.currentMap.path.Count - 1; i++)
            {
                var current = mapManager.currentMap.path[i];
                var next = mapManager.currentMap.path[i + 1];
                var lineConnection = lineConnections.FirstOrDefault(connect => connect.@from.node.point.Equals(current) && connect.to.node.point.Equals(next));

                lineConnection?.SetColor(lineVisitedColor);
            }
        }
        private void SetOrientation()
        {
            var scrollNonUi = mapParent.GetComponent<ScrollNonUI>();
            var span = mapManager.currentMap.DistanceBetweenFirstAndLastLayers();
            var bossNode = mapNodes.FirstOrDefault(node => node.node.nodeType == NodeType.Boss);
            Debug.Log("설정된 방향의 지도 범위 : " + span + "카메라 측면 : " + cam.aspect);

            // 첫번째 부모가 카메라 바로 앞에 있도록 설정 : 
            firstParent.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            var offset = orientationOffset;
            switch (mapOrientation)
            {
                case MapOrientation.BottomToTop:
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.yConstraints.max = 0;
                        scrollNonUi.yConstraints.min = -(span + 2f * offset);
                    }
                    firstParent.transform.localPosition += new Vector3(0, offset, 0);
                    break;
                case MapOrientation.TopToBottom:
                    mapParent.transform.eulerAngles = new Vector3(0, 0, 180);
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.yConstraints.min = 0;
                        scrollNonUi.yConstraints.max = span + 2f * offset;
                    }
                    // factor in map span:
                    firstParent.transform.localPosition += new Vector3(0, -offset, 0);
                    break;
                case MapOrientation.RightToLeft:
                    offset *= cam.aspect;
                    mapParent.transform.eulerAngles = new Vector3(0, 0, 90);
                    // factor in map span:
                    firstParent.transform.localPosition -= new Vector3(offset, bossNode.transform.position.y, 0);
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.xConstraints.max = span + 2f * offset;
                        scrollNonUi.xConstraints.min = 0;
                    }
                    break;
                case MapOrientation.LeftToRight:
                    offset *= cam.aspect;
                    mapParent.transform.eulerAngles = new Vector3(0, 0, -90);
                    firstParent.transform.localPosition += new Vector3(offset, -bossNode.transform.position.y, 0);
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.xConstraints.max = 0;
                        scrollNonUi.xConstraints.min = -(span + 2f * offset);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void DrawLines()
        {
            foreach (var node in mapNodes)
            {
                foreach (var connection in node.node.outgoing)
                    AddLineConnection(node, GetNode(connection));
            }
        }
        private void ResetNodesRotation()
        {
            foreach (var node in mapNodes)
                node.transform.rotation = Quaternion.identity;
        }
        public void AddLineConnection(MapNode from, MapNode to)
        {
            var lineObject = Instantiate(linePrefab, mapParent.transform);
            var lineRenderer = lineObject.GetComponent<LineRenderer>();
            var fromPoint = from.transform.position + (to.transform.position - from.transform.position).normalized * offsetFromNodes;
            var toPoint = to.transform.position + (from.transform.position - to.transform.position).normalized * offsetFromNodes;

            // 로컬에서 선 그리기
            lineObject.transform.position = fromPoint;
            lineRenderer.useWorldSpace = false;

            // 포인트가 두개인 라인렌더러는 투명도처리가 제대로 되지않음
            lineRenderer.positionCount = linePointsCount;
            for (var i = 0; i < linePointsCount; i++)
            {
                lineRenderer.SetPosition(i, Vector3.Lerp(Vector3.zero, toPoint - fromPoint, (float)i / (linePointsCount - 1)));
            }

            var dottedLine = lineObject.GetComponent<DottedLineRenderer>();
            if (dottedLine != null) dottedLine.ScaleMaterial();

            lineConnections.Add(new LineConnection(lineRenderer, from, to));
        }
        private MapNode GetNode(Point point)
        {
            return mapNodes.FirstOrDefault(n => n.node.point.Equals(point));
        }
        private MapConfig GetConfig(string configName)
        {
            return allMapConfigs.FirstOrDefault(c => c.name == configName);
        }
        public NodeBlueprint GetBlueprint(NodeType type)
        {
            var config = GetConfig(mapManager.currentMap.configName);
            return config.nodeBlueprints.FirstOrDefault(n => n.nodeType == type);
        }
        public NodeBlueprint GetBlueprint(string blueprintName)
        {
            var config = GetConfig(mapManager.currentMap.configName);
            return config.nodeBlueprints.FirstOrDefault(n => n.name == blueprintName);
        }

    }
}
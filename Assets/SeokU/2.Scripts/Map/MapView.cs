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

        // ���� �����ϴµ� ����� �� �ִ� MapConfig ��ũ��Ʈ ���
        public List<MapConfig> allMapConfigs;
        public GameObject nodePrefab;
        public float orientationOffset;                 //ȭ�� �����ڸ����� ������ ����/�� ��� offset

        [Header("Background Set")]
        public Sprite background;     // null�̸� ����� �Ⱥ���
        public Color backgroundColor = Color.white;
        public float xSize;
        public float yOffset;

        [Header("Line Set")]
        public GameObject linePrefab;
        [Range(3, 10)] public int linePointsCount = 10;
        public float offsetFromNodes = 0.5f;              // ��忡�� �� ������ ������ �Ÿ�

        [Header("Colors")]
        public Color visitedColor = Color.white;      //��� �湮 or �� �� �ִ°� ����
        public Color lockedColor = Color.gray;        //��� ��� ����
        public Color lineVisitedColor = Color.white;  //�湮 or ��� ���� ����
        public Color lineLockedColor = Color.gray;    //��� �� ����

        private GameObject firstParent;
        private GameObject mapParent;
        private List<List<Point>> paths;
        private Camera cam;

        //��� ��� :
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
        //�� �� �ִ� ���
        public void SetAttainableNodes()
        {
            // 1) ��� ��带 ���� �Ұ���/������� ����
            foreach (var node in mapNodes)
                node.SetState(NodeStates.Locked);
            if (mapManager.currentMap.path.Count == 0)
            {
                // ���� �������� ������� �ʾ�����, ��ü ù��° ���̾ ���� �������� ����
                foreach (var node in mapNodes.Where(n => n.node.point.y == 0))
                    node.SetState(NodeStates.Attainable);
            }
            else
            {
                // �ʿ��� �̵��ϱ� ����������, �湮�� ��θ� ���� ǥ��
                foreach (var point in mapManager.currentMap.path)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeStates.Visited);
                }

                var currentPoint = mapManager.currentMap.path[mapManager.currentMap.path.Count - 1];
                var currentNode = mapManager.currentMap.GetNode(currentPoint);

                // �̵������� ��� ��带 ���� �������� ����
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
            // ó���� �������� ȸ������ ����
            foreach (var connection in lineConnections)
                connection.SetColor(lineLockedColor);

            //���� �������� �̵��� �������� ���� ���
            if (mapManager.currentMap.path.Count == 0)
                return;

            // ���� ���� ������ ��������� �� �� �ִ� �������� ǥ��
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
            Debug.Log("������ ������ ���� ���� : " + span + "ī�޶� ���� : " + cam.aspect);

            // ù��° �θ� ī�޶� �ٷ� �տ� �ֵ��� ���� : 
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

            // ���ÿ��� �� �׸���
            lineObject.transform.position = fromPoint;
            lineRenderer.useWorldSpace = false;

            // ����Ʈ�� �ΰ��� ���η������� ����ó���� ����� ��������
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public static class MapGenerator
    {
        private static MapConfig config;

        private static readonly List<NodeType> randomNodes = new List<NodeType>
         { NodeType.Random, NodeType.Store, NodeType.Treasure, NodeType.NormalEnemy, NodeType.RestSite };

        private static List<float> layerDistances;
        private static List<List<Point>> paths;

        //레이어의 모든 노드
        private static readonly List<List<Node>> nodes = new List<List<Node>>();

        public static Map GetMap(MapConfig _config)
        {
            if (config == null)
            {
                Debug.LogWarning("Config was null in MapGenerator.Generate()");
                return null;
            }
            config = _config;
            nodes.Clear();

            GenerateLayerDistances();

            for (var i = 0; i < _config.layers.Count; i++)
                PlaceLayer(i);

            GeneratePaths();

            RandomizeNodePositions();

            SetUpConnections();

            RemoveCrossConnections();

            // 연결되있는 모든 노드를 선택
            var nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

            // 이 맵에 대한 보스를 랜덤 선택
            var bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == NodeType.Boss).ToList().Random().name;
            return new Map(_config.name, bossNodeName, nodesList, new List<Point>());
        }

        private static void GenerateLayerDistances()
        {
            layerDistances = new List<float>();
            foreach (var layer in config.layers)
                layerDistances.Add(layer.distanceFromPreviousLayer.GetValue());
        }
        private static float GetDistanceToLayer(int layerIndex)
        {
            if (layerIndex < 0 || layerIndex > layerDistances.Count) return 0f;

            return layerDistances.Take(layerIndex + 1).Sum();
        }
        private static void PlaceLayer(int layerIndex)
        {
            var layer = config.layers[layerIndex];
            var nodesOnThisLayer = new List<Node>();

            // 모든 노드가 중앙에 오도록 이 레이어의 오프셋 : 
            var offset = layer.nodesApartDistance * config.gridWidth / 2f;

            for (var i = 0; i < config.gridWidth; i++)
            {
                var nodeType = Random.Range(0f, 1f) < layer.randomizeNodes ? GetRandomNode() : layer.nodeType;
                var blueprintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().Random().name;
                var node = new Node(nodeType, blueprintName, new Point(i, layerIndex))
                {
                    position = new Vector2(-offset + i * layer.nodesApartDistance, GetDistanceToLayer(layerIndex))
                };
                nodesOnThisLayer.Add(node);
            }
            nodes.Add(nodesOnThisLayer);
        }
        private static void RandomizeNodePositions()
        {
            for (var index = 0; index < nodes.Count; index++)
            {
                var list = nodes[index];
                var layer = config.layers[index];
                var distToNextLayer = index + 1 >= layerDistances.Count ? 0f : layerDistances[index + 1];
                var distToPreviousLayer = layerDistances[index];

                foreach (var node in list)
                {
                    var xRandom = Random.Range(-1f, 1f);
                    var yRandom = Random.Range(-1f, 1f);

                    var x = xRandom * layer.nodesApartDistance / 2f;
                    var y = yRandom < 0 ? distToPreviousLayer * yRandom / 2f : distToNextLayer * yRandom / 2f;

                    node.position += new Vector2(x, y) * layer.randomizePosition;
                }
            }
        }
        private static void SetUpConnections()
        {
            foreach (var path in paths)
            {
                for (var i = 0; i < path.Count - 1; ++i)
                {
                    var node = GetNode(path[i]);
                    var nextNode = GetNode(path[i + 1]);
                    node.AddOutgoing(nextNode.point);
                    nextNode.AddIncoming(node.point);
                }
            }
        }
        private static void RemoveCrossConnections()
        {
            for (var i = 0; i < config.gridWidth - 1; ++i)
                for (var j = 0; j < config.layers.Count - 1; ++j)
                {
                    var node = GetNode(new Point(i, j));
                    if (node == null || node.HasNoConnections()) continue;
                    var right = GetNode(new Point(i + 1, j));
                    if (right == null || right.HasNoConnections()) continue;
                    var top = GetNode(new Point(i, j + 1));
                    if (top == null || top.HasNoConnections()) continue;
                    var topRight = GetNode(new Point(i + 1, j + 1));
                    if (topRight == null || topRight.HasNoConnections()) continue;

                    Debug.Log("노드 연결 검사 : " + node.point);
                    if (!node.outgoing.Any(element => element.Equals(topRight.point))) continue;
                    if (!right.outgoing.Any(element => element.Equals(top.point))) continue;

                    Debug.Log("교차노드 : " + node.point);

                    //교차노드를 찾았으면
                    //직접 연결 추가 : 
                    node.AddOutgoing(top.point);
                    top.AddIncoming(node.point);

                    right.AddOutgoing(topRight.point);
                    topRight.AddIncoming(right.point);

                    var random = Random.Range(0f, 1f);
                    if (random < 0.2f)
                    {
                        //두 교차연결 제거
                        // 1)
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);
                        // 2)
                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                    else if (random < 0.6f)
                    {
                        // 1)
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);
                    }
                    else
                    {
                        // 2)
                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                }
        }
        private static Node GetNode(Point point)
        {
            if (point.y >= nodes.Count) return null;
            if (point.x >= nodes[point.y].Count) return null;

            return nodes[point.y][point.x];
        }
        private static Point GetFinalNode()
        {
            var y = config.layers.Count - 1;
            if (config.gridWidth % 2 == 1)
                return new Point(config.gridWidth / 2, y);
            return Random.Range(0, 2) == 0
                ? new Point(config.gridWidth / 2, y)
                : new Point(config.gridWidth / 2 - 1, y);
        }
        private static void GeneratePaths()
        {
            var finalNode = GetFinalNode();
            paths = new List<List<Point>>();
            var numOfStartingNodes = config.numOfStartingNodes.GetValue();
            var numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

            var candidateXs = new List<int>();
            for (var i = 0; i < config.gridWidth; i++)
                candidateXs.Add(i);

            candidateXs.Shuffle();
            var startingXs = candidateXs.Take(numOfStartingNodes);
            var startingPoints = (from x in startingXs select new Point(x, 0)).ToList();

            candidateXs.Shuffle();
            var preBossXs = candidateXs.Take(numOfPreBossNodes);
            var preBossPoints = (from x in preBossXs select new Point(x, finalNode.y - 1)).ToList();

            int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes) + Mathf.Max(0, config.extraPaths);
            for (int i = 0; i < numOfPaths; ++i)
            {
                Point startNode = startingPoints[i % numOfStartingNodes];
                Point endNode = preBossPoints[i % numOfPreBossNodes];
                var path = Path(startNode, endNode);
                path.Add(finalNode);
                paths.Add(path);
            }
        }

        // 랜덤경로 아래서 위로 생성
        private static List<Point> Path(Point fromPoint, Point toPoint)
        {
            int toRow = toPoint.y;
            int toCol = toPoint.x;

            int lastNodeCol = fromPoint.x;

            var path = new List<Point> { fromPoint };
            var candidateCols = new List<int>();
            for (int row = 1; row < toRow; ++row)
            {
                candidateCols.Clear();

                int verticalDistance = toRow - row;
                int horizontalDistance;

                int forwardCol = lastNodeCol;
                horizontalDistance = Mathf.Abs(toCol - forwardCol);
                if (horizontalDistance <= verticalDistance)
                    candidateCols.Add(lastNodeCol);

                int leftCol = lastNodeCol - 1;
                horizontalDistance = Mathf.Abs(toCol - leftCol);
                if (leftCol >= 0 && horizontalDistance <= verticalDistance)
                    candidateCols.Add(leftCol);

                int rightCol = lastNodeCol + 1;
                horizontalDistance = Mathf.Abs(toCol - rightCol);
                if (rightCol < config.gridWidth && horizontalDistance <= verticalDistance)
                    candidateCols.Add(rightCol);

                int randomCandidateIndex = Random.Range(0, candidateCols.Count);
                int candidateCol = candidateCols[randomCandidateIndex];
                var nextPoint = new Point(candidateCol, row);

                path.Add(nextPoint);

                lastNodeCol = candidateCol;
            }

            path.Add(toPoint);

            return path;
        }
        private static NodeType GetRandomNode()
        {
            return randomNodes[Random.Range(0, randomNodes.Count)];
        }
    }
}
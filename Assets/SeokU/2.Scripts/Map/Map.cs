using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Json.Net;
using UnityEngine;

namespace Map
{
    public class Map
    {        
        public List<Node> nodes;
        public List<Point> path;
        public string bossNodeName;
        public string configName;

        public Map(string configName, string bossNodeName, List<Node> nodes, List<Point> path)
        {
            this.configName = configName;
            this.bossNodeName = bossNodeName;
            this.nodes = nodes;
            this.path = path;
        }
        public Node GetBossNode()
        {
            // linq.FirstOrDefault : 시퀀스의 첫번째 요소를 반환하거나, 요소가 없으면 기본값을 반환.
            return nodes.FirstOrDefault(n => n.nodeType == NodeType.Boss);
        }
        public float DistanceBetweenFirstAndLastLayers()
        {
            var bossNode = GetBossNode();
            var firstLayerNode = nodes.FirstOrDefault(n => n.point.y == 0);
            if (bossNode == null || firstLayerNode == null)
                return 0f;

            return bossNode.position.y - firstLayerNode.position.y;
        }
        public Node GetNode(Point point)
        {
            return nodes.FirstOrDefault(n => n.point.Equals(point));
        }
        public string ToJson()
        {
            var setting = new JsonSerializerSettings();
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            return JsonConvert.SerializeObject(this, setting);
        }        
    }
}
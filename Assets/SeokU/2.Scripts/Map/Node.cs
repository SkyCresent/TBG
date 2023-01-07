using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Json.Net;

namespace Map
{
    public class Node
    {
        public readonly Point point;
        public readonly List<Point> incoming = new List<Point>();
        public readonly List<Point> outgoing = new List<Point>();
        [JsonConverter(typeof(StringEnumConverter))]        
        public readonly NodeType nodeType;
        public readonly string blueprintName;
        public Vector2 position;
        public Node(NodeType nodeType, string blueprintName, Point point)
        {
            this.nodeType = nodeType;
            this.blueprintName = blueprintName;
            this.point = point;
        }
        public void AddIncoming(Point point)
        {
            if (incoming.Any(element => element.Equals(point)))
                return;

            incoming.Add(point);
        }
        public void AddOutgoing(Point point)
        {
            if (outgoing.Any(element => element.Equals(point)))
                return;

            outgoing.Add(point);
        }
        public void RemoveIncoming(Point point)
        {
            incoming.RemoveAll(element => element.Equals(point));
        }
        public void RemoveOutgoing(Point point)
        {
            outgoing.RemoveAll(element => element.Equals(point));
        }
        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }
    }
}
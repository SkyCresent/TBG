using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
//using Json.Net;

namespace Map
{
    public class Node
    {
        // const : 컴파일 상수 (컴파일 시 const 변수의 값을 가져온다.)
        // 내장 자료형(정수형, 실수형, Enum, String)에 대해서만 사용 할 수있다.
        // 변수 선언과 동시에 값을 할당 해야 한다.
        // 메모리 할당 위치는 Stack. 단 static 선언을 하면 Heap에 저장 가능

        // readonly : 런타임 상수 (exe 또는 dll을 사용할 때 변수의 값을 가져온다.)
        // 모든 자료형에 사용 할 수 있으며, 생성과 동시에 초기화 할 필요는 없다.
        // 단, 생성자 단계에서 단1번 할당을 통해 초기화 할 수 있다.
        // 메모리 할당 위치는 Heap이다.

        // const는 stack에 저장 되기 때문에 접근이 빠르다는 장점이 있지만, 
        // 컴파일 상수이기 때문에 const 변수 값이 바뀌는 경우 해당 프로젝트 뿐만 아니라 참조 받거나 영향을 받는 프로젝트 모두 재 컴파일을 해야하는 단점
        // readonly는 생성시 선언하지 않아도, 생성자에서 초기화하여 사용 가능하기 때문에 유연하며 실제 사용하는 단계에서 변수의 값을 가져오는 장점

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
            // linq.Any : 집합 안에 조건에 맞는 요소가 하나라도 있는지 확인하는 메서드
            // linq.All : 집합안에 모든 요소가 조건에 맞는지 확인하는 메서드
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
using UnityEngine;
using OneLine;

namespace Map
{
    [System.Serializable]
    public class MapLayer
    {
        // 이 맵 레이어의 기본 노드, RandomizeNodes가 0이면 100% 이 노드를 얻게됨
        public NodeType nodeType;
        [OneLineWithHeader]
        public FloatMinMax distanceFromPreviousLayer;       // 이전 노드와의 거리
        public float nodesApartDistance;                    // 이 레이어의 노드 간 거리
        [Range(0f, 1f)] public float randomizePosition;     // 0으로 설정하면 이 레이어의 노드가 직선으로 나타남. 1f에 가까우면 더 많은 위치 무작위
        [Range(0f, 1f)] public float randomizeNodes;        // 이 레이어의 기본 노드와 다른 임의의 노드를 얻을 수 있는 기회
    }
}
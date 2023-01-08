using System.Collections.Generic;
using UnityEngine;
using Reorderable;
using OneLine;

namespace Map
{
    [CreateAssetMenu]
    public class MapConfig : ScriptableObject
    {
        public List<NodeBlueprint> nodeBlueprints;
        // Mathf.Max : a 와 b 중에 더 큰값을 반환
        public int gridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

        [OneLineWithHeader]
        public IntMinMax numOfPreBossNodes;   //보스 이전 노드 수
        [OneLineWithHeader]
        public IntMinMax numOfStartingNodes;  //시작 노드 수

        public int extraPaths;                // 숫자를 높히면 더 많은 경로를 생성함
        [Reorderable]
        public ListOfMapLayers layers;

        [System.Serializable]
        public class ListOfMapLayers : ReorderableArray<MapLayer>
        {
        }
    }
}
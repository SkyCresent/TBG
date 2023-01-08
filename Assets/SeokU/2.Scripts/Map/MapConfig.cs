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
        // Mathf.Max : a �� b �߿� �� ū���� ��ȯ
        public int gridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

        [OneLineWithHeader]
        public IntMinMax numOfPreBossNodes;   //���� ���� ��� ��
        [OneLineWithHeader]
        public IntMinMax numOfStartingNodes;  //���� ��� ��

        public int extraPaths;                // ���ڸ� ������ �� ���� ��θ� ������
        [Reorderable]
        public ListOfMapLayers layers;

        [System.Serializable]
        public class ListOfMapLayers : ReorderableArray<MapLayer>
        {
        }
    }
}
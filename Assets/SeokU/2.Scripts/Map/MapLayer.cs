using UnityEngine;
using OneLine;

namespace Map
{
    [System.Serializable]
    public class MapLayer
    {
        // �� �� ���̾��� �⺻ ���, RandomizeNodes�� 0�̸� 100% �� ��带 ��Ե�
        public NodeType nodeType;
        [OneLineWithHeader]
        public FloatMinMax distanceFromPreviousLayer;       // ���� ������ �Ÿ�
        public float nodesApartDistance;                    // �� ���̾��� ��� �� �Ÿ�
        [Range(0f, 1f)] public float randomizePosition;     // 0���� �����ϸ� �� ���̾��� ��尡 �������� ��Ÿ��. 1f�� ������ �� ���� ��ġ ������
        [Range(0f, 1f)] public float randomizeNodes;        // �� ���̾��� �⺻ ���� �ٸ� ������ ��带 ���� �� �ִ� ��ȸ
    }
}
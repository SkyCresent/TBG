using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Map
{
    public class MapPlayerTracker : MonoBehaviour
    {
        public bool lockAfterSelecting = false;
        public float enterNodeDelay = 1f;
        public MapManager mapManager;
        public MapView mapView;

        public static MapPlayerTracker Instance;

        public bool isLocked { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void SelectNode(MapNode mapNode)
        {
            if (isLocked) return;

            Debug.Log("���õ� ��� : " + mapNode.node.point);

            if (mapManager.currentMap.path.Count == 0)
            {
                // �÷��̾ �����ѳ�尡 ���� �� y = 0�� ����� �ϳ��� ���� ����
                if (mapNode.node.point.y == 0)
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
            else
            {
                var currentPoint = mapManager.currentMap.path[mapManager.currentMap.path.Count - 1];
                var currentNode = mapManager.currentMap.GetNode(currentPoint);

                if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.node.point)))
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
        }
        private void SendPlayerToNode(MapNode mapNode)
        {
            isLocked = lockAfterSelecting;
            mapManager.currentMap.path.Add(mapNode.node.point);
            mapManager.SaveMap();
            mapView.SetAttainableNodes();
            mapView.SetLineColors();
            mapNode.ShowSwirlAnimation();

            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode));
        }
        private static void EnterNode(MapNode mapNode)
        {
            //�������Ʈ �̸�
            Debug.Log("�� ��� : " + mapNode.node.blueprintName + "�� Ÿ�� : " + mapNode.node.nodeType);
            //���Ÿ������ ������ ����� �ε� 
            //�Ǵ� �������� guiǥ��
            //gui�� ǥ���ϵ��� ������ ��� MapPlayerTracker���� isLocked�� �ٽ� false�� ����
            switch (mapNode.node.nodeType)
            {
                case NodeType.NormalEnemy:
                    break;
                case NodeType.EliteEnemy:
                    break;
                case NodeType.RestSite:
                    break;
                case NodeType.Treasure:
                    break;
                case NodeType.Store:
                    break;
                case NodeType.Boss:
                    break;
                case NodeType.Random:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void PlayWarningThatNodeCannotBeAccessed()
        {
            Debug.Log("������ ���� ���� �� �� �����ϴ�");
        }
    }
}
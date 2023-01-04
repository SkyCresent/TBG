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

            Debug.Log("선택된 노드 : " + mapNode.node.point);

            if (mapManager.currentMap.path.Count == 0)
            {
                // 플레이어가 선택한노드가 없을 때 y = 0인 노드중 하나를 선택 가능
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
            //블루프린트 이름
            Debug.Log("들어간 노드 : " + mapNode.node.blueprintName + "의 타입 : " + mapNode.node.nodeType);
            //노드타입으로 적절한 장면을 로드 
            //또는 지도위에 gui표시
            //gui를 표시하도록 선택한 경우 MapPlayerTracker에서 isLocked를 다시 false로 설정
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
            Debug.Log("선택한 노드는 접근 할 수 없습니다");
        }
    }
}
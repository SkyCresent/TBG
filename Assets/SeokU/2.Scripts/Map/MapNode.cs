using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace Map
{
    public enum NodeStates
    {
        Locked, Visited, Attainable
    }
}
namespace Map
{
    public class MapNode : MonoBehaviour
    {
        public SpriteRenderer sr;
        public SpriteRenderer visitedCheck;
        public Image visitedCheckImage;
        
        // private set : 읽기전용
        // private set : 외부에서는 접근을 못하지만 내부에서는 접근이 가능
        // set이 없는형태 : 내부 외부 가릴것 없이 접근 불가
        public Node node { get; private set; }
        public NodeBlueprint blueprint { get; private set; }

        private float initScale;
        private const float hoverScaleFactor = 1.2f;
        private float mouseDownTime;
        private int switch_on;
        private const float maxClickDuration = 0.5f;

        public void SetUp(Node _node, NodeBlueprint _blueprint)
        {
            node = _node;
            blueprint = _blueprint;
            sr.sprite = _blueprint.sprite;
            if (_node.nodeType == NodeType.Boss) transform.localScale *= 1.5f;
            initScale = sr.transform.localScale.x;
            visitedCheck.color = MapView.Instance.visitedColor;
            visitedCheck.gameObject.SetActive(false);
            SetState(NodeStates.Locked);
        }
        public void SetState(NodeStates state)
        {
            visitedCheck.gameObject.SetActive(false);
            switch (state)
            {
                case NodeStates.Locked:
                    sr.DOKill();
                    sr.color = MapView.Instance.lockedColor;
                    break;
                case NodeStates.Visited:
                    sr.DOKill();
                    sr.color = MapView.Instance.visitedColor;
                    break;
                case NodeStates.Attainable:
                    sr.color = MapView.Instance.lockedColor;
                    sr.DOKill();
                    sr.DOColor(MapView.Instance.visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        private void OnMouseEnter()
        {
            sr.transform.DOKill();
            sr.transform.DOScale(initScale * hoverScaleFactor, 0.3f);
        }
        private void OnMouseExit()
        {
            sr.transform.DOKill();
            sr.transform.DOScale(initScale, 0.3f);
        }
        private void OnMouseDown()
        {
            mouseDownTime = Time.time;
        }
        private void OnMouseUp()
        {
            if (Time.time - mouseDownTime < maxClickDuration)
            {
                //플레이어가 이 노드를 클릭 했을 때
                MapPlayerTracker.Instance.SelectNode(this);
            }
        }
        public void ShowSwirlAnimation()
        {
            if (visitedCheckImage == null)
                return;

            const float fillDuration = 0.3f;
            visitedCheckImage.fillAmount = 0f;

            DOTween.To(() => visitedCheckImage.fillAmount, x => visitedCheckImage.fillAmount = x, 1f, fillDuration);
        }
    }
}
using UnityEngine;
using DG.Tweening;

namespace Map
{
    public class ScrollNonUI : MonoBehaviour
    {
        public float tweenBackDuration = 0.3f;
        public Ease tweenBackEase;
        public bool freezeX;
        public FloatMinMax xConstraints = new FloatMinMax();
        public bool freezeY;
        public FloatMinMax yConstraints = new FloatMinMax();
        private Vector2 offset;
        private Vector3 pointerDisplacement;  //중심에서 드래그를 시작하기 위해 클릭한 지점까지의 거리
        private float zDisplacement;
        private bool dragging;
        private Camera main;

        private void Awake()
        {
            main = Camera.main;
            zDisplacement = -main.transform.position.z + transform.rotation.z;
        }

        private void OnMouseDown()
        {
            pointerDisplacement = -transform.position + MouseInWorldCoords();
            transform.DOKill();
            dragging = true;
        }
        private void OnMouseUp()
        {
            dragging = false;
            TweenBack();
        }
        private void Update()
        {
            if (!dragging) return;

            var mousePos = MouseInWorldCoords();

            transform.position = new Vector3(
                freezeX ? transform.position.x : mousePos.x - pointerDisplacement.x,
                freezeY ? transform.position.y : mousePos.y - pointerDisplacement.y,
                transform.position.z);
        }
        /// <summary>
        /// GameObject가 따라갈 월드좌표의 마우스 위치 반환
        /// </summary>
        /// <returns></returns>
        private Vector3 MouseInWorldCoords()
        {
            var screenMousePos = Input.mousePosition;
            screenMousePos.z = zDisplacement;
            return main.ScreenToWorldPoint(screenMousePos);
        }
        private void TweenBack()
        {
            if (freezeY)
            {
                if (transform.localPosition.x >= xConstraints.min && transform.localPosition.x <= xConstraints.max)
                    return;

                var targetX = transform.localPosition.x < xConstraints.min ? xConstraints.min : xConstraints.max;
                transform.DOLocalMoveX(targetX, tweenBackDuration).SetEase(tweenBackEase);
            }
            else if (freezeX)
            {
                if (transform.localPosition.y >= yConstraints.min && transform.localPosition.y <= yConstraints.max)
                    return;

                var targetY = transform.localPosition.y < yConstraints.min ? yConstraints.min : yConstraints.max;
                transform.DOLocalMoveY(targetY, tweenBackDuration).SetEase(tweenBackEase);
            }
        }
    }
}
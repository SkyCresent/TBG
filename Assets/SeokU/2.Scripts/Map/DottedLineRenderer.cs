using UnityEngine;

namespace Map
{
    public class DottedLineRenderer : MonoBehaviour
    {
        public bool scaleInUpdate = false;
        private LineRenderer lr;
        private Renderer rd;

        private void Start()
        {
            ScaleMaterial();
            enabled = scaleInUpdate;
        }
        public void ScaleMaterial()
        {
            lr = GetComponent<LineRenderer>();
            rd = GetComponent<Renderer>();
            rd.material.mainTextureScale = new Vector2(Vector2.Distance(lr.GetPosition(0), lr.GetPosition(lr.positionCount - 1)) / lr.widthMultiplier, 1);
        }

        private void Update()
        {
            rd.material.mainTextureScale = new Vector2(Vector2.Distance(lr.GetPosition(0), lr.GetPosition(lr.positionCount - 1)) / lr.widthMultiplier, 1);
        }
    }
}
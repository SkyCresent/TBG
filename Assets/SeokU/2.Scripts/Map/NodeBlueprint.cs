using UnityEngine;

namespace Map
{
    public enum NodeType
    {
        NormalEnemy,
        EliteEnemy,
        RestSite,
        Treasure,
        Store,
        Boss,
        Random
    }
}
namespace Map
{
    [CreateAssetMenu]
    public class NodeBlueprint : ScriptableObject
    {
        public Sprite sprite;
        public NodeType nodeType;
    }
}
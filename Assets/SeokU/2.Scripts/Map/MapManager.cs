using System.Linq;
using UnityEngine;
using Newtonsoft.Json;


//TBG Project �����ؾ��Ұ�

//��巹���� ����

//system.Linq

//readonly, const

//IEquatable<T>

//ReferenceEquals

//GetHashCode

//���ٽ�

//JsonConvert.SerializeObject(this, Formatting.Indented);

//FirstOrDefault(Linq)

//3�� ������
namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public MapConfig config;
        public MapView mapView;

        public Map currentMap { get; private set; }

        private void Start()
        {
            if (PlayerPrefs.HasKey("Map"))
            {
                var mapJson = PlayerPrefs.GetString("Map");
                var map = JsonConvert.DeserializeObject<Map>(mapJson);
                // .Contains() ��� �ᵵ��
                if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
                {
                    // �÷��̾ �̹� �������� ��������, �� �� ����
                    GenerateNewMap();
                }
                else
                {
                    currentMap = map;
                    // �÷��̾ ���� �������� ���� ��������, ���� �� �ε�
                    mapView.ShowMap(map);
                }
            }
            else
            {
                GenerateNewMap();
            }
        }
        public void GenerateNewMap()
        {
            var map = MapGenerator.GetMap(config);
            currentMap = map;
            Debug.Log(map.ToJson());
            mapView.ShowMap(map);
        }
        public void SaveMap()
        {
            if (currentMap == null) return;

            var json = JsonConvert.SerializeObject(currentMap);
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
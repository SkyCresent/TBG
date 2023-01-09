using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Json.Net;

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
            // PlayerPrefs.HasKey("string") : Ű ���� �����ϸ� true
            if (PlayerPrefs.HasKey("Map"))
            {
                var mapJson = PlayerPrefs.GetString("Map");
                var map = JsonConvert.DeserializeObject<Map>(mapJson);
                // linq.Any : ���� �ȿ� ���ǿ� �´� ��Ұ� �ϳ��� �ִ��� Ȯ���ϴ� �޼���
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

            var setting = new JsonSerializerSettings();
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            
            var json = JsonConvert.SerializeObject(currentMap, setting);
            //PlayerPrefs.SetString : string �����ڵ�
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
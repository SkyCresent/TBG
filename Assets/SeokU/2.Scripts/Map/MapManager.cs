using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Json.Net;

//TBG Project 공부해야할것

//어드레서블 에셋

//system.Linq

//readonly, const

//IEquatable<T>

//ReferenceEquals

//GetHashCode

//람다식

//JsonConvert.SerializeObject(this, Formatting.Indented);

//FirstOrDefault(Linq)

//3항 연산자
namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public MapConfig config;
        public MapView mapView;

        public Map currentMap { get; private set; }

        private void Start()
        {
            // PlayerPrefs.HasKey("string") : 키 값이 존재하면 true
            if (PlayerPrefs.HasKey("Map"))
            {
                var mapJson = PlayerPrefs.GetString("Map");
                var map = JsonConvert.DeserializeObject<Map>(mapJson);
                // linq.Any : 집합 안에 조건에 맞는 요소가 하나라도 있는지 확인하는 메서드
                // .Contains() 대신 써도됨
                if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
                {
                    // 플레이어가 이미 보스에게 도착했음, 새 맵 생성
                    GenerateNewMap();
                }
                else
                {
                    currentMap = map;
                    // 플레이어가 아직 보스에게 도착 못했으면, 현재 맵 로드
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
            //PlayerPrefs.SetString : string 저장코드
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
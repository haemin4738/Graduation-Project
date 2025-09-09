using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace TempleRun
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private int tileStartCount = 10; // 초기 생성할 타일 수
        [SerializeField] private int minimumStraightTiles = 3; // 직선 타일 최소 수
        [SerializeField] private int maximumStraighTiles = 15; // 직선 타일 최대 수
        [SerializeField] private GameObject startingTile; // 직선 기본 타일 프리팹
        [SerializeField] private List<GameObject> turnTiles; // 방향 전환 타일 프리팹 리스트
        [SerializeField] private List<GameObject> obstacles; // 장애물 프리팹 리스트
        [SerializeField] private Material[] floorMaterials; // 타일에 적용할 바닥 재질들

        private Vector3 currentTileLocation = Vector3.zero; // 현재 타일을 생성할 위치
        private Vector3 currentTileDirection = Vector3.forward; // 현재 타일 생성 방향
        private GameObject prevTile; // 마지막으로 생성한 타일 참조

        private List<GameObject> currentTiles; // 현재 씬에 존재하는 타일 리스트
        private List<GameObject> currentObstacles; // 현재 씬에 존재하는 장애물 리스트

        private void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond); // 무작위성 초기화

            // 초기 직선 타일 생성
            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>());
            }

            // 초기 방향 전환 타일 추가
            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
        }

        // 타일 생성 함수
        private void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            // 현재 방향에 맞게 회전 적용
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            // 타일 생성
            prevTile = Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            // 랜덤 재질 적용
            MeshRenderer renderer = prevTile.GetComponent<MeshRenderer>();
            if (renderer != null && floorMaterials != null && floorMaterials.Length > 0)
            {
                int materialIndex = Random.Range(0, floorMaterials.Length);
                renderer.material = floorMaterials[materialIndex];
            }

            // 장애물 생성 여부
            if (spawnObstacle) SpawnObstacle();

            // 직선 타일일 경우, 다음 타일 위치 계산
            if (tile.type == TileType.STRAIGHT)
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }

        // 이전 타일 및 장애물 제거
        private void DeletePreviousTiles()
        {
            while (currentTiles.Count != 1) // 마지막 타일 하나는 유지
            {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
            }

            while (currentObstacles.Count != 0)
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }

        // 새로운 방향 추가 (방향 전환 시 호출)
        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;

            // 새 방향에 맞게 다음 타일 위치 계산
            Vector3 tilePlacementScale;
            if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
            {
                tilePlacementScale = Vector3.Scale(
                    prevTile.GetComponent<Renderer>().bounds.size / 2 + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2),
                    currentTileDirection);
            }
            else
            {
                tilePlacementScale = Vector3.Scale(
                    (prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2),
                    currentTileDirection);
            }

            currentTileLocation += tilePlacementScale;

            // 이전 타일 제거
            DeletePreviousTiles();

            // 새 방향으로 일정 수의 직선 타일 생성
            int currentPathLength = Random.Range(minimumStraightTiles, maximumStraighTiles);
            for (int i = 0; i < currentPathLength; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true); // 첫 타일에는 장애물 생성 안 함
            }

            // 마지막에 방향 전환 타일 추가
            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
        }

        // 타일의 재질을 업데이트 (현재 사용되지 않음)
        private void UpdateFloorMaterial()
        {
            foreach (GameObject tile in currentTiles)
            {
                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                if (renderer != null && floorMaterials != null && floorMaterials.Length > 0)
                {
                    int materialIndex = Random.Range(0, floorMaterials.Length);
                    renderer.material = floorMaterials[materialIndex];
                }
            }
        }

        // 장애물 생성
        private void SpawnObstacle()
        {
            if (Random.value > 0.2f) return; // 20% 확률로 장애물 생성

            GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
            Quaternion newObjectRotation = obstaclePrefab.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
            currentObstacles.Add(obstacle);
        }

        // 리스트에서 랜덤 GameObject 선택
        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }
    }
}

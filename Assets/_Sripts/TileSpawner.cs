using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace TempleRun
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private int tileStartCount = 10; // �ʱ� ������ Ÿ�� ��
        [SerializeField] private int minimumStraightTiles = 3; // ���� Ÿ�� �ּ� ��
        [SerializeField] private int maximumStraighTiles = 15; // ���� Ÿ�� �ִ� ��
        [SerializeField] private GameObject startingTile; // ���� �⺻ Ÿ�� ������
        [SerializeField] private List<GameObject> turnTiles; // ���� ��ȯ Ÿ�� ������ ����Ʈ
        [SerializeField] private List<GameObject> obstacles; // ��ֹ� ������ ����Ʈ
        [SerializeField] private Material[] floorMaterials; // Ÿ�Ͽ� ������ �ٴ� ������

        private Vector3 currentTileLocation = Vector3.zero; // ���� Ÿ���� ������ ��ġ
        private Vector3 currentTileDirection = Vector3.forward; // ���� Ÿ�� ���� ����
        private GameObject prevTile; // ���������� ������ Ÿ�� ����

        private List<GameObject> currentTiles; // ���� ���� �����ϴ� Ÿ�� ����Ʈ
        private List<GameObject> currentObstacles; // ���� ���� �����ϴ� ��ֹ� ����Ʈ

        private void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond); // �������� �ʱ�ȭ

            // �ʱ� ���� Ÿ�� ����
            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>());
            }

            // �ʱ� ���� ��ȯ Ÿ�� �߰�
            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
        }

        // Ÿ�� ���� �Լ�
        private void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            // ���� ���⿡ �°� ȸ�� ����
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            // Ÿ�� ����
            prevTile = Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            // ���� ���� ����
            MeshRenderer renderer = prevTile.GetComponent<MeshRenderer>();
            if (renderer != null && floorMaterials != null && floorMaterials.Length > 0)
            {
                int materialIndex = Random.Range(0, floorMaterials.Length);
                renderer.material = floorMaterials[materialIndex];
            }

            // ��ֹ� ���� ����
            if (spawnObstacle) SpawnObstacle();

            // ���� Ÿ���� ���, ���� Ÿ�� ��ġ ���
            if (tile.type == TileType.STRAIGHT)
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }

        // ���� Ÿ�� �� ��ֹ� ����
        private void DeletePreviousTiles()
        {
            while (currentTiles.Count != 1) // ������ Ÿ�� �ϳ��� ����
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

        // ���ο� ���� �߰� (���� ��ȯ �� ȣ��)
        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;

            // �� ���⿡ �°� ���� Ÿ�� ��ġ ���
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

            // ���� Ÿ�� ����
            DeletePreviousTiles();

            // �� �������� ���� ���� ���� Ÿ�� ����
            int currentPathLength = Random.Range(minimumStraightTiles, maximumStraighTiles);
            for (int i = 0; i < currentPathLength; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true); // ù Ÿ�Ͽ��� ��ֹ� ���� �� ��
            }

            // �������� ���� ��ȯ Ÿ�� �߰�
            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
        }

        // Ÿ���� ������ ������Ʈ (���� ������ ����)
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

        // ��ֹ� ����
        private void SpawnObstacle()
        {
            if (Random.value > 0.2f) return; // 20% Ȯ���� ��ֹ� ����

            GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
            Quaternion newObjectRotation = obstaclePrefab.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
            currentObstacles.Add(obstacle);
        }

        // ����Ʈ���� ���� GameObject ����
        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }
    }
}

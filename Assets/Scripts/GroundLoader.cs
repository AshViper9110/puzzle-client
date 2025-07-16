using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GroundLoader : MonoBehaviour
{
    // �^�C���}�b�v�Ǝg�p����^�C��/�v���n�u
    public Tilemap tilemap;
    public Tile groundTile;
    public GameObject boxPrefab;
    public GameObject goalPrefab;

    void Start()
    {
        // �^�C�������������A�g�𐶐�
        tilemap.ClearAllTiles();
        GenerateGroundFrameCentered();

        // ���݂̃X�e�[�WID����X�e�[�W�f�[�^��API�o�R�Ŏ擾
        StartCoroutine(LoadGroundMapFromApi(GameMane.CurrentStageID));
    }

    // API����X�e�[�W�f�[�^���擾���A�}�b�v�𐶐�����R���[�`��
    IEnumerator LoadGroundMapFromApi(int stageId)
    {
        string url = "http://localhost:8000/api/stages/" + stageId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // ���N�G�X�g���M
            yield return request.SendWebRequest();

            // �ʐM�G���[�܂��̓X�e�[�W�����݂��Ȃ��ꍇ�̓Z���N�g��ʂɖ߂�
            if (request.result != UnityWebRequest.Result.Success)
            {
                SceneManager.LoadScene("StageSelectScene");
                yield break;
            }

            // ���X�|���X�iJSON������j���擾
            string json = request.downloadHandler.text;

            // JSON���p�[�X���ăf�[�^�ɕϊ�
            GroundMapData mapData = JsonUtility.FromJson<GroundMapData>(json);

            // �n�`�������Ƙg�̍ĕ`��
            tilemap.ClearAllTiles();
            GenerateGroundFrameCentered();

            // �擾�����Z���������ɒn�`�E�I�u�W�F�N�g�𐶐�
            foreach (Cell cell in mapData.cells)
            {
                Vector3Int cellPos = new Vector3Int(cell.x, cell.y, 0);

                switch (cell.type)
                {
                    case "ground":
                        tilemap.SetTile(cellPos, groundTile);
                        break;

                    case "box":
                        if (boxPrefab != null)
                        {
                            Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                            Instantiate(boxPrefab, worldPos, Quaternion.identity);
                        }
                        break;

                    case "goal":
                        if (goalPrefab != null)
                        {
                            Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                            Instantiate(goalPrefab, worldPos, Quaternion.identity);
                        }
                        break;
                }
            }
        }
    }

    // �X�e�[�W�̊O�g�i�n�ʁj�𒆐S����ɐ�������
    void GenerateGroundFrameCentered()
    {
        int width = 17;
        int height = 11;

        int xMin = (-width / 2) - 1;
        int xMax = width / 2;
        int yMin = -height / 2;
        int yMax = height / 2;

        tilemap.ClearAllTiles();

        // �O�g�݂̂Ƀ^�C����z�u
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (x == xMin || x == xMax || y == yMin || y == yMax)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }
        }
    }

    // API����󂯎��Z�����
    [System.Serializable]
    public class Cell
    {
        public int x;
        public int y;
        public string type; // "ground", "box", "goal"
    }

    // API����󂯎��X�e�[�W�S�̂̃f�[�^
    [System.Serializable]
    public class GroundMapData
    {
        public int id;
        public string name;
        public string description;
        public List<Cell> cells;
    }
}

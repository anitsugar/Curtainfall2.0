using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObjectGrid : MonoBehaviour
{
    [SerializeField] private GameObject cell;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int worldSizeX = 20;
    [SerializeField] private int worldSizeZ = 20;
    [SerializeField] private int noiseHeight = 3;
    [SerializeField] private float cellOffset = 1.1f;
    [SerializeField] private int spawnCount = 20;
    [SerializeField] private float detailScale = 8f;

    private Transform gridParent;
    private List<GameObject> blocks = new List<GameObject>();

    private void Awake()
    {
        gridParent = new GameObject("GridParent").transform;
        gridParent.parent = transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateObjects();
    }

    private void GenerateObjects()
    {
        RandomGridValues();

        // destruir la grilla anterior
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);
        blocks.Clear();

        // generar nueva grilla
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int z = 0; z < worldSizeZ; z++)
            {
                var pos = new Vector3(x * cellOffset, generateNoise(x, z, detailScale), z * cellOffset);
                GameObject newBlock = Instantiate(cell, pos, Quaternion.identity, gridParent);
                blocks.Add(newBlock);
            }
        }

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        int count = Mathf.Min(spawnCount, blocks.Count);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, blocks.Count);
            GameObject block = blocks[randomIndex];

            float topY = GetTopY(block);
            Vector3 spawnPos = new Vector3(block.transform.position.x, topY, block.transform.position.z);

            // instanciamos y luego alineamos la base del objeto con topY
            GameObject inst = Instantiate(objectToSpawn, spawnPos, Quaternion.identity, gridParent);
            AlignObjectBottomWithY(inst, topY);

            blocks.RemoveAt(randomIndex);
        }
    }

    // obtiene la Y máxima de todos los renderers hijos (robusto ante prefabs complejos)
    private float GetTopY(GameObject go)
    {
        var rs = go.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length == 0)
            return go.transform.position.y;

        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b.max.y;
    }

    // mueve el objeto instanciado para que su punto inferior (bounds.min.y) quede en targetTopY
    private void AlignObjectBottomWithY(GameObject inst, float targetTopY)
    {
        var rs = inst.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length == 0) return;

        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);

        float objBottomY = b.min.y;
        float delta = targetTopY - objBottomY;
        inst.transform.position += Vector3.up * delta;
    }

    private float generateNoise(int x, int z, float detailScale)
    {
        float xNoise = (x + this.transform.position.x) / detailScale;
        float zNoise = (z + this.transform.position.z) / detailScale;
        float noise = Mathf.PerlinNoise(xNoise, zNoise);
        return noise * noiseHeight;
    }

    private void RandomGridValues()
    {
        worldSizeX = Random.Range(5, 70);
        worldSizeZ = Random.Range(5, 70);
        noiseHeight = Random.Range(3, 10);
    }

}

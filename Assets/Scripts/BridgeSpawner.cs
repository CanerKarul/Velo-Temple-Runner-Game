using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject[] bridgePrefabs;

    enum enType
    {
        L_Corner,
        Straight,
        R_Corner,
    }

    enum enDirection
    {
        North,
        East,
        West,
    }

    class Segments
    {
        public GameObject segPrefab;
        public enType segType;

        public Segments(GameObject segPrefab, enType segType)
        {
            this.segPrefab = segPrefab;
            this.segType = segType;
        }
    };

    private List<GameObject> activeSegments = new List<GameObject>();
    private Segments segment;
    private Vector3 spawnCoord = new Vector3(0, 0, -6.0f);
    private enDirection segCurrentDirection = enDirection.North;
    private enDirection segNextDirection = enDirection.North;
    private Transform playerTransform;

    private float segLenth = 6.0f;
    private float segWidth = 3.0f;
    private int segOnScreen = 5;
    private bool stopGame = false;
    
    // Start is called before the first frame update
    void Start()
    {
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeSegments();
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTrigger();
    }

    void InitializeSegments()
    {
        for (int i = 0; i < segOnScreen; i++)
        {
            
            SpawnSegments();
        }
    }

    void CreateSegments()
    {
        switch (segCurrentDirection)
        {
            case enDirection.North:
                segment.segType = (enType)Random.Range(0, 3);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.L_Corner)
                {
                    segment.segPrefab = bridgePrefabs[12];
                }
                else if (segment.segType == enType.R_Corner)
                {
                    segment.segPrefab = bridgePrefabs[11];
                }
                break;
            case enDirection.East:
                segment.segType = (enType)Random.Range(0, 2);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.L_Corner)
                {
                    segment.segPrefab = bridgePrefabs[12];
                }
                break;
            case enDirection.West:
                segment.segType = (enType)Random.Range(1, 3);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.R_Corner)
                {
                    segment.segPrefab = bridgePrefabs[11];
                }
                break;
        }
    }

    void SpawnSegments()
    {
        GameObject prefabToInstantie = segment.segPrefab;
        Quaternion prefabRotation = Quaternion.identity;
        Vector3 offSet = new Vector3(0, 0, 0);

        switch (segCurrentDirection)
        {
            case enDirection.North:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(000,000,000);
                    segNextDirection = enDirection.North;
                    spawnCoord.z += segLenth;
                }
                else if (segment.segType == enType.R_Corner)
                {
                    prefabRotation = Quaternion.Euler(000,000,000);
                    segNextDirection = enDirection.East;
                    spawnCoord.z += segLenth;
                    offSet.z += segLenth + segWidth / 2;
                    offSet.x += segWidth / 2;
                }
                else if (segment.segType == enType.L_Corner)
                {
                    prefabRotation = Quaternion.Euler(000,000,000);
                    segNextDirection = enDirection.West;
                    spawnCoord.z += segLenth;
                    offSet.z += segLenth + segWidth / 2;
                    offSet.x -= segWidth / 2;
                }
                break;
            
            case enDirection.East:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(000,090,000);
                    segNextDirection = enDirection.East;
                    spawnCoord.x += segLenth;
                }
                else if (segment.segType == enType.L_Corner)
                {
                    prefabRotation = Quaternion.Euler(000,090,000);
                    segNextDirection = enDirection.North;
                    spawnCoord.x += segLenth;
                    offSet.z += segWidth / 2;
                    offSet.x += segLenth + segWidth / 2;
                }
                break;
            
            case enDirection.West:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(000,-090,000);
                    segNextDirection = enDirection.West;
                    spawnCoord.x -= segLenth;
                }
                else if (segment.segType == enType.R_Corner)
                {
                    prefabRotation = Quaternion.Euler(000,-090,000);
                    segNextDirection = enDirection.North;
                    spawnCoord.x -= segLenth;
                    offSet.z += segWidth / 2;
                    offSet.x -= segLenth + segWidth / 2;
                }
                break;
        }

        if (prefabToInstantie != null)
        {
            GameObject spawnedPrefab = Instantiate(prefabToInstantie, spawnCoord, prefabRotation) as GameObject;
            activeSegments.Add(spawnedPrefab);
            spawnedPrefab.transform.parent = this.transform;
        }

        segCurrentDirection = segNextDirection;
        spawnCoord += offSet;
    }

    void RemoveSegments()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
        
    }

    void PlayerTrigger()
    {
        if (stopGame)
        {
            return;
        }
        
        GameObject go = activeSegments[0];

        if (Vector3.Distance(playerTransform.position,go.transform.position) > 15.0f)
        {
            CreateSegments();
            SpawnSegments();
            RemoveSegments();
        }
    }

    public void CleanTheScene()
    {
        stopGame = true;
        
        for (int j = activeSegments.Count - 1; j >= 0; j--)
        {
            Destroy(activeSegments[j]);
            activeSegments.RemoveAt(j);
        }

        spawnCoord = new Vector3(0, 0, -6);
        segCurrentDirection = enDirection.North;
        segNextDirection = enDirection.North;
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        InitializeSegments();
        stopGame = false;
    }
    
}

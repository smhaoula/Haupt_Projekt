using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public GameObject tree;
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float squareViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public static float maxViewDst;
    public LODInfo[] detailLevels;
    public Transform viewer;
    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDst;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    public Material mapMaterial;
    void Start(){
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDst = detailLevels[detailLevels.Length-1].visibleDstThreshold;
        chunkSize = MapGenerator.mapChunkSize-1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst/chunkSize);
        UpdateVisibleChunks();
        
    }
    void Update(){
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / mapGenerator.terrainData.uniformScale;
        if((viewerPositionOld-viewerPosition).sqrMagnitude > squareViewerMoveThresholdForChunkUpdate){
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void LateUpdate(){
        foreach(var value in terrainChunkDictionary.Values){
            if(!value.generatedNature){
                value.SpawnTrees();
                value.generatedNature = true;
            }
        }
    }
    void UpdateVisibleChunks(){

        for(int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++){
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

        for(int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++){
            for(int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++){
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)){
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    
                }
                else{
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial, tree));
                }
            }
        }
    }

    public class TerrainChunk{
        public bool generatedNature;
        GameObject _tree;
        TextureData textureData = mapGenerator.textureData;
        GameObject meshObject;
        public Vector2 position;
        public Bounds bounds;

        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        LODInfo[] detailLevels;
        LODMesh[] lODMeshes;
        LODMesh collisionLODMesh;
        bool mapDataReceived;
        int previousLODIndex = -1;
        int terrainSize;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, GameObject tree){
            _tree = tree;
            generatedNature = false;
            this.detailLevels = detailLevels;
            position = coord * size;
            terrainSize = size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshObject.layer = 6;
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
            SetVisible(false);

            lODMeshes = new LODMesh[detailLevels.Length];
            for(int i = 0; i< detailLevels.Length; i++){
                lODMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                if(detailLevels[i].useForCollider){
                    collisionLODMesh = lODMeshes[i];
                }
            }
            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        public void SpawnTrees(){
            float minTreeHeight = textureData.layers[2].startHeight * mapGenerator.noiseData.noiseScale;
            float maxTreeHeight = textureData.layers[4].startHeight * mapGenerator.noiseData.noiseScale;
            int treeCount = 5;
            int layerMask = LayerMask.GetMask("Terrain");
            for(int i = 0; i < treeCount; i++){
                Debug.Log(position);
                float posX = Random.Range(position.x, position.x + terrainSize);
                Debug.Log( posX);
                float posZ = Random.Range(position.y, position.y + terrainSize);
                Debug.Log(posZ);
                RaycastHit hit = new RaycastHit();
                Ray ray = new Ray(new Vector3(posX, 100, posZ), Vector3.down);

                if(Physics.Raycast(ray, out hit, 1000, layerMask)){
                    var distToGround = hit.distance;
                    float posY = 100 - distToGround;
                    Debug.Log(posY);
                    if(posY >= minTreeHeight && posY <= maxTreeHeight){
                        Instantiate(_tree, new Vector3(posX, posY, posZ), Quaternion.Euler(0,0,0));
                        
                    }
                }
                else{Debug.Log("Raycast fehlgeschlagen");}
                float rnd = Random.Range(0, 5);
                    GameObject tree = Instantiate(_tree, new Vector3(posX, rnd, posZ), Quaternion.Euler(0,0,0));
                    
            }
            
        }
        

        void OnMapDataReceived(MapData mapData){
            this.mapData = mapData;
            mapDataReceived = true;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk(){
            if(mapDataReceived){
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDst;
                if(visible){
                    int lodIndex = 0;
                    for(int i = 0; i < detailLevels.Length-1; i++){
                        if(viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold){
                            lodIndex = i+1;
                        }
                        else{
                            break;
                        }
                    }
                    if(lodIndex != previousLODIndex){
                        LODMesh lODMesh = lODMeshes[lodIndex];
                        if(lODMesh.hasMesh){
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lODMesh.mesh;
                            
                        }
                        else if(!lODMesh.hasRequestedMesh){
                            lODMesh.RequestMesh(mapData);
                        }
                    }

                    if(lodIndex==0){
                        if(collisionLODMesh.hasMesh){
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                        }
                        else if(!collisionLODMesh.hasRequestedMesh){
                            collisionLODMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible){
            meshObject.SetActive(visible);
        }

        public bool IsVisible(){
            return meshObject.activeSelf;
        }
    }

    class LODMesh{
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback){
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData){
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData){
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo{
        public int lod;
        public float visibleDstThreshold;
        public bool useForCollider;
    }
}

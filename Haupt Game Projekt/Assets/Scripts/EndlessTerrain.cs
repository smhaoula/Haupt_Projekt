using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndlessTerrain : MonoBehaviour
{
    [SerializeField]
    private Transform mapgenarator;
    public GameObject village;
    public GameObject nature;
    public GameObject tree;
    public GameObject grass;
    public GameObject rock;
    public GameObject pine;
    public GameObject mushroom;
    public GameObject enemy;
    public GameObject crystal;
    public GameObject[] npc;
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
    private NavMeshSurface[] Surfaces;
    private void Awake()
    {
        
            if (mapgenarator == null || mapgenarator.GetComponent<NavMeshSurface>() == null)
            {
                Debug.LogError("mapgenerator Manager must be assigned and must have at least 1 NavMeshSurface!");
                gameObject.SetActive(false);
                return;
            }
        Surfaces = mapgenarator.GetComponentsInChildren<NavMeshSurface>();
    }
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
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial, tree, grass, nature, village, pine, rock, mushroom, enemy, crystal, npc));
                    
                }
            }

        }
        Surfaces = mapgenarator.GetComponentsInChildren<NavMeshSurface>();
       
        for (int i = 0; i < Surfaces.Length; i++)
        {
            if(Surfaces[i].navMeshData == null)
            {
                BuildNavMeshAsync(Surfaces[i]);
            }         
        }
    }
    public static AsyncOperation BuildNavMeshAsync(NavMeshSurface surface)
    {
        surface.RemoveData();
        surface.navMeshData = new NavMeshData(surface.agentTypeID)
        {
            name = surface.gameObject.name,
            position = surface.transform.position,
            rotation = surface.transform.rotation
        };

        if (surface.isActiveAndEnabled)
            surface.AddData();

        return surface.UpdateNavMesh(surface.navMeshData);
    }

  

    public class TerrainChunk{
        CoroutineHandler cHandler; 
        public List<GameObject> spawnedObjects;
        GameObject _village;
        GameObject _nature;
        GameObject _grass;
        GameObject _pine;
        GameObject _rock;
        GameObject _enemy;
        GameObject _mushroom;
        GameObject _crystal;
        GameObject[] _npc;
        public bool generatedNature;
        public bool generatedNavmesh;
        GameObject _tree;
        TextureData textureData = mapGenerator.textureData;
        GameObject meshObject;
        public Vector2 position;
        public Vector2 startOfChunkPosition;
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

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, GameObject tree, GameObject grass, GameObject nature, GameObject village, GameObject pine, GameObject rock, GameObject mushroom, GameObject enemy, GameObject crystal, GameObject[] npc){
            spawnedObjects = new List<GameObject>();
            cHandler = FindObjectOfType<CoroutineHandler>();
            _village = village;
            _nature = nature;
            _tree = tree;
            _grass = grass;
            _pine = pine;
            _rock = rock;
            _enemy = enemy;
            _mushroom = mushroom;
            _crystal = crystal;
            _npc = npc;
            generatedNature = false;
            generatedNavmesh = false;
            this.detailLevels = detailLevels;
            position = coord * size;
            
            terrainSize = Mathf.RoundToInt(size*mapGenerator.terrainData.uniformScale);
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            startOfChunkPosition.x = bounds.center.x - size/2;
            startOfChunkPosition.y = bounds.center.z - size/2;
            startOfChunkPosition = startOfChunkPosition* mapGenerator.terrainData.uniformScale;
            
            Debug.DrawRay(new Vector3(startOfChunkPosition.x, 100, startOfChunkPosition.y), Vector3.down*200, Color.red, 20f);
            //Debug.Log("Ray wurde gezeichnet");
            //Debug.Log(startOfChunkPosition);
            //Debug.Log(coord);


            meshObject = new GameObject("Terrain Chunk");
            meshObject.layer = 6;
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshObject.AddComponent<NavMeshSurface>();
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
            float minTreeHeight = textureData.layers[1].startHeight * mapGenerator.noiseData.noiseScale;
            float maxTreeHeight = textureData.layers[4].startHeight * mapGenerator.noiseData.noiseScale;
            int treeCount = 35;
            int layerMask = LayerMask.GetMask("Terrain");
            for(int i = 0; i < treeCount; i++){
                float posX = Random.Range(startOfChunkPosition.x, startOfChunkPosition.x + terrainSize);
                float posZ = Random.Range(startOfChunkPosition.y, startOfChunkPosition.y + terrainSize);
               
                RaycastHit hit = new RaycastHit();
                var p = new Vector3(posX, 100, posZ);
                Ray ray = new Ray(p, Vector3.down*200);
                //Debug.DrawRay(p, Vector3.down*200, Color.red, 20f);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                    float posY = hit.point.y;
                    if(posY >= minTreeHeight && posY <= maxTreeHeight){
                        Vector3 spawnPos = new Vector3(posX, posY, posZ);

                         Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 10f);
                        foreach (var hitCollider in hitColliders)
                        {
                            if(hitCollider.gameObject.tag=="Architecture"){
                                Debug.Log("Village zu nah");
                                break;
                            }
                            else{
                                //Debug.Log("Baum platziert");
                                GameObject t = Instantiate(_tree, spawnPos, Quaternion.Euler(0,Random.Range(0,360),0));
                                spawnedObjects.Add(t);
                                t.transform.parent = _nature.transform;

                                for(int y = 0; y < 5; y++){
                                    float offset1 = Random.Range(-5,5);
                                    float offset2 = Random.Range(-5, 5);

                                    RaycastHit hitGrass = new RaycastHit();
                                    var gr = new Vector3(spawnPos.x + offset1, 100, spawnPos.z + offset2);
                                    Ray grassRay = new Ray(gr, Vector3.down*200);
                                    if(Physics.Raycast(grassRay, out hitGrass, Mathf.Infinity)){
                                        float posGrassY = hit.point.y;
                                        if(posGrassY >= minTreeHeight && posGrassY <= maxTreeHeight){
                                            Vector3 grassSpawnPos = new Vector3(spawnPos.x + offset1, posGrassY, spawnPos.z + offset2);
                                            GameObject g = Instantiate(_grass, grassSpawnPos, Quaternion.Euler(0,Random.Range(0,360),0));
                                            spawnedObjects.Add(g);
                                            g.transform.parent = _nature.transform;
                                        }
                                    }
                                }
                            }
                        }

                        
                    }
                }
                else
                {
                    //Debug.Log(startOfChunkPosition);
                    //Debug.Log(spawnedObjects.Count);
                    //Debug.Log("Raycast fehlgeschlagen");
                }
            }
        }

        public void SpawnGrass(){
            float minGrassHeight = textureData.layers[1].startHeight * mapGenerator.noiseData.noiseScale;
            float maxGrassHeight = textureData.layers[3].startHeight * mapGenerator.noiseData.noiseScale;
            int grassCount = 70;
            int layerMask = LayerMask.GetMask("Terrain");

            for(int i = 0; i < grassCount; i++){
                float posX = Random.Range(startOfChunkPosition.x, startOfChunkPosition.x + terrainSize);
                float posZ = Random.Range(startOfChunkPosition.y, startOfChunkPosition.y + terrainSize);
               
                RaycastHit hit = new RaycastHit();
                var p = new Vector3(posX, 100, posZ);
                Ray ray = new Ray(p, Vector3.down*200);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                    float posY = hit.point.y;
                    if(posY >= minGrassHeight && posY <= maxGrassHeight){
                        Vector3 spawnPos = new Vector3(posX, posY, posZ);
                        GameObject t = Instantiate(_grass, spawnPos, Quaternion.Euler(0,Random.Range(0,360),0));
                        spawnedObjects.Add(t);
                        t.transform.parent = _nature.transform;
                    }
                }
            }
        }

        public void SpawnNature(GameObject prefab, int count, int minHeight, int maxHeight){
            float minSpawnHeight = textureData.layers[minHeight].startHeight * mapGenerator.noiseData.noiseScale;
            float maxSpawnHeight = textureData.layers[maxHeight].startHeight * mapGenerator.noiseData.noiseScale;
            int spawnCount = count;
            int layerMask = LayerMask.GetMask("Terrain");

            for(int i = 0; i < spawnCount; i++){
                float posX = Random.Range(startOfChunkPosition.x, startOfChunkPosition.x + terrainSize);
                float posZ = Random.Range(startOfChunkPosition.y, startOfChunkPosition.y + terrainSize);
               
                RaycastHit hit = new RaycastHit();
                var p = new Vector3(posX, 100, posZ);
                Ray ray = new Ray(p, Vector3.down*200);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                    float posY = hit.point.y;
                    if(posY >= minSpawnHeight && posY <= maxSpawnHeight){
                        Vector3 spawnPos = new Vector3(posX, posY, posZ);
                        GameObject t = Instantiate(prefab, spawnPos, Quaternion.Euler(0,Random.Range(0,360),0));
                        spawnedObjects.Add(t);
                        t.transform.parent = _nature.transform;
                    }
                }
            }
        }

         public void SpawnNavMeshAgent(GameObject prefab, int count, int minHeight, int maxHeight){
            float minSpawnHeight = textureData.layers[minHeight].startHeight * mapGenerator.noiseData.noiseScale;
            float maxSpawnHeight = textureData.layers[maxHeight].startHeight * mapGenerator.noiseData.noiseScale;
            int spawnCount = count;
            int layerMask = LayerMask.GetMask("Terrain");

            for(int i = 0; i < spawnCount; i++){
                float posX = Random.Range(startOfChunkPosition.x, startOfChunkPosition.x + terrainSize);
                float posZ = Random.Range(startOfChunkPosition.y, startOfChunkPosition.y + terrainSize);
               
                RaycastHit hit = new RaycastHit();
                var p = new Vector3(posX, 100, posZ);
                Ray ray = new Ray(p, Vector3.down*200);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                    float posY = hit.point.y;
                    if(posY >= minSpawnHeight && posY <= maxSpawnHeight){
                        Vector3 spawnPos = new Vector3(posX, posY, posZ);
                        NavMeshHit closestHit;
                        if(NavMesh.SamplePosition(spawnPos, out closestHit, 500, 1)){
                            spawnPos = closestHit.position;
                            GameObject t = Instantiate(prefab, spawnPos, Quaternion.Euler(0,Random.Range(0,360),0));
                            t.AddComponent<NavMeshAgent>();
                            t.GetComponent<NavMeshAgent>().enabled = true;
                            spawnedObjects.Add(t);
                            t.transform.parent = _nature.transform;
                        }
                        else{
                            Debug.Log("NavMesh Hit nicht funktioniert");
                        }
                        
                    }
                }
            }
        }

        public void SpawnNPC()
        {
            int length = _npc.Length-1;
            int select = Random.Range(0,length);
            GameObject selNpc = _npc[select];
            SpawnNature(selNpc, 2, 2, 4);
        }

        public bool SpawnVillage(){
            float minVillageHeight = textureData.layers[2].startHeight * mapGenerator.noiseData.noiseScale;
            float maxVillageHeight = textureData.layers[3].startHeight * mapGenerator.noiseData.noiseScale;
            int villageCount = 1;

            for(int i = 0; i < villageCount; i++){
                float posX = Random.Range(startOfChunkPosition.x, startOfChunkPosition.x + terrainSize);
                float posZ = Random.Range(startOfChunkPosition.y, startOfChunkPosition.y + terrainSize);
               
                RaycastHit hit = new RaycastHit();
                var p = new Vector3(posX, 100, posZ);
                Ray ray = new Ray(p, Vector3.down*200);
                //Debug.DrawRay(p, Vector3.down*200, Color.red, 20f);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
                    float posY = hit.point.y;
                    //Debug.Log(posY);
                    if(posY >= minVillageHeight && posY <= maxVillageHeight){
                        Vector3 spawnPos = new Vector3(posX, posY, posZ);
                        GameObject t = Instantiate(_village, spawnPos, Quaternion.Euler(0,Random.Range(0,360),0));
                        spawnedObjects.Add(t);
                        t.transform.parent = _nature.transform;
                    }
                }
                else{
                    return false;
                }
            }
            return true;
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

                            /*if(spawnedObjects.Count == 0){
                                if(SpawnVillage()){
                                    generatedNature = true;
                                }
                                SpawnTrees();
                                SpawnGrass();
                                SpawnNature(_pine, 20, 4,5);
                                SpawnNature(_rock, 10, 4,5);
                            }*/
                            
                        }
                        else if(!lODMesh.hasRequestedMesh){
                            lODMesh.RequestMesh(mapData);
                        }
                    }

                    if(lodIndex==0){
                        if(collisionLODMesh.hasMesh){
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                            
                            //Debug.Log(startOfChunkPosition);
                            //Debug.Log(spawnedObjects.Count);

                            /*if(spawnedObjects.Count == 0){
                                if(SpawnVillage()){
                                    generatedNature = true;
                                }
                                SpawnTrees();
                                SpawnGrass();
                                SpawnNature(_pine, 20, 4,5);
                                SpawnNature(_rock, 10, 4,5);
                            }*/
                        
                        
                        }
                        else if(!collisionLODMesh.hasRequestedMesh){
                            collisionLODMesh.RequestMesh(mapData);
                        }
                    }

                   
                    terrainChunksVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
                if(meshCollider.sharedMesh != null && this.spawnedObjects.Count ==0){
                    if(spawnedObjects.Count == 0){
                                //SpawnVillage();
                                //SpawnTrees();
                                SpawnNature(_village, 3, 2,3);
                                SpawnNature(_tree, 40, 1, 4);
                                SpawnNature(_grass, 70, 1,3);
                                SpawnNature(_pine, 40, 4,5);
                                SpawnNature(_rock, 30, 4,5);
                                SpawnNature(_crystal, 10, 1, 4);
                               cHandler.StartChildCoroutine(WaitForNavmesh());
                            }
                }
            }
        }
        IEnumerator WaitForNavmesh()
        {
            yield return new WaitForSeconds(3f);
            //Transform player = GameObject.FindWithTag("Player").transform;
            //Debug.Log(player);
            SpawnNature(_enemy, 5, 1,4);
            //SpawnNavMeshAgent(_enemy, 5, 1, 4);
        }

        public void SetVisible(bool visible){
            meshObject.SetActive(visible);
            foreach (GameObject item in spawnedObjects)
            {
                if(item != null)
                {
                    item.SetActive(visible);
                }
                
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject WorldMap; //전체 맵

    //나무, 잔디, 꽃, 바위, 우물 ,구름, 부쉬 
    [SerializeField]
    int _treeMaxCount;
    [SerializeField]
    int _grassMaxCount;
    [SerializeField]
    int _flowerMaxCount;
    [SerializeField]
    int _rockMaxCount;
    [SerializeField]
    int _wellMaxCount;
    [SerializeField]
    int _bushMaxCount;
    [SerializeField]
    int _cloudMaxCount;

    [SerializeField]
    GameObject[] _generateFieldPos; //필드에 오브젝트들 생성 위치
    [SerializeField]
    GameObject[] _generateSkyPos; //하늘에 오브젝트들 생성 위치

    [SerializeField]
    float _spawnFieldRadius; //필드 생성 범위
    [SerializeField]
    float _spawnSkyRadius; //하늘 생성 범위 

    RaycastHit _hit;
    int _layerMask; //필드레이어

    GameObject _obj;  //이거 게임오브젝트 변수의 위치 어디가 좋을까...

    int GetRandTreeCount() { return Random.Range(_treeMaxCount / 2, _treeMaxCount); } // 나무 수 
    int GetRandGrassCount() { return Random.Range(_grassMaxCount / 2, _grassMaxCount); } // 잔디 
    int GetRandFlowerCount() { return Random.Range(_flowerMaxCount / 2, _flowerMaxCount); } // 꽃  
    int GetRandRockCount() { return Random.Range(0, _rockMaxCount); } // 바위 0~4
    int GetRandWellCount() { return Random.Range(0, _wellMaxCount); } // 우물.. 
    //int GetRandBushCount() { return Random.Range(_bushMaxCount - 4, _bushMaxCount); } //부쉬 
    int GetRandCloudCount() { return Random.Range(_cloudMaxCount / 2, _cloudMaxCount); } // 구름

    void Start()
    {
        if (WorldMap == null)
        {
            WorldMap = GameObject.Find("WorldMap");
            if (WorldMap == null)
                WorldMap = Managers.Resource.Instantiate("Map/WorldMap");
        }

        _layerMask = 1 << 6; //6 = LayerMask.NameToLayer("Field")
        Generate();
    }

    void Generate() //월드 맵 오브젝트 생성
    {
        GameObject Trees = Util.FindChind(WorldMap, "Trees", true);
        GameObject Grasses = Util.FindChind(WorldMap, "Grasses", true);
        GameObject Flowers = Util.FindChind(WorldMap, "Flowers", true);
        GameObject Rocks = Util.FindChind(WorldMap, "Rocks", true);
        GameObject Clouds = Util.FindChind(WorldMap, "Clouds", true);

        for (int p = 0; p < _generateFieldPos.Length; p++) //필드
        {

            for (int i = 0; i < GetRandTreeCount(); i++) //나무
            {
                int treeNum = Random.Range(0, 6); // Tree Type
                //float treeScale = Random.Range(0, 0.01f); //Tree Scale
                _obj = Managers.Game.Spawn(Define.WorldObject.Tree, $"Map/Tree{treeNum}", Trees.transform);
                _obj.transform.position = RandPosOrNormal(p).randPos;
                if( treeNum ==2 || treeNum == 4)
                    _obj.transform.position -= new Vector3(0, 10f, 0);
                //_obj.transform.localScale += new Vector3(0, 0, -treeScale); //나무 랜덤으로 높이 조절
            }
            for (int i = 0; i < GetRandGrassCount(); i++) //잔디
            {
                (Vector3, Vector3) RandPosOrNor;
                RandPosOrNor = RandPosOrNormal(p);
                _obj = Managers.Game.Spawn(Define.WorldObject.Tree, $"Map/Grass0", Grasses.transform);
                _obj.transform.position = RandPosOrNor.Item1;
                _obj.transform.position += new Vector3(0, -1.0f, 0); //y축으로 -1
                _obj.transform.up = RandPosOrNor.Item2; //기울기

            }
            for (int i = 0; i < GetRandFlowerCount(); i++) //꽃
            {
                int flowerNum = Random.Range(0, 5);
                _obj = Managers.Game.Spawn(Define.WorldObject.Tree, $"Map/Flower{flowerNum}", Flowers.transform);
                _obj.transform.position = RandPosOrNormal(p).randPos;
                _obj.transform.rotation = Quaternion.Euler(new Vector3(0, Random.value * 360.0f, 0));
            }
            for (int i = 0; i < GetRandRockCount(); i++) //바위
            {
                _obj = Managers.Game.Spawn(Define.WorldObject.Tree, $"Map/Rocks0", Rocks.transform);
                _obj.transform.position = RandPosOrNormal(p).randPos;
            }
        }

        for (int p = 0; p < _generateSkyPos.Length; p++) //하늘
        {

            for (int i = 0; i < GetRandCloudCount(); i++) //구름
            {
                //int cloudNum = Random.Range(0, 4); // Cloud Type , 일단 애니메이션만 있는 구름만 사용..
                Vector3 randPosition;
                Vector3 randDir = Random.insideUnitSphere * _spawnSkyRadius;
                if (randDir.y < 0) randDir.y = 0; //구체에서 y=0 이상만
                randPosition = _generateSkyPos[p].transform.position + randDir; // 포지션

                _obj = Managers.Game.Spawn(Define.WorldObject.Tree, $"Map/Cloud0", Clouds.transform);
                _obj.transform.position = randPosition;
                _obj.transform.rotation = Random.rotation; //임의의 회전
                _obj.transform.localScale += new Vector3(Random.value * 3.0f, Random.value * 10.0f, Random.value * 10.0f); //임의의 스케일
            }
        }

        //to do 우물
    }

    (Vector3 randPos, Vector3 normal) RandPosOrNormal(int num) //오브젝트의 랜덤 좌표와 법선벡터 반환문
    {
        Vector3 randPosition;
        Vector3 normal;
        Vector3 raycastOrigin;
        Vector3 dir;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * _spawnFieldRadius; // 미리 지정해준 생성 범위(_spawnRadius)내 좌표
            randDir.y = 0;
            randPosition = _generateFieldPos[num].transform.position + randDir;
            raycastOrigin = randPosition;
            raycastOrigin.y = 500.0f;
            dir = randPosition - raycastOrigin;
            if (Physics.Raycast(raycastOrigin, dir.normalized, out _hit, Mathf.Infinity, _layerMask)) //필드 안에 있는 좌표라면
                break;
        }
        normal = FindHeightField(randPosition).normal;
        randPosition.y = FindHeightField(randPosition).height;

        return (randPosition, normal);
    }


    (float height, Vector3 normal) FindHeightField(Vector3 target)   //위에서 아래로 레이케스트를 쏜 다음 오브젝트가 생성 될 정확한 높이를 구함, 이 때 법선벡터도 구함
    {                                                               //(지형의 높이 , 지형의 법선벡터)
        Vector3 raycastOrigin = target;
        raycastOrigin.y = 500.0f;
        var dir = target - raycastOrigin;

        if (Physics.Raycast(raycastOrigin, dir.normalized, out _hit, Mathf.Infinity, _layerMask))
        {
            return (_hit.point.y, _hit.normal);
        }
        return (0, Vector3.up);
    }

    //생성 위치와 범위 보기
    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < _generateFieldPos.Length; i++)
    //    {
    //        Gizmos.color = Color.gray;
    //        Gizmos.DrawWireSphere(_generateFieldPos[i].transform.position, _spawnFieldRadius);
    //    }

    //    for (int i = 0; i < _generateSkyPos.Length; i++)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(_generateSkyPos[i].transform.position, _spawnSkyRadius);
    //    }
    //}
}

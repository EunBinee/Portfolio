using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class ObjectPooling
{
    // * 이펙트 풀링------------------------------------------------------------------------------------//
    public int PoolCount = 50;
    public Dictionary<string, Effect> effectPrefabs;
    public Dictionary<string, List<Effect>> effectPools;
    public List<Effect> loofEffectPools;
    // * -----------------------------------------------------------------------------------------------//

    public int projectilesPoolCount = 50;
    public Dictionary<string, GameObject> projectilePrefabs;
    public Dictionary<string, List<GameObject>> projectilePools;



    public void InitPooling()
    {
        //오브젝트 풀링.
        //*-----------------------------------------------------//
        effectPrefabs = new Dictionary<string, Effect>();
        effectPools = new Dictionary<string, List<Effect>>();
        loofEffectPools = new List<Effect>();

        effectPrefabs.Clear();
        effectPools.Clear();
        loofEffectPools.Clear();
        //*------------------------------------------------------//
        projectilePrefabs = new Dictionary<string, GameObject>();
        projectilePools = new Dictionary<string, List<GameObject>>();
        projectilePrefabs.Clear();
        projectilePools.Clear();
        //*-----------------------------------------------------//
    }

    // * --------------------------------------------------------------------------------------------------------//
    //* 이펙트
    public Effect ShowEffect(string effectName, Transform parent = null) //effectName은 경로의 역할도 함
    {
        Effect curEffect = null;

        //프리펩 찾기
        if (effectPrefabs.ContainsKey(effectName))
        {
            //Debug.Log(effectPrefabs[effectName]);
            curEffect = effectPrefabs[effectName];
        }
        else
        {
            curEffect = Resources.Load<Effect>("EffectPrefabs/" + effectName);
            if (curEffect != null)
            {
                effectPrefabs.Add(effectName, curEffect);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("ObjectPooling 프리펩 없음. 오류.");
#endif
            }
        }

        if (curEffect != null)
        {
            //오브젝트 풀에 
            if (effectPools.ContainsKey(effectName))
            {
                if (effectPools[effectName].Count > 0)
                {
                    curEffect = effectPools[effectName][0];
                    effectPools[effectName].Remove(curEffect);
                }
                else
                {
                    curEffect = UnityEngine.Object.Instantiate(curEffect);
                }
            }
            else
            {
                effectPools.Add(effectName, new List<Effect>());
                curEffect = UnityEngine.Object.Instantiate(curEffect);
            }

            Transform effecParent = (parent == null) ? GameManager.Instance.transform : parent;
            curEffect.gameObject.transform.SetParent(effecParent);
            curEffect.ShowEffect();
            curEffect.callBack = () =>
            {
                AddEffectPool(effectName, curEffect);
            };
        }

        return curEffect;
    }

    public void AddEffectPool(string effectName, Effect effect)
    {
        if (effectPools[effectName].Count >= PoolCount)
        {
            //만약 풀이 가득 찼다면, 그냥 삭제.
            UnityEngine.Object.Destroy(effect.gameObject);

        }
        else
        {
            effect.gameObject.transform.SetParent(GameManager.Instance.transform);
            if (!effectPools[effectName].Contains(effect))
            {
                effectPools[effectName].Add(effect);
            }

        }
    }
    // * --------------------------------------------------------------------------------------------------------//
    //* 몬스터 발사체 ex.총알
    public GameObject GetProjectilePrefab(string projectileName, Transform parent = null)
    {
        GameObject curProjectileObj = null;
        //프리펩 찾기
        if (projectilePrefabs.ContainsKey(projectileName))
        {
            curProjectileObj = projectilePrefabs[projectileName];
        }
        else
        {
            curProjectileObj = Resources.Load<GameObject>("ProjectilePrefabs/" + projectileName);
            if (curProjectileObj != null)
            {
                //프리펩 추가
                projectilePrefabs.Add(projectileName, curProjectileObj);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Projectile 프리펩 없음. 오류.");
#endif
            }
        }


        if (curProjectileObj != null)
        {
            //오브젝트 풀에 
            if (projectilePools.ContainsKey(projectileName))
            {
                if (projectilePools[projectileName].Count > 0)
                {
                    curProjectileObj = projectilePools[projectileName][0];
                    projectilePools[projectileName].RemoveAt(0);
                }
                else
                {
                    curProjectileObj = UnityEngine.Object.Instantiate(curProjectileObj);
                }
            }
            else
            {
                projectilePools.Add(projectileName, new List<GameObject>());
                curProjectileObj = UnityEngine.Object.Instantiate(curProjectileObj);
            }

            Transform objParent = (parent == null) ? GameManager.Instance.transform : parent;
            curProjectileObj.gameObject.transform.SetParent(objParent);
        }

        return curProjectileObj;
    }

    public void AddProjectilePool(string projectileName, GameObject projectileObj)
    {
        if (projectilePools[projectileName].Count >= projectilesPoolCount)
        {
            //만약 풀이 가득 찼다면, 그냥 삭제.
            UnityEngine.Object.Destroy(projectileObj);
        }
        else
        {
            projectileObj.transform.SetParent(GameManager.Instance.transform);
            projectilePools[projectileName].Add(projectileObj);
            projectileObj.SetActive(false);
        }
    }
}

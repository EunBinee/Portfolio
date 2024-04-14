using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    //이펙트 실행
    //이펙트 끄기
    //OnEnable() //풀링할거라서 
    private ParticleSystem mEffect;
    List<ParticleSystem> mEffect_ParticleList;
    public Action callBack = null;
    public Action finishAction; //이펙트가 끝났을때 필요한 것
    bool particleNull = false;

    bool isAlive = false;
    bool isStop = false;

    public void ShowEffect()
    {
        isStop = false;
        mEffect = GetComponent<ParticleSystem>();
        if (mEffect == null)
        {
            particleNull = true;
            Transform[] children = GetComponentsInChildren<Transform>(true);
            mEffect_ParticleList = new List<ParticleSystem>();

            foreach (Transform child in children)
            {
                ParticleSystem childParticle;
                childParticle = child.gameObject.GetComponent<ParticleSystem>();

                if (childParticle != null && child.gameObject.activeSelf == true)
                {
                    mEffect_ParticleList.Add(childParticle);
                }
            }
        }
        else
            particleNull = false;

        if (particleNull == false)
        {
            mEffect.gameObject.SetActive(true);
            mEffect.Play();
        }
        else
        {
            if (mEffect_ParticleList.Count > 0)
            {
                this.gameObject.SetActive(true);
                //mEffect.gameObject.SetActive(true);
                foreach (ParticleSystem particle in mEffect_ParticleList)
                {
                    particle.Play();
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("이펙트에 파티클 시스템 없음.");
#endif
            }
        }

    }

    public void StopEffect()
    {
        if (particleNull == false)
            mEffect.Stop();
        else
        {
            if (mEffect_ParticleList.Count > 0)
            {
                foreach (ParticleSystem particle in mEffect_ParticleList)
                {
                    particle.Stop();
                }
            }
        }
    }

    private void Update()
    {
        if (particleNull == true && !isStop)
        {
            foreach (ParticleSystem particle in mEffect_ParticleList)
            {
                if (!particle.IsAlive())
                    isAlive = false;
                if (particle.IsAlive())
                {
                    isAlive = true;
                    break;
                }
            }

            if (!isAlive)
            {
                finishAction?.Invoke();
                finishAction = null;
                callBack?.Invoke();
                isStop = true;

                this.gameObject.SetActive(false);
                mEffect_ParticleList.Clear();
            }
        }
        else if (particleNull == false && !isStop)
        {
            if (!mEffect.IsAlive())
            {
                finishAction?.Invoke();
                finishAction = null;
                callBack?.Invoke();
                isStop = true;
                mEffect.gameObject.SetActive(false);
            }
        }

    }
}
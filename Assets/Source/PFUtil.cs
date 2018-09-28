using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class PFUtil
{
    public static void DestroyGameObject<T>(List<T> target) where T : MonoBehaviour
    {
        for (int i = 0; i < target.Count; ++i)
        {
            GameObject.Destroy(target[i].gameObject);
            //GameObject.DestroyImmediate(target[i].gameObject);
        }

        target.Clear();
    }

    public static void DestroyChild(GameObject gameObject, bool bImmediate)
    {
        if (gameObject != null && 
            gameObject.transform != null)
        {
            //자식들 지움(순회하면서 곧바로 원소를 지우기 때문에 뒤에서부터 loop를 돌린다)
            for (int i = gameObject.transform.childCount - 1; i >= 0; --i)
            {
                Transform child = gameObject.transform.GetChild(i);
                if (child != null)
                {
                    if (bImmediate)
                        GameObject.DestroyImmediate(child.gameObject);
                    else
                        GameObject.Destroy(child.gameObject);
                }
            }
        }
    }

    public static T Instantiate<T>(string prefabPath, GameObject parent, Vector3 pos) where T : MonoBehaviour
    {
        T original = Resources.Load<T>(prefabPath);

        if (original != null)
        {
            T newInstance = PFUtil.Instantiate(original, parent);

            newInstance.gameObject.transform.localPosition = pos;

            return newInstance;
        }

        return null;
    }

    public static List<T> GetRandomList<T>(List<T> targetList, int takeNum, bool bUnique)
    {
        List<int> randomIdxList = GetRandomIdxList(targetList, takeNum, bUnique);

        if (randomIdxList != null)
        {
            List<T> resultList = new List<T>();

            for (int i = 0; i < randomIdxList.Count; ++i)
            {
                int idx = randomIdxList[i];
                T val = targetList[idx];
                resultList.Add(val);
            }

            return resultList;
        }

        return null;
    }

    public static List<int> GetRandomIdxList<T>(List<T> totalList, int takeNum, bool bUnique)
    {
        if (totalList != null)
        {
            if (bUnique && (takeNum > totalList.Count))
            {
                StringBuilder sbLog = new StringBuilder("");
                sbLog.AppendFormat("bUnique인데, 랜덤으로 취하는 개수가 파라미터List.Count를 넘어섭니다");
                Debug.LogWarning(sbLog.ToString());

                takeNum = totalList.Count;
            }

            List<int> randomIdxList = new List<int>();

            for (int i = 0; i < takeNum; ++i)
            {
                if (bUnique)
                {
                    while (true)
                    {
                        int randIdx = UnityEngine.Random.Range(0, totalList.Count);

                        bool bExist = randomIdxList.Exists(comp =>
                        {
                            bool bResult = (comp == randIdx);
                            return bResult;
                        });

                        if (!bExist)
                        {
                            randomIdxList.Add(randIdx);
                            break;
                        }
                    }
                }
                else
                {
                    int randIdx = UnityEngine.Random.Range(0, totalList.Count);
                    randomIdxList.Add(randIdx);
                }
            }

            return randomIdxList;
        }

        return null;
    }

    public static void SetDefaultAttach(GameObject parent, GameObject child)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
    }

    public static T CreateGameObject<T>(GameObject parentObj) where T : MonoBehaviour
    {
        Type typeOfT = typeof(T);

        //1
        GameObject newGameObj = new GameObject(typeOfT.FullName);

        //2
        PFUtil.SetDefaultAttach(parentObj, newGameObj);

        //3
        T tObj = newGameObj.AddComponent<T>();

        return tObj;
    }

    public static T Instantiate<T>(T originalObj, GameObject parent) where T : MonoBehaviour
    {
        if (originalObj != null &&
            parent != null)
        {
            //1
            T newInstance = GameObject.Instantiate<T>(originalObj);

            //2
            Type typeOfT = typeof(T);
            newInstance.name = typeOfT.FullName;

            //3
            PFUtil.SetDefaultAttach(parent, newInstance.gameObject);

            return newInstance;
        }

        return null;
    }

    public static T Instantiate<T>(T originalObj, GameObject parent, Vector3 pos) where T : MonoBehaviour
    {
        T newInstance = PFUtil.Instantiate(originalObj, parent);

        if (newInstance != null)
        {
            newInstance.gameObject.transform.localPosition = pos;
        }

        return newInstance;
    }
}

public static class EnumUtil<T>
{
    public static T Parse(string s)
    {
        return (T)Enum.Parse(typeof(T), s);
    }
}

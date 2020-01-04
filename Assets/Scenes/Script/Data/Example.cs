using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    [System.Serializable]
    public class Data
    {
        public int id;
        public string name;
        public GameObject gameobject;
        public List<int> nested;
    }

    [SerializeField]
    private List<Data> m_datas = new List<Data>();

    public Vector3[] vectorArray;
}
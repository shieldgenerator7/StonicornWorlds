using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiGroupedList<T, K>
{
    Dictionary<T, List<K>> list = new Dictionary<T, List<K>>();

    public MultiGroupedList(GroupFunction groupFunction)
    {
        this.groupFunction = groupFunction;
    }

    public delegate List<T> GroupFunction(K val);
    private GroupFunction groupFunction;

    public void Add(K val)
    {
        getLists(val)
            .ForEach(list => list.Add(val));
    }

    public void AddAll(List<K> vals)
    {
        vals.ForEach(val => Add(val));
    }

    public void Update(K val)
    {
        list.Values.ToList().ForEach(l => l.Remove(val));
        Add(val);
    }

    public void Remove(K val)
    {
        getLists(val)
            .ForEach(list => list.Remove(val));
    }
    private List<List<K>> getLists(K val)
    {
        List<T> keys = groupFunction(val);
        return GetLists(keys);
    }

    public List<K> GetList(T key)
    {
        if (!list.ContainsKey(key))
        {
            list.Add(key, new List<K>());
        }
        return list[key];
    }

    public List<List<K>> GetLists(List<T> keys)
    {
        return keys.ConvertAll(key => GetList(key));
    }

    void print()
    {
        string output = "MultiGroupList counts: ";
        list.Keys.ToList().ForEach(
            key => output += "" + key + ": " + list[key].Count + ", "
            );
        Debug.Log(output);
    }
}

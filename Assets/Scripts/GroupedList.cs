using System.Collections;
using System.Collections.Generic;

public class GroupedList<T, K>
{
    Dictionary<T, List<K>> list = new Dictionary<T, List<K>>();

    public GroupedList(GroupFunction groupFunction)
    {
        this.groupFunction = groupFunction;
    }

    public delegate T GroupFunction(K val);
    private GroupFunction groupFunction;

    public void add(K val)
    {
        getList(val).Add(val);
    }
    public void remove(K val)
    {
        getList(val).Remove(val);
    }
    private List<K> getList(K val)
    {
        T key = groupFunction(val);
        return getList(key);
    }

    public List<K> getList(T key)
    {
        if (!list.ContainsKey(key))
        {
            list.Add(key, new List<K>());
        }
        return list[key];
    }
}

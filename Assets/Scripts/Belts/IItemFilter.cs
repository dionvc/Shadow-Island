using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemFilter
{
    string GetFilter(); //returns current filter name
    int GetFilterItem(); //returns item being filtered
    void SetFilter(string filterName, int itemId);
    /// <summary>
    /// Returns name as well as count of filters in terms of count
    /// </summary>
    /// <returns></returns>
    string[] GetFilterNames();
}

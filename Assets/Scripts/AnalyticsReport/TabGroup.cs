/* Reference
https://github.com/IkeThermite/GameDevGuide-CustomTabsAndFlexibleGrid
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    // These must be 1 to 1, same order in hierarchy
    [HideInInspector]
    public List<Tab> tabButtonList = new List<Tab>();

    //In case I need to sort the lists by GetSiblingIndex
    //objListOrder.Sort((x, y) => x.OrderDate.CompareTo(y.OrderDate));

    [SerializeField] private Color tabIdleColor;
    [SerializeField] private Color tabSelectedColor;
    [SerializeField] private AnalyticsManager analyticsManager;
    private Tab selectedTab;

    public void Start()
    {
        // Select first tab
        foreach (Tab tabButton in tabButtonList)
        {
            if (tabButton.transform.GetSiblingIndex() == 0)
                OnTabSelected(tabButton);
        }
    }

    public void Subscribe(Tab tabButton)
    {
        tabButtonList.Add(tabButton);
        // Sort by order in hierarchy
        tabButtonList.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
    }

    public void OnTabSelected(Tab tabButton)
    {
        RectTransform rt;
        if (selectedTab != null)
        {
            rt = selectedTab.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40);
        }

        selectedTab = tabButton;

        rt = selectedTab.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 50);

        ResetTabs();
        tabButton.background.color = tabSelectedColor;
        int index = tabButton.transform.GetSiblingIndex();

        analyticsManager.DisplayAnalyticsOnContent(selectedTab.contentType);
        Debug.Log($"content type = {selectedTab.contentType}");
    }

    public void ResetTabs()
    {
        foreach (Tab tabButton in tabButtonList)
        {
            if ((selectedTab != null) && (tabButton == selectedTab))
                continue;
            tabButton.background.color = tabIdleColor;
        }
    }
}

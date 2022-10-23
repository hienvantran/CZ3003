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
    public List<Tab> tabButtons = new List<Tab>();

    //In case I need to sort the lists by GetSiblingIndex
    //objListOrder.Sort((x, y) => x.OrderDate.CompareTo(y.OrderDate));

    [SerializeField] private Color tabIdleColor;
    [SerializeField] private Color tabSelectedColor;
    [SerializeField] private AnalyticsManager analyticsManager;
    private Tab selectedTab;

    public void Start()
    {
        // Select first tab
        foreach (Tab tabButton in tabButtons)
        {
            if (tabButton.transform.GetSiblingIndex() == 0)
                OnTabSelected(tabButton);
        }
    }

    public void Subscribe(Tab tabButton)
    {
        tabButtons.Add(tabButton);
        // Sort by order in hierarchy
        tabButtons.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
    }

    public void OnTabExit(Tab tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(Tab tabButton)
    {
        RectTransform rt;
        if (selectedTab != null)
        {
            selectedTab.Deselect();
             rt = selectedTab.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40);
        }

        selectedTab = tabButton;

        selectedTab.Select();
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
        foreach (Tab tabButton in tabButtons)
        {
            if ((selectedTab != null) && (tabButton == selectedTab))
                continue;
            tabButton.background.color = tabIdleColor;
        }
    }

    public void NextTab()
    {
        int currentIndex = selectedTab.transform.GetSiblingIndex();
        int nextIndex = currentIndex < tabButtons.Count - 1 ? currentIndex + 1 : tabButtons.Count - 1;
        OnTabSelected(tabButtons[nextIndex]);
    }

    public void PreviousTab()
    {
        int currentIndex = selectedTab.transform.GetSiblingIndex();
        int previousIndex = currentIndex > 0 ? currentIndex - 1 : 0;
        OnTabSelected(tabButtons[previousIndex]);
    }
}

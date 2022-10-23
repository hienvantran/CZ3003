/* Reference
https://github.com/IkeThermite/GameDevGuide-CustomTabsAndFlexibleGrid
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private TabGroup tabGroup;
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    public AnalyticsManager.ContentType contentType;

    [HideInInspector]
    public Image background;

    void Start()
    {
        background = GetComponent<Image>();
        if (tabGroup != null)
            tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabSelected.Invoke();
        }

    }


}

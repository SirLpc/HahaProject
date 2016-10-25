using UnityEngine;
using System.Collections;

public class ShipEyeBase : MonoBehaviour
{
    [SerializeField]
    protected Transform _renderTr;

    protected int _visibleLayer;
    protected int _disibleLayer;

    protected ShipControlBase _ship;

    private void Awake()
    {
        _visibleLayer = LayerMask.NameToLayer(Tags.VisibleLayer);
        _disibleLayer = LayerMask.NameToLayer(Tags.DisibleLayer);
        _ship = GetComponentInParent<ShipControlBase>();
    }

    public void Init(Transform render)
    {
        _renderTr = render;

        ShowAllRender((this as ShipVisibleEye) != null);
    }

    protected void ShowAllRender(bool show)
    {
        _renderTr.gameObject.layer = show ? _visibleLayer : _disibleLayer;
        ShowRender(_renderTr, show);
    }

    private void ShowRender(Transform tr, bool show)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            var child = tr.GetChild(i);
            child.gameObject.layer = show ? _visibleLayer : _disibleLayer;
            ShowRender(child, show);
        }
    }

}

using UnityEngine;
using System.Collections;

/// <summary>
/// Class that extends the features of the CanvasGroup.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Container : MonoBehaviour
{
    
    #region Input

    /// <summary>
    /// Mouse position relative to this container.
    /// </summary>
    public Vector2 mouse
    {
        get
        {   
            Vector2 pos = Vector2.zero;
            Camera cam  = canvas ? canvas.worldCamera : null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,Input.mousePosition,cam,out pos);
            return pos;
        }
    }

    /// <summary>
    /// Flag that indicates this container will be mouse interactable.
    /// </summary>
    public bool mouseEnabled
    {
        get { return group.interactable || group.blocksRaycasts; }
        set { group.interactable = group.blocksRaycasts = value; }
    }

    #endregion

    #region Transform

    /// <summary>
    /// Position of this element.
    /// </summary>
    public Vector2 position
    {
        get { return rectTransform.anchoredPosition; }
        set { rectTransform.anchoredPosition = value; }
    }

    /// <summary>
    /// X Position
    /// </summary>
    public float x { get { return position.x; } set { Vector2 v = position; v.x = value; position = v; } }
    
    /// <summary>
    /// Y Position
    /// </summary>
    public float y { get { return position.y; } set { Vector2 v = position; v.y = value; position = v; } }

    

    /// <summary>
    /// Rotation;
    /// </summary>
    public float rotation { get { return rectTransform.localEulerAngles.z; } set { Vector3 v = rectTransform.localEulerAngles; v.z = value; rectTransform.localEulerAngles = v; } }

    /// <summary>
    /// Size
    /// </summary>
    public Vector2 size { get { return rectTransform.sizeDelta; } set { rectTransform.sizeDelta = value; } }

    /// <summary>
    /// Width
    /// </summary>
    public float width { get { return size.x; } set { Vector2 v = size; v.x = value; size = v; } }

    /// <summary>
    /// Height
    /// </summary>
    public float height { get { return size.y; } set { Vector2 v = size; v.y = value; size = v; } }

    #endregion

    #region Material

    /// <summary>
    /// Flag that indicates this element is visible.
    /// </summary>
    public bool visible
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    /// <summary>
    /// Alpha component of this element.
    /// </summary>
    public float alpha
    {
        get { return m_alpha; }
        set { group.alpha = m_alpha = value; gameObject.SetActive(m_alpha >= 0f); }
    }
    private float m_alpha;

    #endregion

    #region References

    /// <summary>
    /// Reference to this element canvas group.
    /// </summary>
    public CanvasGroup group 
    { 
        get
        {
            if(m_group)return  m_group;
            if(!this) return m_group;
            m_group = GetComponent<CanvasGroup>();
            m_alpha = m_group.alpha;
            return m_group;
        } 
    }
    private CanvasGroup m_group;

    /// <summary>
    /// Reference to the rect transform.
    /// </summary>
    public RectTransform rectTransform
    {
        get { return m_rect_transform ? m_rect_transform : (m_rect_transform = GetComponent<RectTransform>()); }
    }
    private RectTransform m_rect_transform;

    /// <summary>
    /// Reference to the canvas containing this instance.
    /// </summary>
    public Canvas canvas
    {
        get 
        { 
            if(m_canvas) return m_canvas; 
            Transform t = transform; 
            while(t && (!m_canvas)) { t = t.parent; m_canvas = t.GetComponent<Canvas>();  }             
            return m_canvas;  
        }
    }    
    private Canvas m_canvas;

    #endregion

    #region Find

    /// <summary>
    /// Finds a child of this container at path.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p_path"></param>
    /// <returns></returns>
    public T Find<T>(string p_path) where T : Component {
        string[] pl = p_path.Split('.');
        Transform t = transform;
        for(int i=0;i<pl.Length;i++) {
            t = t.FindChild(pl[i]);
            if(!t) return null;
        }
        return t.GetComponent<T>();
    }

    /// <summary>
    /// Finds a child Transform of this container at path.
    /// </summary>
    /// <param name="p_path"></param>
    /// <returns></returns>
    public Transform Find(string p_path) { return Find<Transform>(p_path); }

    /// <summary>
    /// Tells if a given child is contained in this container.
    /// </summary>
    /// <param name="p_child"></param>
    /// <returns></returns>
    public bool Contains(Component p_child) {
        bool found=false;
        Traverse(delegate(Transform it) {
            if(it.gameObject == p_child.gameObject) found=true;
        });
        return found;
    }

    /// <summary>
    /// Traverses this container hierarchy.
    /// </summary>
    /// <param name="p_callback"></param>
    public void Traverse(System.Action<Transform> p_callback) {
        TraverseStep(transform,p_callback);
    }

    /// <summary>
    /// Helper iterator callback.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="cb"></param>
    private void TraverseStep(Transform t,System.Action<Transform> cb) {
        if(t!=transform)if(t)cb(t);
        for(int i=0;i<t.childCount;i++) { TraverseStep(t.GetChild(i),cb); }
    }

    #endregion

    /// <summary>
    /// Callback called when parenting is changed.
    /// </summary>
    private void OnTransformParentChanged()
    {
        m_canvas = null;
    }
    
}

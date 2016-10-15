using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StateBase))]
public class StateInfoBar : MonoBehaviour
{
	public Vector3 offset = new Vector3(0, 2.5f, 0);		// Offset for the health bar position relative to object position
	public float barWidth = 100;
    public float barHeight = 20;
    public float nameHeight = 20;

	private Rect _barRect;
    private Rect _nameRect;

	private Vector3 _worldPosition = new Vector3();		// Transform position + offset position
	private Vector3 _screenPosition = new Vector3();	// Screen position for drawing health bar

	private bool _visible = false;						// Health bar is visible or not

	private StateBase _statController;	// Reference to Enemy Stat script
	private Transform _t;								// Reference to object transform
	private Color _barColor = Color.red;				// Used by the Infobar

	public Color BarColor
	{
		get { return _barColor; }
		set { _barColor = value; }
	}

	void Start()
	{
		_t = transform;
		_statController = GetComponent<StateBase>();
	}

	void Update()
	{
		// Set health bar position
		_worldPosition = _t.position + offset;

		// Check to see if health bar is within the camera's view
		Vector3 viewPoint = Camera.main.WorldToViewportPoint(_worldPosition);
		_visible = (viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1 && viewPoint.z >= 0);

		if(_visible)
		{
			// Update screen position for drawing the health bar
			_screenPosition = Camera.main.WorldToScreenPoint(_worldPosition);

			float x = _screenPosition.x - barWidth / 2f;
			float y = Screen.height - _screenPosition.y;
			_barRect = new Rect(x, y, barWidth, barHeight);
            _nameRect = new Rect(x, y - barHeight, barWidth, nameHeight);
		}
	}

	void OnGUI()
	{
		// Draw the health bar if we can see it
		if(_visible)
		{
			GUI.color = _barColor;
			GUI.HorizontalScrollbar(_barRect, 0, _statController.health, 0, _statController.maxHealth); // Displays a healthbar
			GUI.color = Color.white;
			GUI.contentColor = Color.white;
            var txtInfo = string.Format("{0}/{1}", _statController.health, _statController.maxHealth);
			GUI.Label(_barRect, txtInfo); // Displays health in text format
            GUI.Label(_nameRect, _statController.playerName);
		}
	}
}
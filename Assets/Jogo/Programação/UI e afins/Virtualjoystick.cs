using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Virtualjoystick : MonoBehaviour
{
    public static Virtualjoystick Instance { get; private set; }

    public Vector2 Direction { get; private set; }

    private VisualElement joystick_background;
    private VisualElement joystick_handle;

    private bool is_dragging;
    private int active_pointer_id = -1;

    private void OnEnable()
    {
        Instance = this;

        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;

        joystick_background = root.Q<VisualElement>("Background");

        joystick_handle = root.Q<VisualElement>("Handle");

        if (joystick_handle == null || joystick_background == null)
        {
            print("Sem Handle ou Background!");
            enabled = false;
            return;
        }

        joystick_handle.pickingMode = PickingMode.Ignore;

        joystick_background.RegisterCallback<PointerDownEvent>(OnPointerDown);
        joystick_background.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        joystick_background.RegisterCallback<PointerUpEvent>(OnPointerUp);
        joystick_background.RegisterCallback<PointerCancelEvent>(OnPointerCancel);

        ResetJoystick();
    }

    private void OnDisable()
    {
        if (joystick_background != null)
        {
            joystick_background.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            joystick_background.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            joystick_background.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            joystick_background.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);

            if (active_pointer_id != -1 && joystick_background.HasPointerCapture(active_pointer_id))
            {
                joystick_background.ReleasePointer(active_pointer_id);
            }
        }

        is_dragging = false;
        active_pointer_id = -1;
        Direction = Vector2.zero;

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        if (is_dragging)
        {
            return;
        }

        is_dragging= true;
        active_pointer_id = evt.pointerId;

        joystick_background.CapturePointer(active_pointer_id);

        UpdateJoystick(evt.position);
        evt.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!is_dragging || evt.pointerId != active_pointer_id)
        {
            return;
        }

        UpdateJoystick(evt.position);
        evt.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!is_dragging || evt.pointerId != active_pointer_id)
        {
            return;
        }

        EndDrag();
        evt.StopPropagation();
    }

    private void OnPointerCancel(PointerCancelEvent evt)
    {
        if (!is_dragging || evt.pointerId != active_pointer_id)
        {
            return;
        }

        EndDrag();
        evt.StopPropagation();
    }

    private void UpdateJoystick(Vector2 panel_position)
    {
        Vector2 local_position = joystick_background.WorldToLocal(panel_position);
        Rect rect = joystick_background.contentRect;
        Vector2 center = rect.center;

        float handle_width = joystick_handle.resolvedStyle.width;
        float handle_height = joystick_handle.resolvedStyle.height;
        float handle_radius = Mathf.Max(handle_width, handle_height) * 0.5f;
        float max_radius = Mathf.Min(rect.width, rect.height) * 0.5f - handle_radius;

        if (max_radius < 0f)
        {
            return;
        }

        Vector2 offset = local_position - center;

        if (offset.magnitude > max_radius)
        {
            offset = offset.normalized * max_radius;
        }

        joystick_handle.style.left = center.x + offset.x - handle_width * 0.5f;
        joystick_handle.style.top = center.y + offset.y - handle_height * 0.5f;

        Direction = offset / max_radius;
    }

    private void EndDrag()
    {
        int pointer_id = active_pointer_id;

        is_dragging = false;
        active_pointer_id = -1;

        if(joystick_background.HasPointerCapture(pointer_id))
        {
            joystick_background.ReleasePointer(pointer_id);
        }

        ResetJoystick();
    }

    private void ResetJoystick()
    {
        Direction = Vector2.zero;

        if (joystick_background == null || joystick_handle == null)
        {
            return;
        }

        Rect rect = joystick_background.contentRect;

        float handle_width = joystick_handle.resolvedStyle.width;
        float handle_height = joystick_handle.resolvedStyle.height;

        joystick_handle.style.left = rect.x + handle_width * 0.5f;
        joystick_handle.style.top = rect.y + handle_height * 0.5f;
    }
}

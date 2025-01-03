namespace MonoGameUI.Elements;

/// <summary>
/// Defines an element that receives and handles user input.
/// </summary>
public abstract class Control(UserInterface ui) : Element(ui), IMouseInputEventHandler
{
    public ControlState CurrentState { get; protected set; } = ControlState.NORMAL;
    private bool lButtonDown = false;
    private bool rButtonDown = false;
    private bool mButtonDown = false;

    /// <summary>
    /// Called when the mouse enters the control.
    /// </summary>
    public virtual void OnMouseEnter() { }

    /// <summary>
    /// Called when the mouse exits the control.
    /// </summary> 
    public virtual void OnMouseExit()
    {
        lButtonDown = false;
        rButtonDown = false;
        mButtonDown = false;
    }

    /// <summary>
    /// Called when the left mouse button is pressed down while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseButtonDown()
    {
        lButtonDown = true;
    }

    /// <summary>
    /// Called when the right mouse button is pressed down while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseRightButtonDown()
    {
        rButtonDown = true;
    }

    /// <summary>
    /// Called when the middle mouse button is pressed down while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseMiddleButtonDown()
    {
        mButtonDown = true;
    }

    /// <summary>
    /// Called when the left mouse button is released while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseButtonUp()
    {
        if (lButtonDown)
        {
            OnMouseClick();
            lButtonDown = false;
        }
    }

    /// <summary>
    /// Called when the right mouse button is released while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseRightButtonUp()
    {
        if (rButtonDown)
        {
            OnRightClick();
            rButtonDown = false;
        }
    }

    /// <summary>
    /// Called when the middle mouse button is released while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseMiddleButtonUp()
    {
        if (mButtonDown)
        {
            OnMiddleClick();
            mButtonDown = false;
        }
    }

    /// <summary>
    /// Called when the left mouse button is clicked while the mouse is over the control.
    /// </summary>
    public virtual void OnMouseClick() { }

    /// <summary>
    /// Called when the right mouse button is clicked while the mouse is over the control.
    /// </summary>
    public virtual void OnRightClick() { }

    /// <summary>
    /// Called when the middle mouse button is clicked while the mouse is over the control.
    /// </summary>
    public virtual void OnMiddleClick() { }

    /// <summary>
    /// Called when the left mouse button is double-clicked while the mouse is over the control.
    /// </summary>
    public virtual void OnDoubleClick() { }

    /// <summary>
    /// Called when the right mouse button is double-clicked while the mouse is over the control.
    /// </summary>
    public virtual void OnRightDoubleClick() { }

    /// <summary>
    /// Called when the middle mouse button is double-clicked while the mouse is over the control.
    /// </summary>
    public virtual void OnMiddleDoubleClick() { }

    /// <summary>
    /// Handles mouse input events for the control.
    /// </summary>
    public bool HandleMouseEvents(MouseInputController mouseInput)
    {
        if (ScreenRect.Contains(mouseInput.CurrentState.Position))
        {
            if (!ScreenRect.Contains(mouseInput.PreviousState.Position)) OnMouseEnter();

            if (mouseInput.ButtonDown) OnMouseButtonDown();
            if (mouseInput.RightButtonDown) OnMouseRightButtonDown();
            if (mouseInput.MiddleButtonDown) OnMouseMiddleButtonDown();

            if (mouseInput.ButtonUp) OnMouseButtonUp();
            if (mouseInput.RightButtonUp) OnMouseRightButtonUp();
            if (mouseInput.MiddleButtonUp) OnMouseMiddleButtonUp();

            if (mouseInput.DoubleClick) OnDoubleClick();
            if (mouseInput.RightDoubleClick) OnRightDoubleClick();
            if (mouseInput.MiddleDoubleClick) OnMiddleDoubleClick();

            return true;
        }
        else if (ScreenRect.Contains(mouseInput.PreviousState.Position))
        {
            OnMouseExit();
        }

        return false;
    }
}
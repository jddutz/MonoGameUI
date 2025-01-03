using MonoGameUI.Elements;

namespace MonoGameUI;

/// <summary>
/// Defines a complex UI Element composed of other sub-elements
/// used for modular UI design.
/// </summary>
public abstract class Component(UserInterface ui) : Container(ui)
{
    public abstract void Build();
}
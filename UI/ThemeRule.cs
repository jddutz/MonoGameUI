using MonoGameUI;
using MonoGameUI.Elements;

namespace MonoGameUI;

public interface IThemeRule
{
    bool TryGetElementStyle(Element element, out ElementStyle style);
}

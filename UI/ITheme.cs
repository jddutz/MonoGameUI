namespace MonoGameUI;

public interface ITheme
{
    ElementStyle DefaultElementStyle { get; set; }
    Dictionary<string, ElementStyle> ElementStyles { get; }
    List<IThemeRule> Rules { get; }
}
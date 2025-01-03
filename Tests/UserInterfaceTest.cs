using Moq;
using MonoGameUI;
using MonoGameUI.Elements;

public class UserInterfaceTest
{
    [Fact(DisplayName = "Clear() should remove all elements from the collection")]
    public void ClearTest01()
    {
        // Arrange
        var userInterface = new UserInterface();
        var element = new Mock<Element>().Object;
        userInterface.Add(element);

        // Act
        userInterface.Clear();

        // Assert
        Assert.Empty(userInterface.Elements);
    }

    [Fact(DisplayName = "Clear() should set the Parent property of all elements to null")]
    public void ClearTest02()
    {
        // Arrange
        var userInterface = new UserInterface();
        var element = new Mock<Element>().Object;
        userInterface.Add(element);

        // Act
        userInterface.Clear();

        // Assert
        Assert.Null(element.Parent);
    }

    [Fact(DisplayName = "SetFocus() should change the focus to the specified element")]
    public void SetFocusTest01()
    {
        // Arrange
        var userInterface = new UserInterface();
        var element = new Mock<Element>().Object;

        // Act
        var result = userInterface.SetFocus(element);

        // Assert
        Assert.True(result);
        Assert.Equal(element, userInterface.FocusedElement);
    }

    [Fact(DisplayName = "SetFocus() should return true if the focus was successfully changed")]
    public void SetFocusTest02()
    {
        // Arrange
        var userInterface = new UserInterface();
        var element = new Mock<Element>();
        element.Setup(e => e.SetFocus(true)).Returns(true);

        // Act
        var result = userInterface.SetFocus(element.Object);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "SetFocus() should return false if the focus was not changed")]
    public void SetFocusTest03()
    {
        // Arrange
        var userInterface = new UserInterface();
        var element = new Mock<Element>();
        element.Setup(e => e.SetFocus(true)).Returns(false);

        // Act
        var result = userInterface.SetFocus(element.Object);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "SetFocus() should remove focus from the current element if the specified element is null")]
    public void SetFocusTest04()
    {
        // Arrange
        var userInterface = new UserInterface();
        var element = new Mock<Element>();
        element.Setup(e => e.SetFocus(false)).Returns(true);
        userInterface.SetFocus(element.Object);

        // Act
        var result = userInterface.SetFocus(null);

        // Assert
        Assert.True(result);
        Assert.Null(userInterface.FocusedElement);
    }
}
using Moq;

namespace MonoGameUI.Test;


public class UserInterfaceManagerTest
{
    // Suppress "Cannot convert null literal to non-nullable reference type" warning
    // We are testing how the constructor handles null arguments, so the warning is not relevant here
#pragma warning disable CS8625

    [Fact(DisplayName = "Constructor should throw ArgumentNullException when game is null")]
    public static void ConstructorTest01()
    {
        // Arrange
        Theme theme = new();

        // Act
        void action()
        {
            var systemUnderTest = new UserInterfaceManager(null, theme);
        }

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

#pragma warning restore CS8625

    [Fact(DisplayName = "Constructor should set a default theme when theme is null")]
    public static void ConstructorTest02()
    {
        // Arrange
        var mockGame = new MockGame();

        // Act
        var systemUnderTest = new UserInterfaceManager(mockGame);

        // Assert
        Assert.NotNull(systemUnderTest.Theme);
    }

    [Fact(DisplayName = "Constructor should set the graphics device and content when game is not null")]
    public static void ConstructorTest03()
    {
        // Arrange
        var mockGame = new MockGame();

        // Act
        var systemUnderTest = new UserInterfaceManager(mockGame);

        // Assert
        Assert.Equal(mockGame.GraphicsDevice, systemUnderTest.GraphicsDevice);
        Assert.Equal(mockGame.Content, systemUnderTest.Content);
    }

    [Fact(DisplayName = "Constructor should set the theme when theme is provided")]
    public static void ConstructorTest04()
    {
        // Arrange
        var mockGame = new MockGame();
        var theme = new Theme();

        // Act
        var systemUnderTest = new UserInterfaceManager(mockGame, theme);

        // Assert
        Assert.Equal(theme, systemUnderTest.Theme);
    }

    [Fact(DisplayName = "Load() should set the Manager property of the UI to the current instance")]
    public static void LoadTest01()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();

        // Act
        systemUnderTest.Load(ui.Object);

        // Assert
        Assert.Equal(systemUnderTest, ui.Object.Manager);
    }

    [Fact(DisplayName = "Load() should call the Build() method of the UI once")]
    public static void LoadTest02()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();

        // Act
        systemUnderTest.Load(ui.Object);

        // Assert
        ui.Verify(ui => ui.Build(), Times.Once);
    }

    [Fact(DisplayName = "Load() should not call Activate() on the pending UI")]
    public static void LoadTest03()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();

        // Act
        systemUnderTest.Load(ui.Object);

        // Assert
        ui.Verify(ui => ui.Activate(), Times.Never);
    }

    [Fact(DisplayName = "Load() should not set the current UI if it is the first UI loaded")]
    public static void LoadTest04()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();

        // Act
        systemUnderTest.Load(ui.Object);

        // Assert
        Assert.Null(systemUnderTest.Current);
    }

    [Fact(DisplayName = "Load() should not change the current UI if a UI has already been loaded")]
    public static void LoadTest05()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);

        // Act
        systemUnderTest.Load(ui2.Object);

        // Assert
        Assert.Equal(ui1.Object, systemUnderTest.Current);
    }

    [Fact(DisplayName = "Load() should not call Unload() on the current UI")]
    public static void LoadTest06()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Load(ui2.Object);

        // Assert
        ui1.Verify(ui => ui.Unload(), Times.Never);
    }

    // Suppress "Cannot convert null literal to non-nullable reference type" warning
    // We are testing how the load() method handles null arguments, so the warning is not relevant here
#pragma warning disable CS8625

    [Fact(DisplayName = "Load() should throw ArgumentNullException if ui is null")]
    public static void LoadTest07()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);

        // Act
        void action() => systemUnderTest.Load(null);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

#pragma warning restore CS8625

    [Fact(DisplayName = "Update() should call Activate() on the pending UI once")]
    public static void UpdateTest01()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Update(0);

        // Assert
        ui1.Verify(ui => ui.Activate(), Times.Once);
    }

    [Fact(DisplayName = "Update() should set the current UI after Load() has been called")]
    public static void UpdateTest02()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Update(0);

        // Assert
        Assert.Equal(ui1.Object, systemUnderTest.Current);
    }

    [Fact(DisplayName = "Update() should not call Build() or Unload() on the pending UI")]
    public static void UpdateTest03()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Update(0);

        // Assert
        ui1.Verify(ui => ui.Build(), Times.Never);
        ui2.Verify(ui => ui.Unload(), Times.Never);
    }

    [Fact(DisplayName = "Update() should call Unload() on the current UI once")]
    public static void UpdateTest04()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        systemUnderTest.Load(ui2.Object);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Update(0);

        // Assert
        ui1.Verify(ui => ui.Unload(), Times.Once);
    }

    [Fact(DisplayName = "Update() should call Activate() on the current UI once")]
    public static void UpdateTest05()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Load(ui2.Object);
        systemUnderTest.Update(0);

        // Assert
        ui2.Verify(ui => ui.Activate(), Times.Once);
    }

    [Fact(DisplayName = "Update() should add the current UI to History when a new UI is activated")]
    public static void UpdateTest06()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        ui1.Invocations.Clear();
        ui2.Invocations.Clear();

        // Act
        systemUnderTest.Load(ui2.Object);
        systemUnderTest.Update(0);

        // Assert
        Assert.True(systemUnderTest.History.Contains(ui1.Object));
    }

    [Fact(DisplayName = "Update() should call Update() on the pending UI once")]
    public static void UpdateTest07()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        systemUnderTest.Update(0);

        // Assert
        ui.Verify(ui => ui.Update(0), Times.Once);
    }

    [Fact(DisplayName = "Update() should call Update() on the current UI once per invocation")]
    public static void UpdateTest08()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        systemUnderTest.Update(0);
        systemUnderTest.Update(0);
        systemUnderTest.Update(0);

        // Assert
        ui.Verify(ui => ui.Update(0), Times.Exactly(3));
    }

    [Fact(DisplayName = "Update() should pass the correct value to the Update() method of the current UI")]
    public static void UpdateTest09()
    {
        // Arrange
        float value0 = 0.5f;
        float value1 = 0.1f;
        float value2 = 0.2f;
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);
        systemUnderTest.Update(value0);

        // Act
        systemUnderTest.Update(value1);
        systemUnderTest.Update(value2);

        // Assert
        ui.Verify(ui => ui.Update(value0), Times.Once);
        ui.Verify(ui => ui.Update(value1), Times.Once);
        ui.Verify(ui => ui.Update(value2), Times.Once);
    }

    [Fact(DisplayName = "Update() should not throw an exception if a UI has not been loaded")]
    public static void UpdateTest10()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();

        // Act
        systemUnderTest.Update(0);

        // Assert
    }

    [Fact(DisplayName = "Update() should throw ArgumentOutOfRangeException if dt is negative")]
    public static void UpdateTest11()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        void action() => systemUnderTest.Update(-1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact(DisplayName = "Update() should throw ArgumentOutOfRangeException if dt is INF")]
    public static void UpdateTest12()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        void action() => systemUnderTest.Update(float.PositiveInfinity);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact(DisplayName = "Update() should throw ArgumentOutOfRangeException if dt is -INF")]
    public static void UpdateTest13()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        void action() => systemUnderTest.Update(float.NegativeInfinity);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact(DisplayName = "Update() should throw ArgumentOutOfRangeException if dt is NaN")]
    public static void UpdateTest14()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        void action() => systemUnderTest.Update(float.NaN);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact(DisplayName = "Update() should not throw any exception if dt is zero")]
    public static void UpdateTest15()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);

        // Act
        void action() => systemUnderTest.Update(0);

        // Assert
        var exception = Record.Exception(action);
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Back() should not throw an exception if no UI has been loaded")]
    public static void BackTest01()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);

        // Act
        systemUnderTest.Back();

        // Assert
        Assert.Null(systemUnderTest.Current);
    }

    [Fact(DisplayName = "Back() should not change the current UI immediately")]
    public static void BackTest02()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        systemUnderTest.Load(ui2.Object);
        systemUnderTest.Update(0);

        // Act
        systemUnderTest.Back();

        // Assert
        Assert.Equal(ui2.Object, systemUnderTest.Current);
    }

    [Fact(DisplayName = "Update() should load the previous UI after calling Back()")]
    public static void UpdateTest16()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        systemUnderTest.Load(ui2.Object);
        systemUnderTest.Update(0);

        // Act
        systemUnderTest.Back();
        systemUnderTest.Update(0);

        // Assert
        Assert.Equal(ui1.Object, systemUnderTest.Current);
    }

    [Fact(DisplayName = "Update() should call Activate() on the previous UI after calling Back()")]
    public static void UpdateTest17()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui1 = new Mock<UserInterface>();
        var ui2 = new Mock<UserInterface>();
        systemUnderTest.Load(ui1.Object);
        systemUnderTest.Update(0);
        systemUnderTest.Load(ui2.Object);
        systemUnderTest.Update(0);

        // Act
        systemUnderTest.Back();
        systemUnderTest.Update(0);

        // Assert
        ui1.Verify(ui => ui.Activate(), Times.Exactly(2));
    }

    [Fact(DisplayName = "Update() should not throw an exception or change the current UI after calling Back() if History is empty")]
    public static void UpdateTest18()
    {
        // Arrange
        var mockGame = new MockGame();
        var systemUnderTest = new UserInterfaceManager(mockGame);
        var ui = new Mock<UserInterface>();
        systemUnderTest.Load(ui.Object);
        systemUnderTest.Update(0);

        // Act
        systemUnderTest.Back();
        systemUnderTest.Update(0);

        // Assert
        Assert.Equal(ui.Object, systemUnderTest.Current);
    }
}
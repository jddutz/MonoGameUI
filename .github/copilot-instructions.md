# Code Review and Test Requirements

All class members must be documented using documentation comments. For public methods, all parameters, exceptions thrown, and return value (unless void) shall be documented.

All public constructors and static factory methods must be thoroughly tested. All objects must be in a usable state after calling the constructor or factory method. All constructors with internally-defined objects as arguments shall accept an interface, and not a concrete or abstract base class.

Property setters and getters should be a single statement and not introduce any side effects. Where more complex behavior is required, the setter or getter shall be replaced by a protected virtual method. Multiple internal fields or dependent properties may need to be set at the same time but it should be explained in the documentation when this is required.

Public property setters and getters must be included in test coverage, but do not require specific testing.

All public methods shall be thoroughly tested, including all boundary and failure conditions in valid and invalid object states. Private and protected methods do not need to be tested, as long as they are included in test coverage through testing of public methods and properties.

We do not need to test the internal state of an object only public-facing members. Private fields and functions are not tested.

# Test Framework and Naming Conventions

This project uses xUnit test framework. Each unit to be tested has a test class named <SystemUnderTest>Test. For example, the 'UserInterface' class has a test class called 'UserInterfaceTest'.

Test methods are named <MethodName>Test<Number>, e.g. ConstructorTest01 or SetFocusTest02. Each test uses the DisplayName property of the Fact or Theory attribute to describe the test. The description should use argument names when referring to a method's arguments.

Test methods should have three parts: Arrange, Act, and Assert. Each section is identified by a comment. The Assert section may be empty for void functions where no changes to internal state are expected. Except in rare circumstances, the Act section should be a single line of code, or the definition of a single function called action() to test exception handling.

The Moq library is used to mock interfaces as required. Some external third-party classes cannot be mocked, such as Microsoft.Xna.Framework.Game. In these cases, a wrapper class shall be created and used in its place (e.g. MonoGameUI.Test.MockGame)

### Examples:

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

# Methodology

1. Describe the system under test:

- Name: What is it called?
- Purpose: What is its purpose?
- Design Considerations: What are some critical factors affecting the design of this system?
- Members: Bullet-point list of members (- MemberName: Description)

2. Conduct a cursory code review as per the Code Review and Test Requirements described previously. Provide a bullet-point list of all findings. If the results include any code changes and not just documentation updates, stop and do not proceed to the next step.

3. Provide a list of required tests, grouped by class member. This list should be a single line of text describing each test and not include any code. Indicate which tests have been written, and those still required.

4. For each of the required tests, write the test methodology then generate the required test code. Include appropriate Fact or Theory attributes, Arrange, Act, and Assert sections. We may need to continue several times before all tests are generated.

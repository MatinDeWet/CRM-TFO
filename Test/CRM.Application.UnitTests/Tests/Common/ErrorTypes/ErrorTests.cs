using CRM.Application.Common.ErrorTypes;
using CRM.Application.Common.ErrorTypes.Enums;
using Shouldly;

namespace CRM.Application.UnitTests.Tests.Common.ErrorTypes;
public class ErrorTests
{
    [Fact]
    public void Constructor_WhenCalled_ShouldCreateErrorWithCorrectProperties()
    {
        // Arrange
        const string code = "Test.Code";
        const string description = "Test description";
        const ErrorTypeEnum type = ErrorTypeEnum.Problem;

        // Act
        var error = new Error(code, description, type);

        // Assert
        error.Code.ShouldBe(code);
        error.Description.ShouldBe(description);
        error.Type.ShouldBe(type);
    }

    [Fact]
    public void None_ShouldBePreinitialized()
    {
        // Act & Assert
        Error.None.Code.ShouldBe(string.Empty);
        Error.None.Description.ShouldBe(string.Empty);
        Error.None.Type.ShouldBe(ErrorTypeEnum.Failure);
    }

    [Fact]
    public void NullValue_ShouldBePreinitializedWithCorrectValues()
    {
        // Act & Assert
        Error.NullValue.Code.ShouldBe("General.Null");
        Error.NullValue.Description.ShouldBe("Null value was provided");
        Error.NullValue.Type.ShouldBe(ErrorTypeEnum.Failure);
    }

    [Fact]
    public void Failure_WhenCalled_ShouldCreateErrorWithFailureType()
    {
        // Arrange
        const string code = "Test.Failure";
        const string description = "Failure description";

        // Act
        var error = Error.Failure(code, description);

        // Assert
        error.Code.ShouldBe(code);
        error.Description.ShouldBe(description);
        error.Type.ShouldBe(ErrorTypeEnum.Failure);
    }

    [Fact]
    public void NotFound_WhenCalled_ShouldCreateErrorWithNotFoundType()
    {
        // Arrange
        const string code = "Test.NotFound";
        const string description = "NotFound description";

        // Act
        var error = Error.NotFound(code, description);

        // Assert
        error.Code.ShouldBe(code);
        error.Description.ShouldBe(description);
        error.Type.ShouldBe(ErrorTypeEnum.NotFound);
    }

    [Fact]
    public void Problem_WhenCalled_ShouldCreateErrorWithProblemType()
    {
        // Arrange
        const string code = "Test.Problem";
        const string description = "Problem description";

        // Act
        var error = Error.Problem(code, description);

        // Assert
        error.Code.ShouldBe(code);
        error.Description.ShouldBe(description);
        error.Type.ShouldBe(ErrorTypeEnum.Problem);
    }

    [Fact]
    public void Conflict_WhenCalled_ShouldCreateErrorWithConflictType()
    {
        // Arrange
        const string code = "Test.Conflict";
        const string description = "Conflict description";

        // Act
        var error = Error.Conflict(code, description);

        // Assert
        error.Code.ShouldBe(code);
        error.Description.ShouldBe(description);
        error.Type.ShouldBe(ErrorTypeEnum.Conflict);
    }

    [Fact]
    public void Error_WhenEqualValues_ShouldBeEqual()
    {
        // Arrange
        var error1 = new Error("Code", "Description", ErrorTypeEnum.Failure);
        var error2 = new Error("Code", "Description", ErrorTypeEnum.Failure);

        // Act & Assert
        error1.ShouldBe(error2);
    }

    [Fact]
    public void Error_WhenDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var error1 = new Error("Code1", "Description", ErrorTypeEnum.Failure);
        var error2 = new Error("Code2", "Description", ErrorTypeEnum.Failure);

        // Act & Assert
        error1.ShouldNotBe(error2);
    }

    [Theory]
    [InlineData("Code1", "Desc1", ErrorTypeEnum.Failure, "Code2", "Desc1", ErrorTypeEnum.Failure, false)]
    [InlineData("Code1", "Desc1", ErrorTypeEnum.Failure, "Code1", "Desc2", ErrorTypeEnum.Failure, false)]
    [InlineData("Code1", "Desc1", ErrorTypeEnum.Failure, "Code1", "Desc1", ErrorTypeEnum.NotFound, false)]
    [InlineData("Code1", "Desc1", ErrorTypeEnum.Failure, "Code1", "Desc1", ErrorTypeEnum.Failure, true)]
    public void Error_Equality_ShouldWorkCorrectly(
        string code1, string desc1, ErrorTypeEnum type1,
        string code2, string desc2, ErrorTypeEnum type2,
        bool shouldBeEqual)
    {
        // Arrange
        var error1 = new Error(code1, desc1, type1);
        var error2 = new Error(code2, desc2, type2);

        // Act & Assert
        if (shouldBeEqual)
        {
            error1.ShouldBe(error2);
        }
        else
        {
            error1.ShouldNotBe(error2);
        }
    }
}

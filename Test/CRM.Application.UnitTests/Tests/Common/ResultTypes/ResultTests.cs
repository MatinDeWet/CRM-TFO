using CRM.Domain.Common.ErrorTypes;
using CRM.Domain.Common.ErrorTypes.Enums;
using CRM.Domain.Common.ResultTypes;
using Shouldly;

namespace CRM.Application.UnitTests.Tests.Common.ResultTypes;
public class ResultTests
{
    #region Basic Result Tests

    [Fact]
    public void Constructor_WhenIsSuccessTrueAndErrorIsNone_ShouldCreateValidResult()
    {
        // Arrange & Act
        var result = new Result(true, Error.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Constructor_WhenIsSuccessFalseAndErrorIsNotNone_ShouldCreateValidResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error message");

        // Act
        var result = new Result(false, error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Constructor_WhenIsSuccessTrueAndErrorIsNotNone_ShouldThrowArgumentException()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error message");

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => new Result(true, error));

        // Assert
        exception.ParamName.ShouldBe("error");
        exception.Message.ShouldContain("Invalid error");
    }

    [Fact]
    public void Constructor_WhenIsSuccessFalseAndErrorIsNone_ShouldThrowArgumentException()
    {
        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => new Result(false, Error.None));

        // Assert
        exception.ParamName.ShouldBe("error");
        exception.Message.ShouldContain("Invalid error");
    }

    [Fact]
    public void IsFailure_ShouldBeOppositeOfIsSuccess()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(Error.Problem("Test.Problem", "Test problem"));

        // Assert
        successResult.IsFailure.ShouldBe(!successResult.IsSuccess);
        failureResult.IsFailure.ShouldBe(!failureResult.IsSuccess);
    }

    [Fact]
    public void Success_WhenCalled_ShouldCreateSuccessResultWithNoneError()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Failure_WhenCalled_ShouldCreateFailureResultWithGivenError()
    {
        // Arrange
        var error = Error.NotFound("Test.NotFound", "Test not found");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(error);
    }

    #endregion

    #region Generic Result Tests

    [Fact]
    public void GenericConstructor_WhenIsSuccessTrueAndErrorIsNone_ShouldCreateValidResult()
    {
        // Arrange
        const string value = "Test Value";

        // Act
        var result = new Result<string>(value, true, Error.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(value);
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void GenericConstructor_WhenIsSuccessFalseAndErrorIsNotNone_ShouldCreateValidResult()
    {
        // Arrange
        var error = Error.Conflict("Test.Conflict", "Test conflict");

        // Act
        var result = new Result<string>(null, false, error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Value_WhenResultIsSuccess_ShouldReturnValue()
    {
        // Arrange
        const int value = 42;
        var result = Result.Success(value);

        // Act
        int retrievedValue = result.Value;

        // Assert
        retrievedValue.ShouldBe(value);
    }

    [Fact]
    public void Value_WhenResultIsFailure_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = Error.Problem("Test.Problem", "Test problem");
        var result = Result.Failure<string>(error);

        // Act & Assert
        InvalidOperationException exception = Should.Throw<InvalidOperationException>(() => _ = result.Value);
        exception.Message.ShouldBe("The value of a failure result can't be accessed.");
    }

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessResultWithGivenValue()
    {
        // Arrange
        const string value = "Test Value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(value);
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void FailureGeneric_WhenCalled_ShouldCreateFailureResultWithGivenError()
    {
        // Arrange
        var error = Error.Failure("Test.Failure", "Test failure");

        // Act
        var result = Result.Failure<int>(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(error);
        Should.Throw<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void ImplicitOperator_WhenValueIsNotNull_ShouldCreateSuccessResult()
    {
        // Arrange
        const string value = "Test Value";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(value);
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void ImplicitOperator_WhenValueIsNull_ShouldCreateFailureResult()
    {
        // Arrange
        string? value = null;

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(Error.NullValue);
        Should.Throw<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void ValidationFailure_WhenCalled_ShouldCreateFailureResultWithGivenError()
    {
        // Arrange
        var error = Error.Failure("Validation.Error", "Validation error message");

        // Act
        var result = Result<int>.ValidationFailure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(error);
        Should.Throw<InvalidOperationException>(() => _ = result.Value);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ResultIntegrationTest_CheckAllErrorTypesWithResults()
    {
        // Arrange & Act
        var notFoundError = Error.NotFound("Entity.NotFound", "Entity not found");
        var notFoundResult = Result.Failure(notFoundError);
        var notFoundGenericResult = Result.Failure<int>(notFoundError);

        var conflictError = Error.Conflict("Entity.Conflict", "Entity already exists");
        var conflictResult = Result.Failure(conflictError);
        var conflictGenericResult = Result.Failure<string>(conflictError);

        var problemError = Error.Problem("System.Problem", "System problem occurred");
        var problemResult = Result.Failure(problemError);
        var problemGenericResult = Result.Failure<bool>(problemError);

        var failureError = Error.Failure("Operation.Failed", "Operation failed");
        var failureResult = Result.Failure(failureError);
        var failureGenericResult = Result.Failure<double>(failureError);

        // Assert
        notFoundResult.Error.Type.ShouldBe(ErrorTypeEnum.NotFound);
        notFoundGenericResult.Error.Type.ShouldBe(ErrorTypeEnum.NotFound);

        conflictResult.Error.Type.ShouldBe(ErrorTypeEnum.Conflict);
        conflictGenericResult.Error.Type.ShouldBe(ErrorTypeEnum.Conflict);

        problemResult.Error.Type.ShouldBe(ErrorTypeEnum.Problem);
        problemGenericResult.Error.Type.ShouldBe(ErrorTypeEnum.Problem);

        failureResult.Error.Type.ShouldBe(ErrorTypeEnum.Failure);
        failureGenericResult.Error.Type.ShouldBe(ErrorTypeEnum.Failure);

        notFoundResult.IsFailure.ShouldBeTrue();
        notFoundGenericResult.IsFailure.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidationFailure_WithNullOrEmptyValues_ShouldCreateConsistentResults(string? nullOrEmptyValue)
    {
        // Act
        Result<string> implicitResult = nullOrEmptyValue;
        Result<string> explicitResult = nullOrEmptyValue is null
            ? Result.Failure<string>(Error.NullValue)
            : Result.Success(nullOrEmptyValue);

        // Assert
        if (nullOrEmptyValue is null)
        {
            implicitResult.IsSuccess.ShouldBeFalse();
            implicitResult.Error.ShouldBe(Error.NullValue);
        }
        else
        {
            implicitResult.IsSuccess.ShouldBeTrue();
            implicitResult.Value.ShouldBe(nullOrEmptyValue);
        }

        explicitResult.IsSuccess.ShouldBe(nullOrEmptyValue is not null);
    }

    #endregion
}

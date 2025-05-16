using System.Security.Claims;
using CRM.Application.Common.Security.Contracts;
using CRM.Application.Common.Security.Implementation;
using CRM.Domain.Enums;
using Shouldly;

namespace CRM.Application.UnitTests.Tests.Common.Security;

public class IdentityInfoFixture
{
    public IInfoSetter InfoSetter { get; }

    public IdentityInfoFixture()
    {
        InfoSetter = new InfoSetter();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Email, "testuser@gmail.com"),
            new(ClaimTypes.Role, ApplicationRoleEnum.Admin.ToString()),
            new(ClaimTypes.Role, ApplicationRoleEnum.SuperAdmin.ToString()),
        };

        InfoSetter.SetUser(claims);
    }
}

public class IdentityInfoTests(IdentityInfoFixture fixture) : IClassFixture<IdentityInfoFixture>
{
    private readonly IdentityInfo _identityInfo = new(fixture.InfoSetter);

    [Fact]
    public void GetIdentityId_WhenCalled_ShouldReturnIdentityId()
    {
        // Act
        int result = _identityInfo.GetIdentityId();

        // Assert
        result.ShouldBe(1);
    }

    [Fact]
    public void HasRole_WhenCalled_WithSuperAdmin_TestAdmin_ShouldReturnTrue()
    {
        // Act
        bool result = _identityInfo.HasRole(ApplicationRoleEnum.Admin);
        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasRole_WhenCalled_WithSuperAdmin_TestSuperAdmin_ShouldReturnTrue()
    {
        // Act
        bool result = _identityInfo.HasRole(ApplicationRoleEnum.SuperAdmin);
        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void GetValue_WhenCalled_ShouldReturnValue()
    {
        // Act
        string result = _identityInfo.GetValue(ClaimTypes.Email);
        // Assert
        result.ShouldBe("testuser@gmail.com");
    }

    [Fact]
    public void HasValue_WhenCalled_ShouldReturnTrue()
    {
        // Act
        bool result = _identityInfo.HasValue(ClaimTypes.Email);
        // Assert
        result.ShouldBeTrue();
    }
}

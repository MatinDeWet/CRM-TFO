using System.Security.Claims;
using CRM.Application.Common.Security.Implementation;
using Moq;
using Shouldly;

namespace CRM.Application.UnitTests.Tests.Common.Security;

public class InfoSetterTests
{
    [Fact]
    public void SetUser_WhenCalled_ShouldSetUser()
    {
        // Arrange
        var infoSetter = new InfoSetter();
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, "1")
        };

        // Act
        infoSetter.SetUser(claims);

        // Assert
        infoSetter.ShouldNotBeEmpty();
        infoSetter.Count.ShouldBe(1);
        infoSetter.ShouldContain(claims.First());
    }

    [Fact]
    public void SetUser_WhenCalled_ShouldCallClear()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, "1")
        };

        var mockInfoSetter = new Mock<InfoSetter> { CallBase = true };

        // Act
        mockInfoSetter.Object.SetUser(claims);

        // Assert
        mockInfoSetter.Verify(x => x.Clear(), Times.Once);
    }

    [Fact]
    public void SetUser_WhenCalled_ShouldCallAddRange()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, "1")
        };

        var mockInfoSetter = new Mock<InfoSetter> { CallBase = true };

        // Act
        mockInfoSetter.Object.SetUser(claims);

        // Assert
        mockInfoSetter.Verify(x => x.AddRange(It.Is<IEnumerable<Claim>>(c => c.SequenceEqual(claims))), Times.Once);
    }
}

using Bogus;
using CRM.Application.Common.Pagination;
using CRM.Application.Common.Pagination.Enums;
using CRM.Application.Common.Pagination.Models;
using CRM.Application.UnitTests.Tests.Common.Pagination.Models;
using MockQueryable;
using Shouldly;

namespace CRM.Application.UnitTests.Tests.Common.Pagination;

public class ClientFixture
{
    public IQueryable<Client> ClientsQueryable { get; }

    public ClientFixture()
    {
        Faker<Client> faker = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.IndexFaker + 1)
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName));

        List<Client> clients = faker.Generate(1_000);

        ClientsQueryable = clients.AsQueryable().BuildMock();
    }
}

public class Pageabletests(ClientFixture fixture) : IClassFixture<ClientFixture>
{
    private readonly IQueryable<Client> _clients = fixture.ClientsQueryable;

    [Fact]
    public async Task ToPageableListAsync_WhenPageNumberIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var request = new ClientPageableDto
        {
            PageNumber = -1,
            PageSize = 100,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        async Task act() => await _clients.ToPageableListAsync(request, cancellationToken);

        // Assert
        ArgumentOutOfRangeException exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
        exception.Message.ShouldContain("PageNumber");
    }

    [Fact]
    public async Task ToPageableListAsync_WhenPageNumberIsOne_ShouldReturnFirstPage()
    {
        // Arrange
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 100,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        PageableResponse<Client> response = await _clients.ToPageableListAsync(request, cancellationToken);

        // Assert
        response.TotalRecords.ShouldBe(1000);
        response.PageSize.ShouldBe(100);
        response.PageNumber.ShouldBe(1);
        response.PageCount.ShouldBe((int)Math.Ceiling(1000 / 100.0));
        response.Data.Count().ShouldBe(100);
        response.OrderBy.ShouldBe("Id");
        response.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);
        response.Data.First().Id.ShouldBe(1);
    }

    [Fact]
    public void OrderBy_ShouldOrderBySpecifiedProperty()
    {
        // Arrange
        IQueryable<Client> query = _clients;

        // Act
        IOrderedQueryable<Client> orderedQuery = query.OrderBy("FirstName");
        var list = orderedQuery.ToList();

        // Assert
        var expected = list.OrderBy(c => c.FirstName).ToList();
        list.ShouldBe(expected);
    }

    [Fact]
    public void OrderByDescending_ShouldOrderBySpecifiedPropertyDescending()
    {
        // Arrange
        IQueryable<Client> query = _clients;

        // Act
        IOrderedQueryable<Client> orderedQuery = query.OrderByDescending("FirstName");
        var list = orderedQuery.ToList();

        // Assert
        var expected = list.OrderByDescending(c => c.FirstName).ToList();
        list.ShouldBe(expected);
    }

    [Fact]
    public async Task ToPageableListAsync_WhenPageSizeIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 0,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        async Task act() => await _clients.ToPageableListAsync(request, cancellationToken);

        // Assert
        ArgumentOutOfRangeException exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
        exception.Message.ShouldContain("PageSize");
    }

    [Fact]
    public async Task ToPageableListAsync_WhenOrderByIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 100,
            OrderBy = null!,
            OrderDirection = OrderDirectionEnum.Ascending
        };
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        async Task act() => await _clients.ToPageableListAsync(request, cancellationToken);

        // Assert
        ArgumentNullException exception = await Should.ThrowAsync<ArgumentNullException>(act);
        exception.Message.ShouldContain("OrderBy");
    }

    [Fact]
    public async Task ToPageableListAsync_WhenOrderDirectionIsDescending_ShouldReturnDataInDescendingOrder()
    {
        // Arrange
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 100,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Descending
        };
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        PageableResponse<Client> response = await _clients.ToPageableListAsync(request, cancellationToken);

        // Assert
        response.Data.First().Id.ShouldBe(1000);
        response.Data.Last().Id.ShouldBe(901);
    }

    [Fact]
    public async Task ToPageableListAsync_WithCustomOrderKeySelector_ShouldOrderByKeySelector()
    {
        // Arrange
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 100,
            OrderBy = null!,
            OrderDirection = OrderDirectionEnum.Ascending
        };
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        PageableResponse<Client> response = await _clients.ToPageableListAsync(c => c.LastName, request, cancellationToken);

        // Assert
        var expected = _clients.OrderBy(c => c.LastName).Take(100).ToList();
        response.Data.ShouldBe(expected);
    }

    [Fact]
    public void OrderBy_WhenPropertyDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        IQueryable<Client> query = _clients;

        // Act
        void act() => query.OrderBy("NonExistentProperty");

        // Assert
        ArgumentException exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldContain("NonExistentProperty");
    }

    [Fact]
    public void OrderByDescending_WhenPropertyDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        IQueryable<Client> query = _clients;

        // Act
        void act() => query.OrderByDescending("NonExistentProperty");

        // Assert
        ArgumentException exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldContain("NonExistentProperty");
    }
}

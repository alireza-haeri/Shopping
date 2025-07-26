using FluentAssertions;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Features.Category.Commands;
using Shopping.Application.Features.Category.Queries;
using Shopping.Domain.Entities.Product;
using Shopping.Infrastructure.Persistence.Repositories.Common;
using Xunit.Abstractions;

namespace Shopping.Infrastructure.Persistence.Test;

public class UnitOfWorkTests : IClassFixture<PersistenceTestSetup>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly UnitOfWork _unitOfWork;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISender _sender;

    public UnitOfWorkTests(PersistenceTestSetup setup, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _unitOfWork = setup.UnitOfWork;
        _serviceProvider = setup.ServiceProvider;
        _sender = _serviceProvider.GetRequiredService<ISender>();
    }

    [Fact]
    public async Task Adding_New_Category_Should_Save_To_Db()
    {
        var category = new CategoryEntity("title");

        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();

        var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
        categories.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Getting_Category_By_Id_Should_Be_Success()
    {
        var category = new CategoryEntity("title 2");
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();

        var categoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);
        categoryInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task Added_New_Category_Should_Have_DateAdded()
    {
        var category = new CategoryEntity("title 2");
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();

        var categoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);

        categoryInDb.Should().NotBeNull();
        categoryInDb.CreatedDate.Should().BeMoreThan(TimeSpan.MinValue);

        _testOutputHelper.WriteLine($"Current Added Date: {categoryInDb.CreatedDate}");
    }

    [Fact]
    public async Task Added_New_Category_Should_Not_Have_ModifiedDate()
    {
        var category = new CategoryEntity("title 2");
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();

        var categoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);

        categoryInDb.Should().NotBeNull();
        categoryInDb.ModifiedDate.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public async Task Update_Category_Should_Have_ModifiedDate()
    {
        var category = new CategoryEntity("title 3");
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();

        var categoryInDb = await _unitOfWork.CategoryRepository.GetByIdWithTrackingAsync(category.Id);

        categoryInDb!.Edit("title 4");
        await _unitOfWork.CommitAsync();

        var editedCategoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);

        editedCategoryInDb.Should().NotBeNull();
        editedCategoryInDb.ModifiedDate.Should().NotBe(DateTime.MinValue);
    }

    [Fact]
    public async Task Adding_New_Category_ByMediator_Should_Be_Success()
    {
        var commandResult = await _sender.Send(new CreateCategoryCommand("test title category by mediator"));

        commandResult.Result.Should().BeTrue();
    }

    [Fact]
    public async Task Adding_New_Category_ByMediator_Should_Persist_Data()
    {
        await _sender.Send(new CreateCategoryCommand("test title2 category by mediator"));
        var categories = await _sender.Send(new GetAllCategoryQuery());

        categories.Result.Should().HaveCountGreaterThan(0);
    }
}
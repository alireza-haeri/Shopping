using FluentAssertions;
using Shopping.Domain.Entities.Product;
using Shopping.Infrastructure.Persistence.Repositories.Common;
using Xunit.Abstractions;

namespace Shopping.Infrastructure.Persistence.Test;

public class UnitOfWorkTests(PersistenceTestSetup setup,ITestOutputHelper testOutputHelper) : IClassFixture<PersistenceTestSetup>
{
    private readonly UnitOfWork _unitOfWork = setup.UnitOfWork;

    [Fact]
    public async Task Adding_New_Category_Should_Save_To_Db()
    {
        var category = new CategoryEntity("title");

        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        
        var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
        categories.Should().HaveCount(1);
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
        
        testOutputHelper.WriteLine($"Current Added Date: {categoryInDb.CreatedDate}");
    }

    [Fact]
    public async Task Added_New_Category_Should_Not_Have_ModifiedDate()
    {
        var category = new CategoryEntity("title 2");
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        
        var categoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);
        
        categoryInDb.Should().NotBeNull();
        categoryInDb.ModifiedDate.Should().Be(null);
    }

    //todo this method have a problem => please use another method by update category: who used table no traking
    [Fact]
    public async Task Update_Category_Should_Have_ModifiedDate()
    {
        var category = new CategoryEntity("title 3");
        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        
        var categoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);
        
        categoryInDb!.Edit("title 4");
        await _unitOfWork.CommitAsync();
        
        var editedCategoryInDb = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);
        
        editedCategoryInDb.Should().NotBeNull();
        editedCategoryInDb.ModifiedDate.Should().BeMoreThan(TimeSpan.MinValue);
    }
}
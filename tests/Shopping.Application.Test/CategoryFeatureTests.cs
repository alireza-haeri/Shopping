using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shopping.Application.Extensions;
using Shopping.Application.Features.Category.Commands;
using Shopping.Application.Features.Category.Queries;
using Shopping.Application.Repositories.Category;
using Shopping.Application.Repositories.Common;
using Shopping.Application.Test.Extensions;
using Shopping.Domain.Entities.Product;
using Xunit.Abstractions;

namespace Shopping.Application.Test
{
    /// <summary>
    /// Contains application layer tests for all features related to Categories,
    /// ensuring command and query handlers behave as expected.
    /// </summary>
    public class CategoryFeatureTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IServiceProvider _serviceProvider;
        private readonly Faker _faker;

        // --- Mocks ---
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly ICategoryRepository _categoryRepositoryMock;

        public CategoryFeatureTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _faker = new Faker();

            // Dependency Injection setup for validators
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterApplicationValidator();
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Mock setup
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
            _unitOfWorkMock.CategoryRepository.Returns(_categoryRepositoryMock);
        }

        #region CreateCategory Tests

        [Fact]
        public async Task CreateCategory_WithValidTitle_ShouldSucceedAndCommit()
        {
            //Arrange
            var command = new CreateCategoryCommand(_faker.Commerce.Categories(1).First(), null);
            var handler = new CreateCategoryCommandHandler(_unitOfWorkMock);

            //Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

            //Assertion
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().BeTrue();

            // Verify that the repository and unit of work methods were called exactly once.
            await _categoryRepositoryMock.Received(1).CreateAsync(Arg.Any<CategoryEntity>(), CancellationToken.None);
            await _unitOfWorkMock.Received(1).CommitAsync(CancellationToken.None);

            _testOutputHelper.WriteLineOperationResultErrors(result);
        }

        [Fact]
        public async Task CreateCategory_WithEmptyTitle_ShouldFailValidation()
        {
            //Arrange
            var command = new CreateCategoryCommand("", null); // Invalid title
            var handler = new CreateCategoryCommandHandler(_unitOfWorkMock);

            //Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

            //Assertion
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Key.Contains("Title"));

            // Verify that no database operations were attempted
            await _categoryRepositoryMock.DidNotReceive()
                .CreateAsync(Arg.Any<CategoryEntity>(), CancellationToken.None);
            await _unitOfWorkMock.DidNotReceive().CommitAsync(CancellationToken.None);

            _testOutputHelper.WriteLineOperationResultErrors(result);
        }

        #endregion

        #region GetAllCategories Tests

        [Fact]
        public async Task GetAllCategories_WhenCategoriesExist_ShouldReturnAllCategories()
        {
            //Arrange
            var query = new GetAllCategoryQuery();
            var categories = new List<CategoryEntity>
            {
                new(_faker.Commerce.Department(), null),
                new(_faker.Commerce.Department(), null)
            };

            _categoryRepositoryMock.GetAllAsync(CancellationToken.None).Returns(Task.FromResult(categories));
            var handler = new GetAllCategoryQueryHandler(_unitOfWorkMock);

            //Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

            //Assertion
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result.Should().HaveCount(categories.Count);
            result.Result.First().CategoryTitle.Should().Be(categories.First().Title);

            _testOutputHelper.WriteLineOperationResultErrors(result);
        }

        [Fact]
        public async Task GetAllCategories_WhenNoCategoriesExist_ShouldReturnEmptyList()
        {
            //Arrange
            var query = new GetAllCategoryQuery();

            _categoryRepositoryMock.GetAllAsync(CancellationToken.None)
                .Returns(Task.FromResult(new List<CategoryEntity>()));
            var handler = new GetAllCategoryQueryHandler(_unitOfWorkMock);

            //Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

            //Assertion
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull().And.BeEmpty();

            _testOutputHelper.WriteLineOperationResultErrors(result);
        }

        #endregion

        #region GetCategoryById Tests

        [Fact]
        public async Task GetCategoryById_WhenCategoryExists_ShouldReturnCategory()
        {
            //Arrange
            var category = new CategoryEntity(_faker.Commerce.Department(), null);
            var query = new GetByIdCategoryQuery(category.Id);

            _categoryRepositoryMock.GetByIdAsync(query.Id, CancellationToken.None)!
                .Returns(Task.FromResult<CategoryEntity?>(category));

            var handler = new GetByIdCategoryQueryHandler(_unitOfWorkMock);

            //Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

            //Assertion
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Title.Should().Be(category.Title);

            _testOutputHelper.WriteLineOperationResultErrors(result);
        }

        [Fact]
        public async Task GetCategoryById_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            //Arrange
            var query = new GetByIdCategoryQuery(Guid.NewGuid());

            // Setup mock to return null for the requested ID
            _categoryRepositoryMock.GetByIdAsync(query.Id, CancellationToken.None)!
                .Returns(Task.FromResult<CategoryEntity?>(null));

            var handler = new GetByIdCategoryQueryHandler(_unitOfWorkMock);

            //Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

            //Assertion
            result.IsSuccess.Should().BeFalse();
            result.IsNotFound.Should().BeTrue();
            result.ErrorMessages.Should().Contain(new List<KeyValuePair<string, string>>()
                { new KeyValuePair<string, string>(nameof(GetByIdCategoryQuery.Id), "Category not found") });

            _testOutputHelper.WriteLineOperationResultErrors(result);
        }

        #endregion
    }
}
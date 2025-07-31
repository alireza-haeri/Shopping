using Bogus;
using FluentAssertions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Features.Category.Commands;
using Shopping.Application.Features.Category.Queries;
using Shopping.Domain.Entities.Product;
using Shopping.Domain.Entities.User;
using Shopping.Infrastructure.Persistence.Repositories.Common;
using Xunit;
using Xunit.Abstractions;

namespace Shopping.Infrastructure.Persistence.Test
{
    /// <summary>
    /// Base class for persistence layer tests. Manages the database fixture
    /// and ensures test isolation by resetting the database and providing seeding helpers.
    /// </summary>
    public abstract class PersistenceTestBase(PersistenceTestSetup setup, ITestOutputHelper testOutputHelper)
        : IClassFixture<PersistenceTestSetup>, IAsyncLifetime
    {
        protected readonly ITestOutputHelper TestOutputHelper = testOutputHelper;
        protected readonly UnitOfWork UnitOfWork = setup.UnitOfWork;
        private readonly ShoppingDbContext _dbContext = setup.ShoppingDbContext;
        protected readonly Faker Faker = new("fa");
        protected readonly ISender Sender = setup.ServiceProvider.GetRequiredService<ISender>();

        /// <summary>
        /// Resets the database to a clean state using correct table names with schemas.
        /// </summary>
        private async Task ResetDatabaseAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [product].[Products]");
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [product].[Categories]");
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [user].[Users]");
        }

        /// <summary>
        /// Seeds a user directly into the database for tests that require a user dependency.
        /// </summary>
        protected async Task<UserEntity> SeedUserAsync()
        {
            var user = new UserEntity(Faker.Person.FirstName, Faker.Person.LastName, Faker.Person.UserName, Faker.Person.Email);
            _dbContext.Set<UserEntity>().Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task InitializeAsync()
        {
            await ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }

    public class PersistenceIntegrationTests
    {
        public class CategoryRepositoryTests(PersistenceTestSetup setup, ITestOutputHelper testOutputHelper)
            : PersistenceTestBase(setup, testOutputHelper)
        {
            [Fact]
            public async Task CreateAsync_ShouldAddCategoryToDatabase()
            {
                var category = new CategoryEntity(Faker.Commerce.Categories(1)[0]);
                await UnitOfWork.CategoryRepository.CreateAsync(category);
                await UnitOfWork.CommitAsync();
                var result = await UnitOfWork.CategoryRepository.GetByIdAsync(category.Id);
                result.Should().NotBeNull();
                result!.Title.Should().Be(category.Title);
            }
        }

        public class ProductRepositoryTests(PersistenceTestSetup setup, ITestOutputHelper testOutputHelper)
            : PersistenceTestBase(setup, testOutputHelper)
        {
            [Fact]
            public async Task GetDetailsByIdAsync_ShouldLoadRelatedEntitiesCorrectly()
            {
                // Arrange
                var user = await SeedUserAsync();
                var category = new CategoryEntity("Electronics");
                await UnitOfWork.CategoryRepository.CreateAsync(category);
                await UnitOfWork.CommitAsync();
                
                var product = ProductEntity.Create("Laptop", "Desc", 1500, 10, ProductEntity.ProductState.Active, user.Id, category.Id);
                await UnitOfWork.ProductRepository.CreateAsync(product);
                
                // Act & Assert: Check that commit does not throw an exception
                await FluentActions.Awaiting(() => UnitOfWork.CommitAsync())
                    .Should().NotThrowAsync<DbUpdateException>();

                var result = await UnitOfWork.ProductRepository.GetDetailsByIdAsync(product.Id);
                result.Should().NotBeNull();
                result!.User!.Id.Should().Be(user.Id);
                result.Category!.Id.Should().Be(category.Id);
            }

            [Fact]
            public async Task GetProductsAsync_ShouldFilterByTitle_WhenDependenciesExist()
            {
                // Arrange: Create necessary dependencies first
                var user = await SeedUserAsync();
                var category = new CategoryEntity("Tech");
                await UnitOfWork.CategoryRepository.CreateAsync(category);
                await UnitOfWork.CommitAsync();
                
                // Now, create products using the real Ids of the dependencies
                await UnitOfWork.ProductRepository.CreateAsync(ProductEntity.Create("Laptop Dell", "D", 1, 1, ProductEntity.ProductState.Active, user.Id, category.Id));
                await UnitOfWork.ProductRepository.CreateAsync(ProductEntity.Create("Laptop HP", "D", 1, 1, ProductEntity.ProductState.Active, user.Id, category.Id));
                await UnitOfWork.ProductRepository.CreateAsync(ProductEntity.Create("Mouse", "D", 1, 1, ProductEntity.ProductState.Active, user.Id, category.Id));
                await UnitOfWork.CommitAsync();
                
                // Act
                var result = await UnitOfWork.ProductRepository.GetProductsAsync("Laptop", 1, 10, null, CancellationToken.None);

                // Assert
                result.Should().HaveCount(2);
                result.Should().OnlyContain(p => p.Title.Contains("Laptop"));
            }
        }
        
        public class MediatorWithPersistenceTests(PersistenceTestSetup setup, ITestOutputHelper testOutputHelper)
            : PersistenceTestBase(setup, testOutputHelper)
        {
            [Fact]
            public async Task CreateCategoryCommand_ShouldPersistData_And_BeRetrievable()
            {
                var command = new CreateCategoryCommand("Category From Mediator");
                var result = await Sender.Send(command);
                result.IsSuccess.Should().BeTrue();
                
                var query = new GetAllCategoryQuery();
                var categories = await Sender.Send(query);
                
                categories.Result.Should().NotBeNull();
                // Assuming the DTO from GetAllCategoryQuery has a 'Title' property
                // If it's different (like 'CategoryTitle'), adjust here.
                categories.Result!.Should().ContainSingle(c => c.CategoryTitle == "Category From Mediator");
            }
        }
    }
}
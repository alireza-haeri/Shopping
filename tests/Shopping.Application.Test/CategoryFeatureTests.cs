using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shopping.Application.Common;
using Shopping.Application.Extensions;
using Shopping.Application.Features.Category.Commands;
using Shopping.Application.Features.Category.Queries;
using Shopping.Application.Features.Common;
using Shopping.Application.Repositories.Category;
using Shopping.Application.Repositories.Common;
using Shopping.Application.Test.Extensions;
using Shopping.Domain.Entities.Product;
using Xunit.Abstractions;

namespace Shopping.Application.Test;

public class CategoryFeatureTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IServiceProvider _serviceProvider;

    public CategoryFeatureTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterApplicationValidator();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Add_Category_With_Valid_Parameters_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new CreateCategoryCommand(faker.Company.CompanyName());

        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        categoryRepositoryMock
            .CreateAsync(Arg.Any<CategoryEntity>())
            .Returns(Task.FromResult);
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        unitOfWorkMock.CategoryRepository.Returns(categoryRepositoryMock);

        var validationBehavior =
            new ValidateRequestBehavior<CreateCategoryCommand, OperationResult<bool>>(_serviceProvider
                .GetRequiredService<IValidator<CreateCategoryCommand>>());
        var commandHandler = new CreateCategoryCommandHandler(unitOfWorkMock);

        //Act
        var commandResult = await validationBehavior.Handle(command, CancellationToken.None, commandHandler.Handle);

        //Assertion

        commandResult.Result.Should().Be(true);
        _testOutputHelper.WriteLineOperationResultErrors(commandResult);
    }

    [Fact]
    public async Task Get_All_Categories_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var query = new GetAllCategoryQuery();
        List<CategoryEntity> categories =
        [
            new(faker.Company.CompanyName(), null),
            new(faker.Company.CompanyName(), null),
            new(faker.Company.CompanyName(), null)
        ];

        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        categoryRepositoryMock
            .GetAllAsync()
            .Returns(Task.FromResult(categories));
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        unitOfWorkMock.CategoryRepository.Returns(categoryRepositoryMock);

        var validationBehavior =
            new ValidateRequestBehavior<GetAllCategoryQuery, OperationResult<List<GetAllCategoryQueryResult>>>(
                _serviceProvider
                    .GetRequiredService<IValidator<GetAllCategoryQuery>>());
        var queryHandler = new GetAllCategoryQueryHandler(unitOfWorkMock);

        //Act
        var queryResult = await validationBehavior.Handle(query, CancellationToken.None, queryHandler.Handle);

        //Assertion

        queryResult.IsSuccess.Should().BeTrue();
        queryResult.Result.Should().NotBeNullOrEmpty();
        _testOutputHelper.WriteLineOperationResultErrors(queryResult);
    }

    [Fact]
    public async Task Get_Category_By_Id_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        CategoryEntity category =
            new(faker.Company.CompanyName(), null);
        var query = new GetByIdCategoryQuery(category.Id);


        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        categoryRepositoryMock
            .GetByIdAsync(category.Id)!
            .Returns(Task.FromResult(category));
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        unitOfWorkMock.CategoryRepository.Returns(categoryRepositoryMock);

        var validationBehavior =
            new ValidateRequestBehavior<GetByIdCategoryQuery, OperationResult<GetByIdCategoryQueryResult>>(
                _serviceProvider
                    .GetRequiredService<IValidator<GetByIdCategoryQuery>>());
        var queryHandler = new GetByIdCategoryQueryHandler(unitOfWorkMock);

        //Act
        var queryResult = await validationBehavior.Handle(query, CancellationToken.None, queryHandler.Handle);

        //Assertion

        queryResult.IsSuccess.Should().BeTrue();
        queryResult.Result.Should().NotBeNull();
        
        _testOutputHelper.WriteLineOperationResultErrors(queryResult);
    }
    
    [Fact]
    public async Task Get_Category_By_Id_Should_Be_NotFound()
    {
        //Arrange
        var faker = new Faker();
        CategoryEntity category =
            new(faker.Company.CompanyName(), null);
        var query = new GetByIdCategoryQuery(Guid.NewGuid());


        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        
        categoryRepositoryMock
            .GetByIdAsync(category.Id)!
            .Returns(Task.FromResult(default(CategoryEntity)));
        unitOfWorkMock.CategoryRepository.Returns(categoryRepositoryMock);
        
        var queryHandler = new GetByIdCategoryQueryHandler(unitOfWorkMock);

        //Act
        var queryResult = await Helpers.ValidateAndExecuteAsync(query,queryHandler,_serviceProvider);

        //Assertion

        queryResult.IsSuccess.Should().BeFalse();
        queryResult.IsNotFound.Should().BeTrue();
        queryResult.Result.Should().BeNull();
        
        _testOutputHelper.WriteLineOperationResultErrors(queryResult);
    }
}
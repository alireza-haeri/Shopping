namespace Shopping.Application.Features.Category.Queries;

public record GetByIdCategoryQueryResult(string Title, Guid? ParentId = null);
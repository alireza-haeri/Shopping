namespace Shopping.Application.Features.Category.Queries;

public record GetAllCategoryQueryResult(Guid CategoryId,string CategoryTitle,Guid? ParentId = null);
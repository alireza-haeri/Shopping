using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Shopping.Application.Features.Category.Commands;
using Shopping.Application.Features.Category.Queries;
using Shopping.WebFramework.Common;
using Shopping.WebFramework.Models;

namespace Shopping.Api.Controllers.V1;

/// <summary>
/// Category Controller
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("/api/v{version:apiVersion}/Categories")]
public class CategoryController(ISender sender):BaseController
{
    /// <summary>
    /// Create a Category
    /// </summary>
    /// <param name="command">Create Command</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpPost("CreateCategory")]
    [ProducesResponseType<ApiResult>(200)]
    public virtual async Task<IActionResult> CreateCategory(CreateCategoryCommand command,CancellationToken cancellationToken)
    => base.OperationResult(await sender.Send(command, cancellationToken));

    /// <summary>
    /// Get a Category By ID
    /// </summary>
    /// <param name="categoryId">Category Id</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("GetCategoryById/{categoryId}")]
    [ProducesResponseType<ApiResult<GetByIdCategoryQueryResult>>(200)]
    public virtual async Task<IActionResult> GetAllCategoryById([FromRoute(Name = "categoryId")]Guid categoryId,CancellationToken cancellationToken)
    => base.OperationResult(await sender.Send(new GetByIdCategoryQuery(categoryId), cancellationToken));
    
    /// <summary>
    /// Get All Categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("GetAllCategories")]
    [ProducesResponseType<ApiResult<List<GetAllCategoryQueryResult>>>(200)]
    public virtual async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
    => base.OperationResult(await sender.Send(new GetAllCategoryQuery(), cancellationToken));
}
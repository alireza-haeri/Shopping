using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Api.Models.Product;
using Shopping.Application.Features.Product.Commands;
using Shopping.Application.Features.Product.Queries;
using Shopping.WebFramework.Common;
using Shopping.WebFramework.Models;

namespace Shopping.Api.Controllers.V1;

/// <summary>
/// Product Contorller
/// </summary>
/// <param name="sender"></param>
[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Products")]
public class ProductController(ISender sender) : BaseController
{
    /// <summary>
    /// Create a Product
    /// </summary>
    /// <param name="request">Create Product Request</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpPost("CreateProduct")]
    [ProducesResponseType<ApiResult>(200)]
    [Authorize]
    public virtual async Task<IActionResult> CreateProduct(CreateProductRequest request,
        CancellationToken cancellationToken) =>
        base.OperationResult(await sender.Send(
            new CreateProductCommand(UserId!.Value, request.CategoryId, request.Title, request.Description,
                request.Price, request.Quantity, request.State, request.Images), cancellationToken));

    /// <summary>
    /// Delete a Product
    /// </summary>
    /// <param name="productId">Product Id</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpDelete("DeleteProduct/{productId:guid}")]
    [ProducesResponseType<ApiResult>(200)]
    [Authorize]
    public virtual async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken) =>
        base.OperationResult(await sender.Send(new DeleteProductCommand(productId), cancellationToken));

    /// <summary>
    /// Edit a Product
    /// </summary>
    /// <param name="command">Edit Command</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpPut("UpdateProduct")]
    [ProducesResponseType<ApiResult>(200)]
    [Authorize]
    public virtual async Task<IActionResult> EditProduct(EditProductCommand command,
        CancellationToken cancellationToken) =>
        base.OperationResult(await sender.Send(command, cancellationToken));

    /// <summary>
    /// Get a Product By Id
    /// </summary>
    /// <param name="productId">Product Id</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("GetProductDetailById/{productId:guid}")]
    [ProducesResponseType<ApiResult>(200)]
    public virtual async Task<IActionResult>
        GetProductDetailById(Guid productId, CancellationToken cancellationToken) =>
        base.OperationResult(await sender.Send(new GetProductDetailByIdQuery(productId), cancellationToken));

    /// <summary>
    /// Get Products
    /// </summary>
    /// <param name="title">Product Title (Min length is 3)</param>
    /// <param name="currentPage">Current Page</param>
    /// <param name="pageCount">Page Count</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <param name="categoryId">Category Id (Optional)</param>
    /// <returns></returns>
    [HttpGet("GetProduct/{title:minlength(3)}/{currentPage:int}/{pageCount:int}")]
    [ProducesResponseType<ApiResult<List<GetProductsQueryResult>>>(200)]
    public virtual async Task<IActionResult> GetProducts(
        string title,
        int currentPage,
        int pageCount,
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken) =>
        base.OperationResult(await sender.Send(new GetProductsQuery(title, currentPage, pageCount, categoryId),
            cancellationToken));
}
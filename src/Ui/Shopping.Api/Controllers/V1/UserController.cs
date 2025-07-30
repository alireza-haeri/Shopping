using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Shopping.Application.Contracts.User.Models;
using Shopping.Application.Features.User.Commands.Register;
using Shopping.Application.Features.User.Queries.PasswordLogin;
using Shopping.WebFramework.Common;
using Shopping.WebFramework.Models;

namespace Shopping.Api.Controllers.V1;

/// <summary>
/// User Endpoints
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Users")]
public class UserController(ISender sender) : BaseController
{
    /// <summary>
    /// Register User
    /// </summary>
    /// <param name="command">Register Command</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No Result</returns>
    [HttpPost("Register")]
    [ProducesResponseType<ApiResult>(200)]
    public virtual async Task<IActionResult> Register(RegisterUserCommand command,CancellationToken cancellationToken) =>
        base.OperationResult(await sender.Send(command, cancellationToken));

    /// <summary>
    /// Get Token With Password
    /// </summary>
    /// <param name="query">Token Request Query</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpPost("TokenRequest")]
    [ProducesResponseType<ApiResult<JwtAccessTokenModel>>(200)]
    public virtual async Task<IActionResult> TokenRequest(UserPasswordLoginQuery query,CancellationToken cancellationToken)=>
    base.OperationResult(await sender.Send(query, cancellationToken));
}
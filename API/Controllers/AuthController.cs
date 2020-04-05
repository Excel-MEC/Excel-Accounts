using System.Threading.Tasks;
using API.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using API.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [SwaggerTag("The Routes under this controller do not need authorization.")]
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [SwaggerOperation(Description = "For login, Pass the access token recieved from auth0. Store the jwt recieved in return. Pass it as the authorization header value in the format \"Bearer jwt\" to access the endpoints that need authorization")]
        [HttpPost("login")]
        public async Task<ActionResult<JwtForClientDto>> Login(TokenForLoginDto tokenForLogin)
        {
            var responseInJson = await _authService.FetchUserFromAuth0(tokenForLogin.auth_token);
            var token = await _authService.CreateJwtForClient(responseInJson);
            return Ok(new { token = token });
        }
    }
}
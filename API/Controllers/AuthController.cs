using System.Threading.Tasks;
using API.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using API.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Google.Apis.Auth;

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

        [SwaggerOperation(Description = "For login, Pass the tokenId received from GoogleOAuth2.0 API. Store the jwt recieved in return. Pass it as the authorization header value in the format \"Bearer jwt\" to access the endpoints that need authorization")]
        [HttpPost("login")]
        public async Task<ActionResult<JwtForClientDto>> Login(TokenForLoginDto tokenForLogin)
        {
            
            // var payload = GoogleJsonWebSignature.ValidateAsync(tokenForLogin.tokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;
            var responseInJson = await _authService.FetchUserGoogle0Auth(tokenForLogin.accessToken);
            var token = await _authService.CreateJwtForClient(responseInJson, tokenForLogin.referralCode);
            return Ok(new JwtForClientDto { Token = token });
        }
    }
}
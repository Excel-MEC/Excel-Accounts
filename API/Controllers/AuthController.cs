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
        private readonly IAuthService _AuthService;
        private readonly IAuthService2 _AuthService2;
        public AuthController(IAuthService AuthService, IAuthService2 AuthService2)
        {
            _AuthService2 = AuthService2;
            _AuthService = AuthService;
        }

        [SwaggerOperation(Description = "For login, Pass the access token recieved from auth0. Store the jwt recieved in return. Pass it as the authorization header value in the format \"Bearer jwt\" to access the endpoints that need authorization")]
        [HttpPost("login2")]
        public async Task<ActionResult<JwtForClientDto>> Login2(TokenForLogin2Dto tokenForLogin)
        {
            var responseInJson = await _AuthService2.FetchUserFromAuth0(tokenForLogin.auth_token);
            var token = await _AuthService2.CreateJwtForClient(responseInJson, tokenForLogin.referralCode);
            return Ok(new JwtForClientDto { Token = token });
        }

        [SwaggerOperation(Description = "For login, Pass the tokenId recieved from GoogleOAuth2.0 API. Store the jwt recieved in return. Pass it as the authorization header value in the format \"Bearer jwt\" to access the endpoints that need authorization")]
        [HttpPost("login")]
        public async Task<ActionResult<JwtForClientDto>> Login(TokenForLoginDto tokenForLogin)
        {
            var payload = GoogleJsonWebSignature.ValidateAsync(tokenForLogin.tokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;
            var token = await _AuthService.CreateJwtForClient(payload, tokenForLogin.referralCode);
            return Ok(new JwtForClientDto { Token = token });
        }
    }
}
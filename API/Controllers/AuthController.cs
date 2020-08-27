using System;
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
        public AuthController(IAuthService AuthService)
        {
            _AuthService = AuthService;
        }


        [SwaggerOperation(Description = "For login, Pass the tokenId received from GoogleOAuth2.0 API. Store the jwt received in return. Pass it as the authorization header value in the format \"Bearer jwt\" to access the endpoints that need authorization")]
        [HttpPost("login")]
        public async Task<ActionResult<JwtForClientDto>> Login(TokenForLoginDto tokenForLogin)
        {
            var responseInJson = await _AuthService.FetchUserGoogle0Auth(tokenForLogin.accessToken);
            var token = await _AuthService.CreateJwtForClient(responseInJson, tokenForLogin.referralCode);
            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<JwtForClientDto>> Refresh(TokenForRefreshDto data)
        {
            var token = await _AuthService.CreateJwtFromRefreshToken(data.refreshToken);
            return Ok(new JwtForClientDto { AccessToken = token, RefreshToken = data.refreshToken});
        }
    }
}
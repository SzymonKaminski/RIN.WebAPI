using FauFau.Net.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RIN.Core;
using RIN.Core.DB;
using RIN.WebAPI.Models;
using System.Web;

namespace RIN.WebAPI.Utils
{
    public class R5SigAuthRequiredAttribute : TypeFilterAttribute
    {
        public R5SigAuthRequiredAttribute() : base(typeof(R5SigAuth))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class R5SigAuth : Attribute, IAsyncAuthorizationFilter
    {
        private WebApiConfigSettings Config;
        private DB Db;

        public R5SigAuth(IOptions<WebApiConfigSettings> config, DB db)
        {
            Config = config.Value;
            Db     = db;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!Config.VerifyRed5Sig)
                return;

            var header = context.HttpContext.Request.GetSingleHeader(Auth.SIG_HEADER_NAME);
            if (header == null)
            {
                context.Result = CreateError(Error.Codes.ERR_INCORRECT_USERPASS, "No Signature");
                return;
            }

            // TODO: Cache the users uuid to account id to skip a db call
            var uid         = HttpUtility.UrlDecode(Red5Sig.ParseString(header).UID.ToString());
            var loginResult = await Db.GetLoginData(uid);
            if (loginResult == null)
            {
                context.Result = CreateError(Error.Codes.ERR_INCORRECT_USERPASS);
                return;
            }

            var result = await VerifyRequest(context.HttpContext.Request, loginResult.secret, Config.Red5SigMaxClockSkew);
            if (result != Red5Sig.VerifyResult.Valid)
                context.Result = CreateError(Error.Codes.ERR_INCORRECT_USERPASS);
        }

        // Checks the token, the body hash and the time of the signature, the body stays readable for the action
        public static async Task<Red5Sig.VerifyResult> VerifyRequest(HttpRequest request, string secret, TimeSpan maxClockSkew)
        {
            var header = request.GetSingleHeader(Auth.SIG_HEADER_NAME);
            if (header == null)
                return Red5Sig.VerifyResult.Malformed;

            request.EnableBuffering();
            using var body = new MemoryStream();
            await request.Body.CopyToAsync(body);
            request.Body.Position = 0;

            return Red5Sig.Verify(secret, header, body.GetBuffer().AsSpan(0, (int)body.Length), DateTimeOffset.UtcNow, maxClockSkew);
        }

        private ObjectResult CreateError(string code, string msg = null)
        {
            var error         = new Error(code, msg);
            var result        = new ObjectResult(error);
            result.StatusCode = 500;

            return result;
        }
    }
}

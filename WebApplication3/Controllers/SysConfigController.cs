using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models;


namespace WebApplication3.Controllers;

[Route("[controller]")]
[ApiController]
public class SysConfigController: ControllerBase
{
    [Authorize(true),HttpGet("GetConfig")]
    public Dictionary<string, object> GetConfig()
    {
        Dictionary<string, object> config = new Dictionary<string, object>
        {
            {"AIKey",ConfigHelper.GetString("AIKey")},
            {"DBIP",ConfigHelper.GetString("DBIP")},
            {"DBNAME",ConfigHelper.GetString("DBNAME")},
            {"DBPASSWORD",ConfigHelper.GetString("DBPASSWORD")},
            {"DBUSER",ConfigHelper.GetString("DBUSER")},
            {"DevMode",ConfigHelper.GetString("DevMode")},
            {"RedisIP",ConfigHelper.GetString("RedisIP")},
            {"RedisPassword",ConfigHelper.GetString("RedisPassword")},
            {"RedisPort",ConfigHelper.GetString("RedisPort")},
            {"SmtpPass",ConfigHelper.GetString("SmtpPass")},
            {"SmtpPort",ConfigHelper.GetString("SmtpPort")},
            {"SmtpServer",ConfigHelper.GetString("SmtpServer")},
            {"SmtpUser",ConfigHelper.GetString("SmtpUser")},
            {"TokenAudience",ConfigHelper.GetString("TokenAudience")},
            {"TokenExpirationHours",ConfigHelper.GetString("TokenExpirationHours")},
            {"TokenIssuer",ConfigHelper.GetString("TokenIssuer")},
            {"TokenKey",ConfigHelper.GetString("TokenKey")},
        };
        return config;
    }
}
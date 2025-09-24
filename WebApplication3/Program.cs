using FreeSql;
using Lazy.Captcha.Core;
using Lazy.Captcha.Core.Generator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Net;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Helper;
using WebApplication3.Foundation.Middleware;
using WebApplication3.Middleware;

namespace WebApplication3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(); // 添加 Session 服务
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            NLogHelper.Debug("系统启动中....");

            // 配置转发头：不限制层数，但只信任你的代理来源（请根据实际网段/IP填写）
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = null; // 不限制代理层数
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("0.0.0.0"), 0)); // 全部 IPv4
                options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("::"), 0));      // 全部 IPv6

                // 强烈建议：配置你基础设施内的代理网段/IP（示例，按需修改/追加）
                // options.KnownProxies.Add(IPAddress.Parse("203.0.113.10"));
                // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
                // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
                // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("192.168.0.0"), 16));

                // 如果无法枚举代理来源，请勿清空 KnownProxies/Networks 并信任所有来源，这将非常不安全。
            });

            builder.Services.AddCaptcha(builder.Configuration, option =>
            {
                option.CaptchaType = CaptchaType.NUMBER; // 验证码类型
                option.CodeLength = 4; // 验证码长度, 要放在CaptchaType设置后.  当类型为算术表达式时，长度代表操作的个数
                option.ExpirySeconds = 30 * 60; // 验证码过期时间
                option.IgnoreCase = true; // 比较时是否忽略大小写
                option.StoreageKeyPrefix = ""; // 存储键前缀

                option.ImageOption.Animation = false; // 是否启用动画
                option.ImageOption.FrameDelay = 30; // 每帧延迟,Animation=true时有效, 默认30

                option.ImageOption.Width = 100; // 验证码宽度
                option.ImageOption.Height = 50; // 验证码高度
                option.ImageOption.BackgroundColor = SkiaSharp.SKColors.White; // 验证码背景色

                option.ImageOption.BubbleCount = 2; // 气泡数量
                option.ImageOption.BubbleMinRadius = 5; // 气泡最小半径
                option.ImageOption.BubbleMaxRadius = 15; // 气泡最大半径
                option.ImageOption.BubbleThickness = 1; // 气泡边沿厚度

                option.ImageOption.InterferenceLineCount = 5; // 干扰线数量

                option.ImageOption.FontSize = 36; // 字体大小
                option.ImageOption.FontFamily = DefaultFontFamilys.Instance.Actionj; // 字体

                /*
                 * 中文使用kaiti，其他字符可根据喜好设置（可能部分转字符会出现绘制不出的情况）。
                 * 当验证码类型为“ARITHMETIC”时，不要使用“Ransom”字体。（运算符和等号绘制不出来）
                 */

                option.ImageOption.TextBold = true; // 粗体，该配置2.0.3新增
            });
            // 添加以下配置
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // 保持与原始代码相同的JSON配置
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            builder.Services.AddMemoryCache();
            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlerMiddleware>();


            app.UseRequestLogging();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // 注意顺序：先处理转发头，后续中间件才能拿到真实 IP 和协议
            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            // 你的中间件此时使用 context.Connection.RemoteIpAddress 即为真实客户端 IP
            app.UseWorkViewCountRedis();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("AllowAll");

            app.MapControllerRoute(
                name: "default",
                pattern: "{action=Index}/{id?}",
                defaults: new { controller = "Home" }
            );

            app.Run();
        }
    }
}

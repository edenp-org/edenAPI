using FreeSql;
using Lazy.Captcha.Core.Generator;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebApplication3.Foundation.Helper;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using WebApplication3.Foundation.Middleware;

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
            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            // 使用 CORS 中间件
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

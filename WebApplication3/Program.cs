using FreeSql;
using Lazy.Captcha.Core.Generator;
using Lazy.Captcha.Core;
using WebApplication3.Foundation.Helper;

namespace WebApplication3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(); // ��� Session ����
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
            // ���� FreeSql
            //builder.Services.AddSingleton<IFreeSql>(new FreeSqlBuilder()
            //    .UseConnectionString(DataType.SqlServer, builder.Configuration.GetConnectionString("DefaultConnection"))
            //    .UseAutoSyncStructure(true) // �Զ�ͬ��ʵ��ṹ�����ݿ�
            //    .Build());
            builder.Services.AddCaptcha(builder.Configuration, option =>
            {
                option.CaptchaType = CaptchaType.NUMBER; // ��֤������
                option.CodeLength = 4; // ��֤�볤��, Ҫ����CaptchaType���ú�.  ������Ϊ�������ʽʱ�����ȴ�������ĸ���
                option.ExpirySeconds = 30*60; // ��֤�����ʱ��
                option.IgnoreCase = true; // �Ƚ�ʱ�Ƿ���Դ�Сд
                option.StoreageKeyPrefix = ""; // �洢��ǰ׺

                option.ImageOption.Animation = false; // �Ƿ����ö���
                option.ImageOption.FrameDelay = 30; // ÿ֡�ӳ�,Animation=trueʱ��Ч, Ĭ��30

                option.ImageOption.Width = 100; // ��֤����
                option.ImageOption.Height = 50; // ��֤��߶�
                option.ImageOption.BackgroundColor = SkiaSharp.SKColors.White; // ��֤�뱳��ɫ

                option.ImageOption.BubbleCount = 2; // ��������
                option.ImageOption.BubbleMinRadius = 5; // ������С�뾶
                option.ImageOption.BubbleMaxRadius = 15; // �������뾶
                option.ImageOption.BubbleThickness = 1; // ���ݱ��غ��

                option.ImageOption.InterferenceLineCount = 5; // ����������

                option.ImageOption.FontSize = 36; // �����С
                option.ImageOption.FontFamily = DefaultFontFamilys.Instance.Actionj; // ����

                /* 
                 * ����ʹ��kaiti�������ַ��ɸ���ϲ�����ã����ܲ���ת�ַ�����ֻ��Ʋ������������
                 * ����֤������Ϊ��ARITHMETIC��ʱ����Ҫʹ�á�Ransom�����塣��������͵ȺŻ��Ʋ�������
                 */

                option.ImageOption.TextBold = true;// ���壬������2.0.3����
            });

            var app = builder.Build();

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
            // ʹ�� CORS �м��
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

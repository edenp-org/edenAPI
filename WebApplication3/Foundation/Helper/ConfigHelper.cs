using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;

using Microsoft.Extensions.Hosting.Internal;

namespace WebApplication3.Foundation.Helper
{
    public static class ConfigHelper
    {
        private static readonly DefaultConfiguration _configuration;
        static string Url = AppContext.BaseDirectory + "config.COIN";
        static ConfigHelper()
        {
            if (!System.IO.File.Exists(Url))
            {
                System.IO.File.Create(Url).Close();
            }
            _configuration = DefaultConfiguration.FromFile(Url);
        }

        /// <summary>
        /// 获取配置文件中的字符串值
        /// </summary>
        /// <param name="key">配置项的键</param>
        /// <returns>配置项的值</returns>
        public static string GetString(string key)
        {
            return DESHeper.DecryptDES(_configuration[key] ?? string.Empty,"test465456456456456412341985641563145");
        }

        /// <summary>
        /// 获取配置文件中的整数值
        /// </summary>
        /// <param name="key">配置项的键</param>
        /// <param name="key">配置项的键</param>
        /// <returns>配置项的整数值</returns>
        public static int GetInt(string key)
        {
            if (int.TryParse(DESHeper.DecryptDES(_configuration[key], "test465456456456456412341985641563145"), out int result))
            {
                return result;
            }
            throw new FormatException($"键 '{key}' 不是一个有效的整数。");
        }

        /// <summary>
        /// 获取配置文件中的布尔值
        /// </summary>
        /// <param name="key">配置项的键</param>
        /// <returns>配置项的布尔值</returns>
        public static bool GetBool(string key)
        {
            if (bool.TryParse(DESHeper.DecryptDES(_configuration[key], "test465456456456456412341985641563145"), out bool result))
            {
                return result;
            }
            throw new FormatException($"键 '{key}' 不是一个有效的布尔值。");
        }

        /// <summary>
        /// 设置配置文件中的值
        /// </summary>
        /// <param name="key">配置项的键</param>
        /// <param name="value">配置项的值</param>
        public static void SetValue(string key, string value)
        {
            _configuration[key] = DESHeper.EncryptDES(value, "test465456456456456412341985641563145");
        }
    }
}

using Newtonsoft.Json;
using System.IO;

namespace MHYStart.Programes
{
    public class MyConfige
    {
        readonly static string configJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="confige">泛型参数</param>
        /// <returns></returns>
        public static void LoadConfig<T>(out T confige) where T : class, new()
        {
            try
            {
                if (File.Exists(configJsonPath))
                {
                    string configJson = File.ReadAllText(configJsonPath);
                    if (!string.IsNullOrWhiteSpace(configJson))
                    {
                        var locaiConfig = JsonConvert.DeserializeObject<T>(configJson);
                        if (locaiConfig != null)
                        {
                            confige = locaiConfig;
                            return;
                        }
                    }
                }
                confige = new T();
                SaveConfig(confige);
            }
            catch (Exception ex)
            {
                MyLog.Error("读取配置错误", ex);
                confige = new T();
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        public static void SaveConfig<T>(T confige)
        {
            try
            {
                File.WriteAllText(configJsonPath, JsonConvert.SerializeObject(confige));
            }
            catch (Exception ex)
            {
                MyLog.Error("保存配置错误", ex);
            }
        }
    }
}

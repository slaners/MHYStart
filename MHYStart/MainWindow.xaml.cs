using MHYStart.Programes;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MHYStart
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MyConfige.LoadConfig(out confige);
        }
        /*
         * 根目录HKEY_CURRENT_USER\SOFTWARE\miHoYo\
         * 账户信息 游戏名称内
         * 安装目录 HYP\1_1 
         *  hk4e_cn 原神
         *  hkrpg_cn 星铁
         *  nap_cn 绝区零
         */

        Saber.GameTag gameTagInfo = new(string.Empty, string.Empty, string.Empty);
        private MainWindowViewModel? mVVM;
        private readonly Saber.UserConfige confige;
        private readonly Dictionary<string, Saber.AudioInfo> sysAudioInfo = [];
        private DispatcherTimer? timer;
        [GeneratedRegex(@"[^\\]+(?=\.exe)")]
        private static partial Regex MyRegex();


        readonly string system = "system";
        readonly string ys = MyResource.StartFile_YS;
        readonly string hsr = MyResource.StartFile_Hsr;
        readonly string zzz = MyResource.StartFile_ZZZ;

        //启动事件
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mVVM = DataContext as MainWindowViewModel;
                SwitchGame(sender, e);

                System_Slider.Maximum = 100;
                YS_Slider.Maximum = 100;
                SR_Slider.Maximum = 100;
                ZZZ_Slider.Maximum = 100;

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                MyLog.Error("启动", ex);
            }
        }

        /// <summary>
        /// 计时事件-遍历音频音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var device = GetDefaultAudioDevice();
                var volume = device.AudioEndpointVolume;
                float masterVolumePercent = volume.MasterVolumeLevelScalar;
                AddAudioInfo(system, masterVolumePercent, volume.Mute);
                var sessionManager = device.AudioSessionManager;//音频会话管理器
                for (int i = 0; i < sessionManager.Sessions.Count; i++)
                {
                    AudioSessionControl session = sessionManager.Sessions[i];//会话
                    //sysAudioInfo.Add(match.Value, new SaClass.AudioInfo { Live = session.SimpleAudioVolume.Volume, IsMute = session.SimpleAudioVolume.Mute });
                    AddAudioInfo(GetSoftwareName(session), session.SimpleAudioVolume.Volume, session.SimpleAudioVolume.Mute);
                }
                //设置音量显示
                foreach (var item in sysAudioInfo)
                {

                    Dispatcher.Invoke(() =>
                    {
                        if (item.Key == system)
                        {
                            System_Slider.Value = item.Value.Live * 100;
                            System_MuteToggle.IsChecked = item.Value.IsMute;
                        }
                        else if (item.Key == ys)
                        {
                            YS_Slider.Value = item.Value.Live * 100;
                            YS_MuteToggle.IsChecked = item.Value.IsMute;
                        }
                        else if (item.Key == hsr)
                        {
                            SR_Slider.Value = item.Value.Live * 100;
                            SR_MuteToggle.IsChecked = item.Value.IsMute;
                        }
                        else if (item.Key == zzz)
                        {
                            ZZZ_Slider.Value = item.Value.Live * 100;
                            ZZZ_MuteToggle.IsChecked = item.Value.IsMute;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MyLog.Error("计时遍历音频", ex.Message);
            }
        }

        /// <summary>
        /// 切换游戏
        /// </summary>
        private void SwitchGame(object sender, RoutedEventArgs e)
        {
            mVVM?.Users.Clear();
            string gameTag = string.Empty;
            if (Genshin_Rbtn.IsChecked == true)
            {
                gameTag = "ys";
            }
            else if (Hsr_Rbtn.IsChecked == true)
            {
                gameTag = "hsr";
            }
            else if (ZZZ_Rbtn.IsChecked == true)
            {
                gameTag = "zzz";
            }
            gameTagInfo = Saber.GameTagInfo.GameTags[gameTag];
            var configeRes = confige?.games.FirstOrDefault(item => item.GameName == gameTagInfo.Name);
            if (configeRes != null)//有游戏配置信息
            {
                foreach (var item in configeRes.Users)
                {
                    mVVM?.Users.Add(item);
                }
            }
            var res = MyRegedit.Read(MyResource.GameRootReg + @"HYP\1_1\" + gameTagInfo.Tag, "GameInstallPath");//读取注册表游戏路径
            if (res is string gamePath)
            {
                if (mVVM != null)
                {
                    mVVM.GameInfo.GamePath = gamePath;
                    mVVM.GameInfo.GameName = gameTagInfo.Name;
                }
            }
            else
            {
                MessageBox.Show($"未获取到{gameTagInfo.Name}的目录", "错误：", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //选择路径
        private void SelectPath_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() != true) return;
            MyLog.Debug(openFileDialog.FileName);
            FilePath_Tbox.Text = openFileDialog.FileName;
        }

        //双击启动
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // 处理双击事件的逻辑
                ListViewItem item = (ListViewItem)sender;
                // 在这里处理双击事件的逻辑
                if (item.DataContext is Saber.User user)
                {
                    //写到注册表
                    byte[] bytes = Encoding.UTF8.GetBytes(user.Key);
                    string regPath = $"{MyResource.GameRootReg}{gameTagInfo.Name}";
                    MyRegedit.Set(regPath, MyResource.LoginCred, bytes);
                    string startFilePath = Path.Combine(FilePath_Tbox.Text, gameTagInfo.GameFile) + ".exe";
                    if (!File.Exists(startFilePath))
                    {
                        MessageBox.Show($"启动文件 {startFilePath} 不存在。");
                        return;
                    }
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = startFilePath,
                        UseShellExecute = true,
                    };
                    Process.Start(startInfo);
                }
            }
        }

        //保存
        private void Save_Btn_Click(object sender, RoutedEventArgs e)
        {
            var (IsOk, name) = ShowInputWindow();
            if (IsOk) mVVM?.Users.Add(new() { Name = name, Key = Encoding.UTF8.GetString(GetRegeditUserValue()) });
            SaveConfige();
        }

        //修改
        private void Update_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (Users_ListView.SelectedItem is Saber.User selectItem)
            {
                var (IsOk, name) = ShowInputWindow(selectItem.Name);
                if (IsOk)
                {
                    selectItem.Name = name;
                    //selectItem.Key = Encoding.UTF8.GetString(GetRegeditUserValue());
                    SaveConfige();
                }
            }
            else
            {
                MyLog.ShowMessage("未选中用户信息。");
            }
        }

        //删除
        private void Delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (Users_ListView.SelectedItem is Saber.User selectItem)
            {
                mVVM?.Users?.Remove(selectItem);
                SaveConfige();
            }
            else
            {
                MyLog.ShowMessage("未选中用户信息。");
            }
        }

        /// <summary>
        /// 显示输入窗口
        /// </summary>
        /// <returns></returns>
        private (bool IsOk, string name) ShowInputWindow(string name = "")
        {
            var inputWindow = new InputWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,//父窗口居中
                InputName = name
            };
            var res = inputWindow.ShowDialog();
            if (res == true) return (true, inputWindow.InputName);
            return (false, string.Empty);
        }

        /// <summary>
        /// 获取游戏账户值
        /// </summary>
        /// <returns></returns>
        private byte[] GetRegeditUserValue()
        {
            string regPath = @$"{MyResource.GameRootReg}{gameTagInfo.Name}";
            var userReg = MyRegedit.Read(regPath, MyResource.LoginCred);
            if (userReg is byte[] byteArray)
            {
                return byteArray;
            }
            return [];
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfige()
        {
            var users = mVVM?.Users.ToList();//获取当前用户集合
            var configeRes = confige?.games.FirstOrDefault(item => item.GameName == gameTagInfo.Name);
            if (configeRes != null)
            {
                configeRes.Users = users ?? [];
            }
            else
            {
                confige?.games.Add(new() { GameName = gameTagInfo.Name, Users = users ?? [] });
            }
            MyConfige.SaveConfig(confige);
        }



        /// <summary>
        /// 新增/更新音频信息
        /// </summary>
        /// <param name="key">字典键</param>
        /// <param name="live">音量</param>
        /// <param name="isMute">是否静音</param>
        private void AddAudioInfo(string key, float live, bool isMute)
        {
            if (sysAudioInfo.ContainsKey(key))
            {
                sysAudioInfo[key] = new Saber.AudioInfo { Live = live, IsMute = isMute };
            }
            else
            {
                sysAudioInfo.Add(key, new Saber.AudioInfo { Live = live, IsMute = isMute });
            }
        }

        /// <summary>
        /// 系统音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void System_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var device = GetDefaultAudioDevice();
            device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)System_Slider.Value / 100;
        }

        /// <summary>
        /// 原神音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YS_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetAudioVolume(ys, (float)YS_Slider.Value);
        }

        /// <summary>
        /// 星铁音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SR_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetAudioVolume(hsr, (float)SR_Slider.Value);
        }

        /// <summary>
        /// 绝区零音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZZZ_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetAudioVolume(zzz, (float)ZZZ_Slider.Value);
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="appName">软件名称</param>
        /// <param name="volume">音量</param>
        private static void SetAudioVolume(string appName, float volume)
        {
            try
            {
                var session = GetDeviceSession(appName);
                if (session != null)
                {
                    session.Volume = volume / 100;
                }
                else
                {
                    MyLog.Error("设置音量", $"未获取到{appName}的音频对话");
                }
            }
            catch (Exception ex)
            {
                MyLog.Error("设置音量", ex.Message);
            }
        }

        /// <summary>
        /// 静音-系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void System_MuteToggle_Click(object sender, RoutedEventArgs e)
        {
            var device = GetDefaultAudioDevice();
            device.AudioEndpointVolume.Mute = System_MuteToggle.IsChecked ?? false;
        }

        /// <summary>
        /// 静音-原神
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YS_MuteToggle_Click(object sender, RoutedEventArgs e)
        {
            SetMute(ys, YS_MuteToggle.IsChecked);
        }

        /// <summary>
        /// 静音-星铁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SR_MuteToggle_Click(object sender, RoutedEventArgs e)
        {
            SetMute(hsr, SR_MuteToggle.IsChecked);
        }

        /// <summary>
        /// 静音-绝区零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZZZ_MuteToggle_Click(object sender, RoutedEventArgs e)
        {
            SetMute(zzz, ZZZ_MuteToggle.IsChecked);
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        /// <param name="appName"></param>
        private static void SetMute(string appName, bool? isMute)
        {
            try
            {
                var session = GetDeviceSession(appName);
                if (session != null)
                {
                    session.Mute = isMute ?? false;
                }
                else
                {
                    MyLog.Error("设置静音", $"未获取到{appName}的音频对话");
                }
            }
            catch (Exception ex)
            {
                MyLog.Error("设置静音", ex.Message);
            }
        }

        /// <summary>
        /// 获取默认音频设备
        /// </summary>
        /// <returns></returns>
        private static MMDevice GetDefaultAudioDevice()
        {
            var devEnum = new MMDeviceEnumerator();
            return devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);//获取默认音频终结点
        }

        /// <summary>
        /// 获取音频会话
        /// </summary>
        /// <param name="appName">音频名称</param>
        /// <returns></returns>
        private static SimpleAudioVolume? GetDeviceSession(string appName)
        {
            var device = GetDefaultAudioDevice();
            var sessionManager = device.AudioSessionManager;//音频会话管理器
            for (int i = 0; i < sessionManager.Sessions.Count; i++)
            {
                AudioSessionControl session = sessionManager.Sessions[i];//会话
                //AddAudioInfo(match.Value, session.SimpleAudioVolume.Volume, session.SimpleAudioVolume.Mute);
                if (GetSoftwareName(session) == appName) return session.SimpleAudioVolume;
            }
            return default;
        }

        /// <summary>
        /// 获取软件名称
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static string GetSoftwareName(AudioSessionControl session)
        {
            var regex = MyRegex();
            var match = regex.Match(session.GetSessionIdentifier);//正则获取软件名称
            return match.Value;
        }

        /// <summary>
        /// 根据鼠标位置设置音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                // 获取鼠标点击的位置
                Point clickPoint = e.GetPosition(slider);

                // 计算对应的值
                double value = clickPoint.X / slider.ActualWidth * (slider.Maximum - slider.Minimum) + slider.Minimum;

                // 更新 Slider 的值
                slider.Value = value;
            }
        }
    }
}
using System.ComponentModel;
using static MHYStart.Programes.Saber;

namespace MHYStart.Programes
{
    public class Saber
    {

        public class UserConfige
        {
            public List<Game> games = [];
        }

        public class Game
        {
            public string GameName = string.Empty;
            public string GamePath = string.Empty;
            public List<User> Users = [];
        }
        public class GameInfo : INotifyPropertyChanged
        {
            string _gameName = string.Empty;
            string _gamePath = string.Empty;
            public string GameName { get => _gameName; set => SetField(ref _gameName, value, nameof(GameName)); }
            public string GamePath { get => _gamePath; set => SetField(ref _gamePath, value, nameof(GamePath)); }
            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            private bool SetField<T>(ref T field, T value, string propertyName)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropChanged(propertyName);
                return true;
            }
        }
        public class User : INotifyPropertyChanged
        {
            private string _name = string.Empty;
            private string _key = string.Empty;

            public string Name { get => _name; set => SetField(ref _name, value, nameof(Name)); }

            public string Key { get => _key; set => SetField(ref _key, value, nameof(Key)); }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            private bool SetField<T>(ref T field, T value, string propertyName)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropChanged(propertyName);
                return true;
            }
        }

        public class AudioInfo
        {
            public float Live { get; set; }
            public bool IsMute { get; set; }
        }

        /// <summary>
        /// 记录游戏标记信息
        /// </summary>
        public static class GameTagInfo
        {
            static GameTagInfo()
            {
                GameTags = new()
                {
                    { "ys", new(MyResource.GName_YS, MyResource.GTag_YS,MyResource.StartFile_YS) },
                    { "hsr", new(MyResource.GName_Hsr, MyResource.GTag_Hsr,MyResource.StartFile_Hsr) },
                    { "zzz", new(MyResource.GName_ZZZ, MyResource.GTag_ZZZ,MyResource.StartFile_ZZZ) }
                };
            }
            public static Dictionary<string, GameTag> GameTags { get; set; }
        }

        /// <summary>
        /// 游戏标记
        /// </summary>
        /// <param name="name">中文名称</param>
        /// <param name="tag">游戏Tag</param>
        public class GameTag(string name, string tag, string gameFile)
        {
            public string Name { get; set; } = name;
            public string Tag { get; set; } = tag;
            public string GameFile { get; set; } = gameFile;
        }
    }
}

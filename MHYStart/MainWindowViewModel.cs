using MHYStart.Programes;
using System.Collections.ObjectModel;

namespace MHYStart
{
    class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            //Users.Add(new() { Name = "测试" });
        }
        public Saber.GameInfo GameInfo { get; set; } = new();
        public ObservableCollection<Saber.User> Users { get; set; } = [];
    }
}

using System.Windows;

namespace MHYStart
{
    public partial class InputWindow : Window
    {
        public string InputName { get; set; } = string.Empty;
        public InputWindow()
        {
            InitializeComponent();
        }


        private void Yes_Btn_Click(object sender, RoutedEventArgs e)
        {
            InputName = Input_Tbox.Text;
            if (string.IsNullOrEmpty(InputName))
            {
                MessageBox.Show("至少输入一个字符。", "提示");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void No_Btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Input_Tbox.Focus();
            Input_Tbox.Text = InputName;
            Input_Tbox.SelectionStart = Input_Tbox.Text.Length;//设置光标位置
            Input_Tbox.SelectAll();//全选
        }
    }
}

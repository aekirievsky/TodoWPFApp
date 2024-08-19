using System.Configuration;
using System.Data;
using System.Windows;

namespace TodoWPFApp
{ 
    public partial class App : Application
    {
        public static int LoggedInUserId { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Запуск окна регистрации
            var registrationWindow = new LoginRegisterWindow();
            registrationWindow.Show();
        }
    }
}

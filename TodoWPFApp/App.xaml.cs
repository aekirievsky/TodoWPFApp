using System.Configuration;
using System.Data;
using System.Windows;

namespace TodoWPFApp
{ 
    public partial class App : Application
    {
        public static int LoggedInUserId { get; set; }
    }
}

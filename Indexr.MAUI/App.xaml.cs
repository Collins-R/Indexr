using Microsoft.Extensions.DependencyInjection;

namespace Indexr.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
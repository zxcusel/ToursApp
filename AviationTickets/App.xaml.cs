using System.Windows;

namespace AviationTickets
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AviationTickets.Data.DatabaseHelper.InitializeDatabase();
        }
    }
}

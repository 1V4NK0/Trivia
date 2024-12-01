namespace CherkesIvanTrivia;

public partial class AppShell : Shell
{
	
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(Settings), typeof(Settings));
		Routing.RegisterRoute(nameof(Game), typeof(Game));
    }
}


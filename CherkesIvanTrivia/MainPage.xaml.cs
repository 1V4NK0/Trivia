namespace CherkesIvanTrivia;

public partial class MainPage : ContentPage
{
	int count = 0;

	


	public MainPage()
	{
		InitializeComponent();
	}

    async void SettingsBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Settings));
    }

    async void StartGameBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Game));
    }
}



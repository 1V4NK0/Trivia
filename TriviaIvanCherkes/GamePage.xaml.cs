using System.ComponentModel;

namespace TriviaIvanCherkes;

public partial class GamePage : ContentPage, INotifyPropertyChanged
{
    private string players;
    public string Players
    {
        get => players;
        set
        {
            players = value;
            OnPropertyChanged(nameof(Players));
        }
    }
	public GamePage(string players)
	{
		InitializeComponent();
        Players = players;
        BindingContext = this;
	}

   

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
    }
}

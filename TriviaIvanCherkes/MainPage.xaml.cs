using System.Windows.Input;

namespace TriviaIvanCherkes;

public partial class MainPage : ContentPage
{
    
    

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        

       
        
    }

    async void PlayBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        var button = (Button)sender;
        var players = button.Text;
        await DisplayAlert("", $"you have chosen {players}", "ok");
        //create tabs
        await Navigation.PushAsync(new GamePage(players));
        //to create a game obj u can use constructors and pass params in there like so
        //await Navigation.PushAsync(new TimedGame(difficulty.SelectedIndex, type.SelectedIndex, playerName));
    }
}

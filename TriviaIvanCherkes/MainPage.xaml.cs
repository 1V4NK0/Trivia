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
        //await DisplayAlert("", $"you have chosen {players}", "ok");
        var difficulty = Preferences.Get("difficulty", "medium");
        var topic = Preferences.Get("topic", "general");
        var numOfQuestions = Preferences.Get("numOfQuestions", 10);
        //TODO: create dictionary for difficulty and topics to correspond to their indexes
        await Navigation.PushAsync(new GamePage(players,numOfQuestions,topic,difficulty));
        
    }
}

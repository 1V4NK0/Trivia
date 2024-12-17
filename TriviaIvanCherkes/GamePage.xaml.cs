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

    private string topic;
    public string Topic
    {
        get => topic;
        set
        {
            topic = value;
            OnPropertyChanged(nameof(Topic));
        }
    }

    private int numOfQuestions;
    public int NumOfQuestions
    {
        get => numOfQuestions;
        set
        {
            numOfQuestions = value;
            OnPropertyChanged(nameof(NumOfQuestions));
        }
    }

    private string difficulty;
    public string Difficulty
    {
        get => difficulty;
        set
        {
            difficulty = value;
            OnPropertyChanged(nameof(Difficulty));
        }
    }

	public GamePage(string players, int numOfQuestions, string topic, string difficulty)
	{
		InitializeComponent();
        BindingContext = this;
        Players = players;
        Difficulty = difficulty;
        NumOfQuestions = numOfQuestions;
        Topic = topic;
	}

   

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
    }
}

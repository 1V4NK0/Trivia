using System.ComponentModel;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Web;
namespace TriviaIvanCherkes;
//dotnet build -t:Run -f net8.0-maccatalyst


//add timer for each question and inside settings!
//add saving results option (Preferences)
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

    private string currPlayer;
    public string CurrPlayer
    {
        get => currPlayer;
        set
        {
            currPlayer = value;
            OnPropertyChanged(nameof(CurrPlayer));
        }
    }

    private ObservableCollection<Question> _questions;
    public ObservableCollection<Question> Questions { get => _questions; }
    private int currentQuestionIndex;
    List<Question> questionList;
    HttpClient httpClient;
    private int score;
    //implement dictionary with names and scores? with saving to preferences
    Dictionary<string, int> playersScore = new Dictionary<string, int>();
    public List<string> names;
    private int currentPlayerIndex;


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

    public List<string> Answers
    {
        get
        {
            if (CurrentQuestion == null) return new List<string>();
            List<string> allAnswers = new List<string>(CurrentQuestion.incorrect_answers) { CurrentQuestion.correct_answer };
            return allAnswers;
        }
    }

    private Question currentQuestion;
    public Question CurrentQuestion
    {
        get => currentQuestion;
        set
        {
            currentQuestion = value;
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(Answers));
            
        }
    }


    private string selectedAnswer;
    public string SelectedAnswer
    {
        get => selectedAnswer;
        set
        {
            selectedAnswer = value;
            OnPropertyChanged(nameof(SelectedAnswer));
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

    private bool isBusy;
    public bool IsBusy
    {
        get
        {
            return isBusy;
        }
        set
        {
            if (isBusy != value)
            {
                isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
    }
    public bool IsNotBusy => !IsBusy;

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
        Players = players;
        Difficulty = difficulty;
        NumOfQuestions = numOfQuestions;
        Topic = topic;
        httpClient = new HttpClient();
        questionList = new List<Question>();
        _questions = new ObservableCollection<Question>();
        currentQuestionIndex = 0;
        currentPlayerIndex = 0;
        BindingContext = this;
        names = new List<string>();
        
	}

    //Method to get players name
    private async Task CollectPlayerNames()
    {
        Int32.TryParse(Players, out int num);

        names.Clear(); // Clear any existing names
        for (int i = 0; i < num; i++)
        {
            string name = await DisplayPromptAsync("Player Name", $"Enter the name of player {i + 1}:", "OK");
            if (name == null || name == "")
            {
                name = $"Player {i + 1}"; // Default name if none is provided
            }
            //THIS MIGHT BE THE WHOLE PROBLEM?!
            playersScore.Add(name, 0);
            names.Add(name);
        }
        CurrPlayer = names[0]; // Set the first player as the current player
        
    }





    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await CollectPlayerNames();
        
        await MakeCollection();
    }


    //Fetching questions from API
    public async Task GetQuestions(string difficulty, int numOfQuestions, string topic)
    {
        int topicNumber;
        if (SettingsPage.TopicToNumber.TryGetValue(topic, out int numberOfTopic))
        {
            topicNumber = numberOfTopic;
        } else
        {
            topicNumber = 9; // Default to general knowledge if no valid topic
        }

        if (questionList.Count > 0) return;

        string url = $"https://opentdb.com/api.php?amount={numOfQuestions}&category={topicNumber}&difficulty={difficulty.ToLower()}";

        try
        {
            //Console.WriteLine($"Making API call to: {url}");

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string contents = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("API Response: " + contents);  

                var questionResponse = JsonSerializer.Deserialize<QuestionResponse>(contents);
                //Console.WriteLine("QUESTION RESPONSE RESULTS: " + questionResponse?.Results);
                //Console.WriteLine("Deserialized API Response: " + JsonSerializer.Serialize(questionResponse));  // Check the deserialized object

                if (questionResponse?.results != null)
                {
                    questionList = questionResponse.results;
                    //Console.WriteLine($"Fetched {questionList.Count} questions.");
                }
                else
                {
                    //Console.WriteLine("No results found in API response.");
                }
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error loading questions: {ex.Message}");
            await Shell.Current.DisplayAlert("Error loading questions", ex.Message, "OK");
        }
    }


    //Making a questions list from data 
    public async Task MakeCollection()
    {
        if (isBusy)
            return;

        IsBusy = true;

        try
        {
            //Console.WriteLine("Calling GetQuestions...");  // Debug log
            await GetQuestions(Difficulty, NumOfQuestions, Topic);

            if (questionList.Count == 0)
            {
                //Console.WriteLine("No questions loaded!");  // Debug log
                return;
            }

            _questions.Clear();
            //Formatting question because you get those html things
            foreach (var question in questionList)
            {
                question.question = HttpUtility.HtmlDecode(question.question);
                question.correct_answer = HttpUtility.HtmlDecode(question.correct_answer);
                for (int i = 0; i < question.incorrect_answers.Count; i++)
                {
                    question.incorrect_answers[i] = HttpUtility.HtmlDecode(question.incorrect_answers[i]);
                }
                _questions.Add(question);
            }
            Console.WriteLine($"Loaded {questionList.Count} questions.");  // Debug log

            NextQuestion();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in MakeCollection: {ex.Message}");  // Debug log
            await Shell.Current.DisplayAlert("Error in loading questions", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }


    //Moving to the next question and switching between players
    public void NextQuestion()
    {
        if (currentQuestionIndex < questionList.Count)
        {
            CurrentQuestion = questionList[currentQuestionIndex];
            currentQuestionIndex++;
            CurrPlayer = names[currentPlayerIndex];
            currentPlayerIndex = (currentPlayerIndex + 1) % names.Count;
        } else
        {
            QuestionLabel.Text = "Game Over!";
            GameFinished();
        }
    }

    
    //Answering question
    async void AnswBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        var button = (Button)sender;
        var answer = button.Text;
        var correctAnsw = CurrentQuestion.correct_answer;
        if (answer.Equals(correctAnsw))
        {
            button.BackgroundColor = Colors.Green;
            button.Text = "😎";
            //might be a problem with this OR no players are actually added to the dictionary?
            if (playersScore.ContainsKey(CurrPlayer))
            {
                playersScore[CurrPlayer]++;
                Console.WriteLine("PLAYER SCORE: " + playersScore[CurrPlayer]);
            }
        }
        else
        {
            button.BackgroundColor = Colors.Red;
            button.Text = "😢";
        }
        await Task.Delay(500);
        NextQuestion();
        button.BackgroundColor = (Color)Application.Current.Resources["TextColor"]; // Reset to the default color
        button.Text = answer;
    }


    //
    void GameFinished()
    {
        foreach (var pop in playersScore)
        {
            Console.WriteLine("SCORE: " + pop.Key + pop.Value);
        }
        // Hide the question view and player turn
        QuestionsView.IsVisible = false;
        CurrPlayerStack.IsVisible = false;
        
        // Create the ListView and bind to the dictionary converted to a list of KeyValuePair
        var resultListView = new ListView
        {
            ItemsSource = playersScore.ToList(),  // Convert dictionary to list of KeyValuePairs
            ItemTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label()
                {
                    TextColor = (Color)Application.Current.Resources["TextColor"],
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };
                nameLabel.SetBinding(Label.TextProperty, new Binding("Key")); // Bind to player name (Key)

                var scoreLabel = new Label()
                {
                    TextColor = (Color)Application.Current.Resources["TextColor"],
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.End

                };
                scoreLabel.SetBinding(Label.TextProperty, new Binding("Value")); // Bind to score (Value)

                // Use a StackLayout to arrange player name and score horizontally
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,  // Horizontal layout for name and score
                    Children = { nameLabel, scoreLabel },
                    Padding = 30
                };

                return new ViewCell { View = stackLayout };
            })
        };

        // Add the ListView to the layout
        GeneralLayout.Children.Add(resultListView);
    }


}

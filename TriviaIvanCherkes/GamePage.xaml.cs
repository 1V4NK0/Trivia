using System.ComponentModel;
using System.Text.Json;
using System.Collections.ObjectModel;

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

    private ObservableCollection<Question> _questions;
    public ObservableCollection<Question> Questions { get => _questions; }
    private int currentQuestionIndex;
    List<Question> questionList;
    HttpClient httpClient;

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
            //might be a problem
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
        BindingContext = this;
	}

   

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        //there is a problem with MakeCollection itself OR with GetQuestions OR the way we handle API request nested objs
        await MakeCollection();
    }

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
            Console.WriteLine($"Making API call to: {url}");

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string contents = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + contents);  

                var questionResponse = JsonSerializer.Deserialize<QuestionResponse>(contents);
                //Console.WriteLine("QUESTION RESPONSE RESULTS: " + questionResponse?.Results);
                Console.WriteLine("Deserialized API Response: " + JsonSerializer.Serialize(questionResponse));  // Check the deserialized object

                if (questionResponse?.results != null)
                {
                    questionList = questionResponse.results;
                    Console.WriteLine($"Fetched {questionList.Count} questions.");
                }
                else
                {
                    Console.WriteLine("No results found in API response.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading questions: {ex.Message}");
            await Shell.Current.DisplayAlert("Error loading questions", ex.Message, "OK");
        }
    }







    public async Task MakeCollection()
    {
        if (isBusy)
            return;

        IsBusy = true;

        try
        {
            Console.WriteLine("Calling GetQuestions...");  // Debug log
            await GetQuestions(Difficulty, NumOfQuestions, Topic);

            if (questionList.Count == 0)
            {
                Console.WriteLine("No questions loaded!");  // Debug log
                return;
            }

            _questions.Clear();
            foreach (var question in questionList)
            {
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


    //nextQuestion might be the problem or getQuestions or makeCollection
    public void NextQuestion()
    {
        
        if (currentQuestionIndex < questionList.Count)
        {
            CurrentQuestion = questionList[currentQuestionIndex];
            currentQuestionIndex++;
        } else
        {
            //game over;
        }
    }

    void OnSelectedAnswer(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
    }

       //todo answer selected method
}

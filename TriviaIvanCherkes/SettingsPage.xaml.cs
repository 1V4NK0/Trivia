using TriviaIvanCherkes.Resources.Styles;
using Plugin.Maui.Audio;
using System.ComponentModel;

namespace TriviaIvanCherkes;

public partial class SettingsPage : ContentPage, INotifyPropertyChanged
{
    //TODO: CREATE ALL SETTINGS THEN PASS THEM INTO NEW GAME OBJ.
    //then try to display those settings in the game page
    private string topic;
    private string difficulty;
    private int numOfQuestions;

    public string Topic
    {
        get => topic;
        set
        {
            topic = value;
            OnPropertyChanged(nameof(Topic));
            Preferences.Set("topic", topic);
        }
    }

    public string Difficulty
    {
        get => difficulty;
        set
        {
            difficulty = value;
            OnPropertyChanged(nameof(Difficulty));
            Preferences.Set("difficulty", difficulty);
        }
    }

    public int NumOfQuestions { get => numOfQuestions; set 
            {
            if (value < 0 || value > 20)
            {
                numOfQuestions = 10;
            }
            numOfQuestions = value;
            OnPropertyChanged(nameof(NumOfQuestions));
            Preferences.Set("numOfQuestions", numOfQuestions);
        } }
    public bool IsDarkTheme { get; set; }
    private IAudioPlayer audioPlayer;


    public SettingsPage()
    {
        InitializeComponent();

        // Retrieve the saved theme preference and set the property
        IsDarkTheme = Preferences.Get("isDarkTheme", false); // Default to LightTheme

        // Apply the saved theme on page load
        ApplyTheme(IsDarkTheme);
        InitializeAudioPlayer();
        BindingContext = this; // Set BindingContext after initializing IsDarkTheme
    }

    void DarkTheme_Toggled(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
    {
        IsDarkTheme = e.Value; // Update the property from the Switch
        Preferences.Set("isDarkTheme", IsDarkTheme); // Save the updated preference

        // Apply the theme based on the updated toggle state
        ApplyTheme(IsDarkTheme);
    }

    private async void InitializeAudioPlayer()
    {
        try
        {
            // Load the audio file from the Resources folder
            audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("music.mp3"));
            audioPlayer.Loop = true; 
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load music: {ex.Message}", "OK");
        }
    }

    private void ApplyTheme(bool isDark)
    {
        Application.Current.Resources.MergedDictionaries.Clear();
        if (isDark)
        {
            Application.Current.Resources.MergedDictionaries.Add(new DarkTheme());
        }
        else
        {
            Application.Current.Resources.MergedDictionaries.Add(new LightTheme());
        }
    }

    void Music_Toggled(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
    {
        if (audioPlayer == null)
            return;

        if (e.Value)
        {
            // Play the music when the switch is turned on
            audioPlayer.Play();
        }
        else
        {
            // Stop the music when the switch is turned off
            audioPlayer.Stop();
        }
    }
}

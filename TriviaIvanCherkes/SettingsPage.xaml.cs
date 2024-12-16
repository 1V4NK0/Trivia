using TriviaIvanCherkes.Resources.Styles;

namespace TriviaIvanCherkes;

public partial class SettingsPage : ContentPage
{
    public bool IsDarkTheme { get; set; } // Use a property for better BindingContext support

    public SettingsPage()
    {
        InitializeComponent();

        // Retrieve the saved theme preference and set the property
        IsDarkTheme = Preferences.Get("isDarkTheme", false); // Default to LightTheme

        // Apply the saved theme on page load
        ApplyTheme(IsDarkTheme);

        BindingContext = this; // Set BindingContext after initializing IsDarkTheme
    }

    void DarkTheme_Toggled(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
    {
        IsDarkTheme = e.Value; // Update the property from the Switch
        Preferences.Set("isDarkTheme", IsDarkTheme); // Save the updated preference

        // Apply the theme based on the updated toggle state
        ApplyTheme(IsDarkTheme);
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
}

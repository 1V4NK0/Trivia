using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CherkesIvanTrivia
{
	public class GameViewModel : INotifyPropertyChanged
    {
		HttpClient httpClient;
		private string currQuestion;
		private ObservableCollection<QuestionModel> questions;

        public event PropertyChangedEventHandler? PropertyChanged;

		public string CurrentQuestion
		{
			get => currQuestion;
			set
			{
				if (currQuestion != value)
				{
					currQuestion = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand SelectAnswerCommand { get; }
		public ICommand NextQuestionCommand { get; }

		public ObservableCollection<QuestionModel> Questions
		{
			get => questions;
			set
			{
				if (questions != value)
				{
					questions = value;
					OnPropertyChanged();
				}
			}
		}




        public GameViewModel()
		{
			httpClient = new HttpClient();
			Questions = new ObservableCollection<QuestionModel>();
			SelectAnswerCommand = new Command(OnAnswerSelected);
			NextQuestionCommand = new Command(OnNextQuestionSelected);

		}

		private void OnAnswerSelected()
		{

		}

		//just get questions from api for now

		private void OnNextQuestionSelected()
		{

		}

		public async Task LoadQuestions(int amountOfQuestions = 10, string questionType = null, string difficulty = null)
		{
			//use if for different parameters
			int amountForTwo = amountOfQuestions * 2;
			var url = "";
			if (difficulty != null && questionType != null)
			{
				url = $"https://opentdb.com/api.php?amount={amountForTwo}&difficulty={difficulty}&type={questionType}";
			} else if (difficulty == null)
			{
				url = $"https://opentdb.com/api.php?amount={amountForTwo}&type={questionType}";
			} else if (questionType == null)
			{
				url = $"https://opentdb.com/api.php?amount={amountForTwo}&difficulty={difficulty}";
			} else if (difficulty == null && questionType == null)
			{
				url = $"https://opentdb.com/api.php?amount={amountForTwo}";
			}

			var response = await httpClient.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				response.Content.ReadAsStringAsync();
			}

		}


        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}


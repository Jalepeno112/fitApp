using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class EditGoalPage : ContentPage
	{
		FitAppDatabase database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));
		bool valueEntryFinished;
		bool unitEntryFinished;
		public EditGoalPage(string name)
		{
			InitializeComponent();
			valueEntryFinished = false;
			unitEntryFinished = false;
			finishButton.IsEnabled = false;

			unitEntry.Items.Add("Minutes");
			unitEntry.Items.Add("Repetitions");
			unitEntry.Items.Add("Kgs");
			unitEntry.Items.Add("Lbs");
			unitEntry.SelectedIndex = 0;

			valueEntry.TextChanged += OnTextChanged;

			finishButton.Clicked += (sender, e) =>
			{
				GoalDB gdb = new GoalDB
				{
					Name = name,
					unit = unitEntry.Items[unitEntry.SelectedIndex],
					goal = Double.Parse(valueEntry.Text)
				};
				database.WriteGoal(gdb);
				Navigation.PopAsync();
			};
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			if (((Entry)sender).Text != "")
			{
				if (sender == valueEntry)
					valueEntryFinished = true;
			}

			if (valueEntryFinished == true)
			{
				finishButton.IsEnabled = true;
			}
		}
	}
}

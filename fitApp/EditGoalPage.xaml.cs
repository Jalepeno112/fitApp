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

			valueEntry.TextChanged += OnTextChanged;
			unitEntry.TextChanged += OnTextChanged;

			finishButton.Clicked += (sender, e) =>
			{
				GoalDB gdb = new GoalDB
				{
					Name = name,
					unit = unitEntry.Text,
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
				if (sender == unitEntry)
					unitEntryFinished = true;
			}

			if (valueEntryFinished == true && unitEntryFinished == true)
			{
				finishButton.IsEnabled = true;
			}
		}
	}
}

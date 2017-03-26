using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class AnalyticsPage : ContentPage
	{
		FitAppDatabase database=new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));
		public AnalyticsPage()
		{
			InitializeComponent();

			var workoutitems = database.GetExercises();
			exerciseList.ItemsSource = workoutitems;
			exerciseList.ItemTemplate = new DataTemplate(typeof(TextCell));
			exerciseList.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");

			// Add an on tap handler and send the name of the exercise to the graph page
			exerciseList.ItemTapped += async (s, e) =>
			{
				await Navigation.PushAsync(new GraphPage(((WorkoutItemDB)e.Item).Name));
				((ListView)s).SelectedItem = null;
			};

			ToolbarItem tbItem = new ToolbarItem();
			tbItem.Text = "Times";
			tbItem.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new GraphPage("_time"));
				exerciseList.SelectedItem = null;
			};
			ToolbarItems.Add(tbItem);
		}
	}
}

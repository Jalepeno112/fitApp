using System;
using System.Collections.Generic;
using Xamarin.Forms;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;

namespace fitApp
{
	public partial class GraphPage : ContentPage
	{
		string name;
		FitAppDatabase database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));

		public GraphPage(string _name)
		{
			InitializeComponent();
			name = _name;
			OxyPlot.Xamarin.Forms.PlotView plotview = new PlotView() { Model = CreatePlotModel() };
			Content = plotview;
		}


		private PlotModel CreatePlotModel()
		{
			var data = database.GetWorkouts(name);
			var model = new PlotModel
			{
				Title = name,
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.BottomCenter,
				LegendOrientation = LegendOrientation.Horizontal,
				LegendBorderThickness = 0
			};
			var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };

			//We have to iterate over each data point to see how many columns we have to create per date
			//Also, create category for each date in same loop
			int max = 0;
			foreach (WorkoutItem _data in data)//Iterate over each date
			{
				int thisMax = 0;
				//Iterate over each set
				foreach (Double amount in _data.Set)
				{
					thisMax++;	
				}
				if (thisMax > max)
					max = thisMax;
				categoryAxis.Labels.Add(_data.Date.ToString("M/d/yy"));
			}

			// Create number of columns corresponding to the maximum number of columns needed
			for (int i = 0; i < max; i++) // Iterate over set #'s
			{
				var series = new ColumnSeries { Title = "Set " + (i + 1), StrokeColor = OxyColors.Black, StrokeThickness = 1 };
				foreach (WorkoutItem _data in data) // Iterate over dates
				{
					if (_data.Set.Count <= i) //If this set does not exist
						series.Items.Add(new ColumnItem { Value = 0 });
					else
						series.Items.Add(new ColumnItem { Value = _data.Set[i] });
				}
				model.Series.Add(series);
			}

			var valueAxis = new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };

			model.Axes.Add(categoryAxis);
			model.Axes.Add(valueAxis);

			return model;
		}
	}
}

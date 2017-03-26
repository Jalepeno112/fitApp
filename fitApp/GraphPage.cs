using System;
using System.Collections.Generic;
using Xamarin.Forms;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;
using System.Collections.ObjectModel;

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
			PlotView plotview;
			if (_name == "_time")
				plotview = new PlotView() { Model = CreateTimeModel() };
			else
				plotview = new PlotView() { Model = CreatePlotModel() };
			Content = plotview;
		}

		/*
		 * If we are graphing the time, we have to treat this a little differently
		 * There is only one time per date,so we will be using a line graph
		 *
		 */
		private PlotModel CreateTimeModel()
		{
			List<WorkoutTimeDB> data = (List<WorkoutTimeDB>)database.GetWorkoutTimes();

			var model = new PlotModel
			{
				Title = name,
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.BottomCenter,
				LegendOrientation = LegendOrientation.Horizontal,
				LegendBorderThickness = 0
			};
			var series = new LineSeries()
			{
				Color = OxyColors.SkyBlue,
				MarkerType = MarkerType.Circle,
				MarkerSize = 6,
				MarkerStroke = OxyColors.White,
				MarkerFill = OxyColors.SkyBlue,
				MarkerStrokeThickness = 1.5
			};
			DateTimeAxis xaxis = new DateTimeAxis { Position = AxisPosition.Bottom };
			xaxis.StringFormat = "M/d";
			xaxis.Minimum = DateTimeAxis.ToDouble(DateTime.Parse(data[0].Date));
			DateTimeAxis yaxis = new DateTimeAxis { Position = AxisPosition.Left };
			model.Axes.Add(xaxis);
			model.Axes.Add(yaxis);
			foreach (WorkoutTimeDB _data in data)
			{
				series.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Parse(_data.Date), DateTime.Parse(_data.Time)));
			}

			model.Series.Add(series);
			return model;
		}

		/*
		 * If we are graphing the exercises, a line graph won't do, so we use a 
		 * column graph to display each set in different colors
		 */
		private PlotModel CreatePlotModel()
		{
			var data = database.GetWorkouts(name);
			GoalDB goal = database.GetGoal(name);
			System.Diagnostics.Debug.WriteLine("GOAL: ", goal);
			var subtitle = "";

			// make sure the goal is set before trying to display it
			if (goal == null)
			{
				subtitle = "Goal not set";
			}
			else
			{
				subtitle = goal.goal.ToString() + goal.unit;
			}

			var model = new PlotModel
			{
				Title = name,
				Subtitle = subtitle,
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.BottomCenter,
				LegendOrientation = LegendOrientation.Horizontal,
				LegendBorderThickness = 0
			};
			var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };

			//We have to iterate over each data point to see how many columns we have to create per date
			//Also, create category for each date
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
				var series = new BarSeries { Title = "Set " + (i + 1), StrokeColor = OxyColors.Black, StrokeThickness = 1 };
				foreach (WorkoutItem _data in data) // Iterate over dates
				{
					if (_data.Set.Count <= i) //If this set does not exist
						series.Items.Add(new BarItem { Value = 0 });
					else
						series.Items.Add(new BarItem { Value = _data.Set[i] });
				}
				model.Series.Add(series);
			}


			var valueAxis = new LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0, Title = data[0].Unit};

			model.Axes.Add(categoryAxis);
			model.Axes.Add(valueAxis);


			return model;
		}
	}
}

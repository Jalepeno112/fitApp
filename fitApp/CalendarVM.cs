using System;
using Xamarin.Forms;

using System.ComponentModel;
using System.Windows.Input;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Collections.Specialized;

using SQLite;



namespace fitApp
{

	public class CalendarVM : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private DateTime _date;
		private bool _editMode;
		private ObservableCollection<WorkoutItem> _workoutList = new ObservableCollection<WorkoutItem>();
		private StackLayout _workoutView;

		public FitAppDatabase database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));

		public CalendarVM()
		{
		}

		// the selected date
		public DateTime Date
		{
			get
			{
				return _date;
			}
			set
			{
				_date = value;
				OnPropertyChanged("Date");
			}
		}

		// whether or not the workout is in editable mode
		// this changes the way we display the data
		public bool Editable
		{
			get
			{
				return (bool)_editMode;
			}
			set
			{
				_editMode = value;
				OnPropertyChanged("Editable");
			}
		}

		// the list of workout items for the given day
		public ObservableCollection<WorkoutItem> Workout
		{
			get
			{
				return _workoutList;
			}
			set
			{
				System.Diagnostics.Debug.WriteLine("Workout changed in VM");
				_workoutList = value;
				OnPropertyChanged("Workout");
			}
		}

		// Remove a given workout item from the database
		// this will also remove it from our object in memory in order to update the view
		public ICommand RemoveWorkoutItem
		{
			get
			{
				return new Command<Button>((obj) =>
				{
					System.Diagnostics.Debug.WriteLine(obj.StyleId);

					// the styleId is the ID of the object
					int id = Convert.ToInt32(obj.StyleId);

					// find the item
					for (int i = 0; i < _workoutList.Count; i++)
					{
						if (_workoutList[i].ID == id)
						{
							// remove from database, remove from list
							database.RemoveWorkout(_workoutList[i]);
							_workoutList.RemoveAt(i);
							break;
						}
					}

					// In order for the property changed to fire, we have to recreate the list
					// not ideal at all, but it works for now
					Workout = new ObservableCollection<WorkoutItem>(_workoutList);
					OnPropertyChanged("Workout");
				});
			}
		}

		// parse the add new workout form and add the information into our database
		public ICommand AddNewWorkout
		{
			get
			{
				return new Command<StackLayout>((sl) =>
				{
					WorkoutItem wi = new WorkoutItem();

					// for each child of the stack layout, try and convert it to an Entry
					// if it can't be converted, we aren't interested in it.
					// Depending on what the name of the field is, 
					// it represents a different property in the workout item we are creating
					foreach (View child in sl.Children)
					{
						Entry e;
						try
						{
							e = (Entry)child;
						}
						catch (System.InvalidCastException)
						{
							continue;
						}
						if (String.Compare(e.StyleId, "ItemName") == 0)
						{
							wi.Name = e.Text;
							e.Text = "";
						}
						else if (String.Compare(e.StyleId, "ItemUnit") == 0)
						{
							wi.Unit = e.Text;
							e.Text = "";
						}
						else if (String.Compare(e.StyleId, "Reps") == 0)
						{
							ObservableCollection<double> reps = new ObservableCollection<double>();
							List<string> split = new List<string>(e.Text.Split(','));
							foreach (string s in split)
							{
								reps.Add(Convert.ToDouble(s));
							}
							wi.Set = reps;
							e.Text = "";
						}
					}
					wi.Date = Date;

					// add to the list and commit to database
					_workoutList.Add(wi);
					database.WriteWorkout(wi);

					// in order for the ObservableCollection to register the change, we have to assign it to a new object
					// not pretty, but it gets the job done
					Workout = new ObservableCollection<WorkoutItem>(_workoutList);
				});
			}
		}

		// this fires whenever the date changes
		public ICommand DateChosen
		{
			get
			{
				return new Command((obj) =>
				{
					// get the new datetime and find the workouts associated with it
					DateTime dt = (DateTime)obj;
					System.Diagnostics.Debug.WriteLine(obj as DateTime?);
					// run SQL command and populate bottom of view with Workout details
					Workout = database.GetWorkouts(dt);

					System.Diagnostics.Debug.WriteLine("Received: " + Workout.Count);
				});
			}

		}
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this,
					new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	// main class that our view uses for presenting data
	public class WorkoutItem
	{

		public string Name { get; set; }
		public ObservableCollection<double> Set { get; set; }
		public string Unit { get; set; }
		public DateTime Date { get; set; }
		public int ID { get; set; }
	}

}


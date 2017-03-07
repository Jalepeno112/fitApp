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
	/*
	 * This interface is necesasry in order to do OS specific operations
	 * 
	 * The location of the DB file will be different on different OS's.
	 * 
	 * https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/databases/
	 */
	public interface IFileHelper
	{
		string GetLocalFilePath(string filename);
	}

	/*
	 * Main databae class.  Contains the 
	 */ 
	public class FitAppDatabase
	{
		private SQLiteConnection _connection;
		public FitAppDatabase(string dbPath)
		{
			// establish connection
			_connection = new SQLiteConnection(dbPath);

			// create the tables that we need
			// we can add more here if necessary
			// if the table already exists, it will skip this part
			_connection.CreateTable<WorkoutItemDB>();
			_connection.CreateTable<SetDB>();
			_connection.CreateTable<GoalDB>();

		}

		public void GenerateFakeData()
		{
			/*Put some fake data into the database*/
			int key = _connection.Insert(new WorkoutItemDB
			{
				Name = "Treadmill",
				Unit = "Minutes",
				Date = "2017-03-02"
			});

			// get the id of the last written object
			// we need it to create all of the set objects
			string sql = @"select last_insert_rowid()";
			key = _connection.ExecuteScalar<int>(sql);
			System.Diagnostics.Debug.WriteLine("LAST INSERTED KEY: " + key);

			// insert a few repetitions for this workout item
			_connection.Insert(new SetDB()
			{
				WorkoutItemID = key,
				Amount = 45.0
			});
			_connection.Insert(new SetDB()
			{
				WorkoutItemID = key,
				Amount = 15.0
			});
			_connection.Insert(new SetDB()
			{
				WorkoutItemID = key,
				Amount = 30.0
			});
		}

		//Gets the list of unique exercises that the user has entered
		public IEnumerable<WorkoutItemDB> GetExercises()
		{
			return _connection.Query<WorkoutItemDB>("SELECT DISTINCT [Name] FROM [WorkoutItemDB]");
		}

		public ObservableCollection<WorkoutItem> GetWorkouts(DateTime date)
		{
			// pull the workout items out of the database and convert them to the WorkoutItem class
			// the WorkoutItem class is more convienent for the view, but WorkoutItemDB is more convient for SQL
			// NOTE:
			//	we should eventually make those two objects the same.  It's redundant having both
			ObservableCollection<WorkoutItem> coll = new ObservableCollection<WorkoutItem>();
			string date_string = date.ToString("yyyy-MM-dd");
			List<WorkoutItemDB> l = _connection.Query<WorkoutItemDB>("SELECT * FROM [WorkoutItemDB] WHERE [Date] = ?", date_string);

			System.Diagnostics.Debug.WriteLine("FETCHED: " + l.Count);

			// convert the DB items into WorkoutItems for the view
			foreach (WorkoutItemDB i in l)
			{
				System.Diagnostics.Debug.WriteLine("KEY: " + i.ID);

				WorkoutItem w = new WorkoutItem
				{
					Name = i.Name,
					Date = Convert.ToDateTime(i.Date),
					Unit = i.Unit,
					ID = i.ID
				};

				// get the set
				List<SetDB> s = _connection.Query<SetDB>("SELECT [Amount] FROM [SetDB] WHERE [WorkoutItemID] = ?", i.ID);
				var d = new ObservableCollection<double>();
				System.Diagnostics.Debug.WriteLine("SET ITEMS: " + s.Count);
				foreach (SetDB k in s)
				{
					d.Add(k.Amount);
				}
				w.Set = d;
				coll.Add(w);
			}

			return coll;
		}

		// Get the workouts based on the name of the exercise
		public ObservableCollection<WorkoutItem> GetWorkouts(string name)
		{
			ObservableCollection<WorkoutItem> coll = new ObservableCollection<WorkoutItem>();
			List<WorkoutItemDB> l = _connection.Query<WorkoutItemDB>("SELECT * FROM [WorkoutItemDB] WHERE [Name] = ? ORDER BY [Date]", name);

			System.Diagnostics.Debug.WriteLine("FETCHED: " + l.Count);

			// convert the DB items into WorkoutItems for the view
			foreach (WorkoutItemDB i in l)
			{
				System.Diagnostics.Debug.WriteLine("KEY: " + i.ID);

				WorkoutItem w = new WorkoutItem
				{
					Name = i.Name,
					Date = Convert.ToDateTime(i.Date),
					Unit = i.Unit,
					ID = i.ID
				};

				// get the set
				List<SetDB> s = _connection.Query<SetDB>("SELECT [Amount] FROM [SetDB] WHERE [WorkoutItemID] = ?", i.ID);
				var d = new ObservableCollection<double>();
				System.Diagnostics.Debug.WriteLine("SET ITEMS: " + s.Count);
				foreach (SetDB k in s)
				{
					d.Add(k.Amount);
				}
				w.Set = d;
				coll.Add(w);
			}

			return coll;
		}

		public void WriteWorkout(WorkoutItem w)
		{
			/*Write the WorkoutItem (which is in our ViewModel) to the WorkoutItemDB and SetDB models*/
			WorkoutItemDB wi = new WorkoutItemDB
			{
				Name = w.Name,
				Date = w.Date.ToString("yyyy-MM-dd"),
				Unit = w.Unit
			};
			int t = _connection.Insert(wi);
			System.Diagnostics.Debug.WriteLine("LINES WRITTEN: " + t);

			string sql = @"select last_insert_rowid()";
			int key = _connection.ExecuteScalar<int>(sql);

			foreach (double d in w.Set)
			{
				SetDB s = new SetDB
				{
					WorkoutItemID = key,
					Amount = d
				};
				t = _connection.Insert(s);
				System.Diagnostics.Debug.WriteLine("LINES WRITTEN: " + t);
			}
		}

		public GoalDB GetGoal(string name)
		{
			return _connection.Find<GoalDB>(name);
		}

		public void WriteGoal(GoalDB g)
		{
			//Delete the old goal if there is one
			GoalDB del = new GoalDB
			{
				Name = g.Name
			};
			_connection.Delete(del);
			           
			//Insert the goal
			int t = _connection.Insert(g);
		}


		public void RemoveWorkout(WorkoutItem w)
		{
			// delete the workout
			WorkoutItemDB wi = new WorkoutItemDB
			{
				ID = w.ID
			};
			_connection.Delete(wi);

			// delete the sets that belong to that ID
			_connection.Execute("DELETE FROM [SetDB] WHERE [WorkoutItemID] = ?", w.ID);
		}

	}

	/*
	 * Models for the Database
	 * 
	 * WorkoutItemDB 
	 * contains the date, the name of the workout, 
	 * and the unit measuring the workout type (weight, minutes, etc.)
	 * 
	 * SetDB
	 * Each workout can have multiple repetitions within it.  
	 * SQLite doesn't support lists, so we have to create a new model that links back to WorkoutItemDB.
	 * We use the WorkoutItemDB instance's Primary Key as a value in the SetDB.
	 * This allows us to retrieve all of the repetitions from the database for a given workout
	 */
	public class WorkoutItemDB
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Name { get; set; }
		public string Unit { get; set; }
		public string Date { get; set; }
	}

	public class SetDB
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		// this is a foreign key that maps to the WorkoutItem.ID
		public int WorkoutItemID { get; set; }
		public double Amount { get; set; }
	}

	public class GoalDB
	{
		[PrimaryKey]
		public string Name { get; set;} //Name of exercise

		public double goal { get; set; } //Goal
		public string unit { get; set;}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushingMachineSolver
{
	class Solver
	{
		//the maze to be solved
		private Maze original2;
		//the targets
		private Maze targets;


		//the path to the solution
		private List<Maze> solution = new List<Maze>();

		//the mazes already tried
		//we will not try them again
		//Dictionary handles colisions
		//indexed by representation, contains the smaller nesting level in which it was foud
		private Dictionary<string, int> skip = new Dictionary<string, int>();

		private int NestingStarting;
		private int NestingMax;
		public Solver(Logger Logger, Maze maze, Maze targets, int NestingStarting, int NestingMax)
		{
			this.original2 = maze;
			this.targets = targets;
			this.NestingStarting = NestingStarting;
			this.NestingMax = NestingMax;
		}


		public void PrintSolutuion(Logger Logger)
		{
			int line = 1;
			Logger.log($"starting:");
			Logger.log(original2.ToString());
			foreach (var v in solution)
			{
				Logger.log($"step: {line++}");
				Logger.log(v.ToString());
				//Console.ReadKey();
			}
			Logger.log("FINAL SOLUTION!");
			//Console.ReadKey();
		}

		/**
		 * param name="MaxDepth" specify the max number of moves
		 * */
		private int NestingMaxThisRound;
		public bool Solve(Logger Logger)
		{
			//try from small to large nestings
			for (NestingMaxThisRound = NestingStarting; NestingMaxThisRound <= NestingMax;)
			{
				DateTime start = DateTime.Now;

				solution = new List<Maze>();
				skip = new Dictionary<string, int>();
				bool solved = Solve_go(original2, 0);

				Logger.log($"Trying to solve for {NestingMaxThisRound} moves took {DateTime.Now - start}");

				if (solved)
					return true;

				NestingMaxThisRound++;
			}
			return false;
		}

		/**
		 * recursive function to solve
		 * */
		private bool Solve_go(Maze maze, int NestingCurrent)
		{
			//limit moves
			if (NestingCurrent >= NestingMaxThisRound)
				return false;

			//alreay went to that path? skip it
			/*
			 * cannot skip because a given situation was found deeper on the tree 
			 * but this same situation may lead to a solution if it happens before
			 * to work: must compare the skip with the nesting level it was achieved and skip it if it was achoiieved on the same or on a smaller nesting
			foreach (var m in skip)
			{
				if (m.SameMaze(maze))
					return false;
			}
			*/
			/*
			foreach (var m in skip)
			{
				if (m.NestingLevel <= CurrentDepth)
					if (m.maze.SameMaze(maze))
						return false;
			}
			*/



			if (skip.ContainsKey(maze.GetRepresentation()))
			{
				var level = skip[maze.GetRepresentation()];
				if (level <= NestingCurrent)
					return false;
			}

			//Logger.log(maze.ToString());
			//Console.ReadKey();


			//now, register as already went this way
			if (!skip.ContainsKey(maze.GetRepresentation()))
			{
				skip.Add(maze.GetRepresentation(), NestingCurrent);
				//Logger.log($"skips: {skip.Count()}");
			}
			skip[maze.GetRepresentation()] = NestingCurrent;



			//now move each piece
			IReadOnlyList<IReadOnlyList<MazeItem>> ourdata = maze.GetData();
			for (int r = 0; r < ourdata.Count(); r++)
			{
				IReadOnlyList<MazeItem> row = ourdata[r];
				for (int c = 0; c < row.Count(); c++)
				{
					var mi = ourdata[r][c];
					//se if it can be pressed
					if (mi == MazeItem.push_down ||
						mi == MazeItem.push_left ||
						mi == MazeItem.push_right ||
						mi == MazeItem.push_up ||
						mi == MazeItem.pushpressed_down ||
						mi == MazeItem.pushpressed_left ||
						mi == MazeItem.pushpressed_right ||
						mi == MazeItem.pushpressed_up)
					{
						//move it
						Maze copy = maze.GetClone();
						copy.PressButton(r, c);

						//solved?
						if (copy.Solved(targets))
						{
							solution.Insert(0, copy);
							return true;
						}

						//try this way
						if (Solve_go(copy, NestingCurrent + 1))
						{
							//store step to the solution
							solution.Insert(0, copy);
							return true;
						}
					}
				}
			}

			//nah, not solved
			return false;
		}
	}
}

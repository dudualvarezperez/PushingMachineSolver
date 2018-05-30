using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushingMachineSolver
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine(
"PushingMachineSolver: wrong number of parameters \n\r" +
"PushingMachineSolver startingmoves filenamebase \n\r" +
"Where startingnesting is the minimmun moves to search for and \n\r" +
"datafilenamebase is the base name for 3 files:  \n\r" +
"'filenamebase'_data.txt is the data file \n\r" +
"'filenamebase'_target.txt is the file with the targets \n\r" +
"    (if there is no element over a target, it should be the same as the data) \n\r" +
"'filenamebase'_output.txt is the file where the solution will be written \n\r" +
"    the solution is both printed on the console and saved to this file \n\r" +
" \n\r" +
"'filenamebase'_data.txt contains one line for each line in the puzzle. \n\r" +
"and 'filenamebase'_target.txt contains the targets. \n\r" +
"Use the following letters:  \n\r" +
" - space for a blank spot \n\r" +
" - X for a wall \n\r" +
" - . for a target \n\r" +
" - o for a crate \n\r" +
" - 1 for a up pusher \n\r" +
" - 2 for a down pusher \n\r" +
" - 3 for a left pusher \n\r" +
" - 4 for a right pusher \n\r" +
" \n\r" +
"You can find some sample files in the 'data' folder on the source \n\r"

);

				return;
			}

			Main_go(int.Parse(args[0]), args[1]+ "_data.txt", args[1]+ "_target.txt",args[1]+ "_output.txt");
		}
		static void Main_go(int startingnesting, string filenamebase_data, string filenamebase_target, string filenamebase_output)
		{
			Logger Logger = new Logger(filenamebase_output);
			Logger.log($"Solving for {filenamebase_data}");
			Maze maze = new Maze();
			maze.Load(File.ReadAllLines(filenamebase_data));
			Maze targets = new Maze();
			targets.Load(File.ReadAllLines(filenamebase_target));
			Solver solver = new Solver(Logger, maze, targets, startingnesting, 120);

			if (solver.Solve(Logger))
			{
				Logger.log("Solved! \n\r");
				solver.PrintSolutuion(Logger);
			}
			else
			{
				Logger.log("NOT Solved! \n\r");
			}


		}
	}
}

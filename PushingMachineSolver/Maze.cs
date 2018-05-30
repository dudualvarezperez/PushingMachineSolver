using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushingMachineSolver
{
	public enum MazeItem
	{
		wall, empty, crate, target,
		//pushers
		push_up, push_down, push_left, push_right,
		//pushers pushed
		pushpressed_up, pushpressed_down, pushpressed_left, pushpressed_right,
		//when pusshers are open
		//note: the same item never meets another same item
		//(that is, a open_push_up will never stop because of a open_push_up)
		//this is required to be able to close the pushers
		open_push_up, open_push_down, open_push_left, open_push_right

	}
	public class MazeItemConvertion
	{
		//the conversion data
		private static Dictionary<MazeItem, char> _data = null;
		private static Dictionary<MazeItem, char> GetConversion()
		{
			if (_data == null)
			{
				_data = new Dictionary<MazeItem, char>();
				_data.Add(MazeItem.wall, 'X');
				_data.Add(MazeItem.empty, ' ');
				_data.Add(MazeItem.crate, 'o');
				_data.Add(MazeItem.target, '.');
				//pushers
				_data.Add(MazeItem.push_up, '1');
				_data.Add(MazeItem.push_down, '2');
				_data.Add(MazeItem.push_left, '3');
				_data.Add(MazeItem.push_right, '4');
				//pushers pressed
				_data.Add(MazeItem.pushpressed_up, '5');
				_data.Add(MazeItem.pushpressed_down, '6');
				_data.Add(MazeItem.pushpressed_left, '7');
				_data.Add(MazeItem.pushpressed_right, '8');
				//when pusshers are open
				_data.Add(MazeItem.open_push_up, '[');
				_data.Add(MazeItem.open_push_down, ']');
				_data.Add(MazeItem.open_push_left, '<');
				_data.Add(MazeItem.open_push_right, '>');
			}
			return _data;
		}

		public static MazeItem FromString(char c)
		{
			var conv = GetConversion();
			foreach (var k in conv)
				if (k.Value.ToString().ToLower() == c.ToString().ToLower())
					return k.Key;
			throw new ArgumentOutOfRangeException($"{c} is not a valid char for building the maze");
		}

		public static char ToChar(MazeItem c)
		{
			var conv = GetConversion();
			if (!conv.ContainsKey(c))
				throw new ArgumentOutOfRangeException($"{c} is not a valid MazeItem");
			return conv[c];
		}
	}


	public class Maze
	{
		//our data
		private List<List<MazeItem>> data = new List<List<MazeItem>>();
		public IReadOnlyList<IReadOnlyList<MazeItem>> GetData() { return data; }

		//a string representation, to compare it faster
		//if null, must be rebuilt
		private string StringRepresentation2 = null;
		public string GetRepresentation()
		{
			if (StringRepresentation2 == null)
			{
				StringRepresentation2 = ToString();
			}
			return StringRepresentation2;
		}

		//print it (enter seprated)
		public override string ToString()
		{
			/*
			 * no stringbouldier: 
			 * 00:00:08.3074953
			 * wiht stringbuilder:
			 * 00:00:07.9234735
			 * 10 nestigns
			 * ..\..\data\genius131_data.txt ..\..\data\genius131_target.txt
			 * */
			/*
		   StringRepresentation = "";
		   foreach (var r in data)
		   {
			   foreach (var c in r)
			   {
				   StringRepresentation += MazeItemConvertion.ToString(c);
			   }
			   StringRepresentation += "-\n\r";
		   }
		   */
			StringBuilder builder = new StringBuilder((2 + data.Count()) * (4 + data[0].Count()));
			foreach (var r in data)
			{
				foreach (var c in r)
				{
					builder.Append(MazeItemConvertion.ToChar(c));
				}
				builder.Append("\n");
			}
			return builder.ToString();
		}


		//loading it, error message thowrs exception
		public void Load(IEnumerable<string> src)
		{
			data = new List<List<MazeItem>>();
			foreach (var s in src)
			{
				List<MazeItem> row = new List<MazeItem>();
				foreach (var c in s)
				{
					row.Add(MazeItemConvertion.FromString(c));

				}
				data.Add(row);
			}

			//check it is square
			foreach (var i in data)
			{
				if (i.Count() != data[0].Count())
					throw new ArgumentOutOfRangeException($"data is not square");
			}
		}

		//retruns a deep copy
		public Maze GetClone()
		{
			Maze ret = new Maze();
			ret.data = new List<List<MazeItem>>();
			foreach (var s in data)
			{
				List<MazeItem> row = new List<MazeItem>();
				foreach (var c in s)
				{
					row.Add(c);
				}
				ret.data.Add(row);
			}
			return ret;
		}

		//compares if it is the same make as another one
		public bool SameMaze(Maze other)
		{

			/*
			 * Trying to solve for 10 nesting levels took 00:00:02.9731781
			 * this one is faster!
			 * */
			if (this.GetRepresentation() == other.GetRepresentation())
				return true;
			return false;


			/*
			 * Trying to solve for 10 nesting levels took 00:00:03.1231871
			if (this.GetRepresentation().Equals(other.GetRepresentation()))
				return true;
			return false;
			*/

		}

		//checks if it is solved
		//all targets on the original must be with crates
		public bool Solved(Maze maze)
		{
			for (int r = 0; r < maze.GetData().Count(); r++)
			{
				IReadOnlyList<MazeItem> row = maze.GetData()[r];
				for (int c = 0; c < row.Count(); c++)
				{
					var mi = maze.GetData()[r][c];
					//se if it is a target
					if (mi == MazeItem.target)
					{
						//must have a crate now
						if (data[r][c] != MazeItem.crate)
							return false;
					}
				}
			}

			//didknt find any targets without crates
			return true;
		}

		//presses a button
		public void PressButton(int r, int c)
		{
			//we will change it!
			StringRepresentation2 = null;

			{
				MazeItem mi = data[r][c];
				bool valid = false;
				if (mi == MazeItem.push_down ||
					mi == MazeItem.push_left ||
					mi == MazeItem.push_right ||
					mi == MazeItem.push_up ||
					mi == MazeItem.pushpressed_down ||
					mi == MazeItem.pushpressed_left ||
					mi == MazeItem.pushpressed_right ||
					mi == MazeItem.pushpressed_up)
				{
					valid = true;
				}
				if (!valid)
					throw new InvalidOperationException("item in maze cannot be pressed");
			}

			/*
			 * r,c = index of pusher
			 * rdir, cdir = -1 or +1 or 0, direction in which to move
			 * rmax, cmax = maximmum
			 * movefill = what to place on empty space
			 */
			int rdir = 0;
			int cdir = 0;
			int rmax = data.Count();
			int cmax = data[0].Count();
			MazeItem movefill = MazeItem.empty;
			MazeItem moveclosing = MazeItem.empty;

			{
				MazeItem mi = data[r][c];
				if (mi == MazeItem.push_down)
				{
					rdir = 1;
					movefill = MazeItem.open_push_down;
					data[r][c] = MazeItem.pushpressed_down;
				}
				if (mi == MazeItem.push_left)
				{
					cdir = -1;
					movefill = MazeItem.open_push_left;
					data[r][c] = MazeItem.pushpressed_left;
				}
				if (mi == MazeItem.push_right)
				{
					cdir = 1;
					movefill = MazeItem.open_push_right;
					data[r][c] = MazeItem.pushpressed_right;
				}
				if (mi == MazeItem.push_up)
				{
					rdir = -1;
					movefill = MazeItem.open_push_up;
					data[r][c] = MazeItem.pushpressed_up;
				}
				if (mi == MazeItem.pushpressed_down)
				{
					rdir = 1;
					moveclosing = MazeItem.open_push_down;
					data[r][c] = MazeItem.push_down;
				}
				if (mi == MazeItem.pushpressed_left)
				{
					cdir = -1;
					moveclosing = MazeItem.open_push_left;
					data[r][c] = MazeItem.push_left;
				}
				if (mi == MazeItem.pushpressed_right)
				{
					cdir = 1;
					moveclosing = MazeItem.open_push_right;
					data[r][c] = MazeItem.push_right;
				}
				if (mi == MazeItem.pushpressed_up)
				{
					rdir = -1;
					moveclosing = MazeItem.open_push_up;
					data[r][c] = MazeItem.push_up;
				}

				if (rdir == 0 && cdir == 0)
					throw new InvalidOperationException("PressButton logic error");
			}

			/*
			 * after a lot of setup...
			 * */

			//have to make the first move
			r += rdir;
			c += cdir;
			while (r >= 0 && r < rmax && c >= 0 && c < cmax)
			{
				//if filling wiht empty, then we are closing, rules are different
				if (movefill == MazeItem.empty)
				{
					//a stop point?
					MazeItem mi = data[r][c];
					bool stop = true;
					if (mi == moveclosing)
					{
						stop = false;
					}
					if (stop)
						break;

					data[r][c] = movefill;
				}
				{
					//opening a push button

					//a stop point?
					MazeItem mi = data[r][c];
					bool stop = true;
					if (mi == MazeItem.empty ||
						mi == MazeItem.crate ||
						mi == MazeItem.push_down||
						mi == MazeItem.push_left ||
						mi == MazeItem.push_right||
						mi == MazeItem.push_up||
						mi == MazeItem.target)
					{
						stop = false;
					}
					if (stop)
						break;

					//if it is a crate, must move it 
					//also move closed buttons
					if (mi == MazeItem.crate ||
						mi == MazeItem.push_down ||
						mi == MazeItem.push_left ||
						mi == MazeItem.push_right ||
						mi == MazeItem.push_up)
					{
						int rnew = r;
						int cnew = c;
						rnew += rdir;
						cnew += cdir;
						if (rnew >= 0 && rnew < rmax && cnew >= 0 && cnew < cmax)
						{
							bool stop2 = true;
							MazeItem mi2 = data[rnew][cnew];
							if (mi2 == MazeItem.empty ||
								//a crate also stops on crate
								//and on buttons
								//mi == MazeItem.crate ||
								mi2 == MazeItem.target)
							{
								stop2 = false;
							}
							if (stop2)
								break;

							data[rnew][cnew] = mi;
							//not needed because done below...
							//data[r][c]=movefill;
						}
						else
						{
							//stop moving, no further space for the crate
							break;
						}
					}

					data[r][c] = movefill;
				}

				//next
				r += rdir;
				c += cdir;
			}
		}
	}
}

// tet_engine Version 1.0

// Rev. started on 2007.01.02
// Rev. completed on 2007.01.06
// This has been compiled with MS Visual Studio 8 (2005), Dev-Cpp 4.9.9.2, and OpenWatcom 1.6
// This should work with Borland and Cygwin
// This can be modified to compile for a Pocket PC in MSVC8 by changing srand(time(NULL)) to 
// srand(GetTickCount())

using System;

namespace csFallingBlocks
{
	class TetEngine
	{
		// private classes for current block (cb) and new block (newb)
		BlockData cb, newb;

		// private square position, don't ever assume what is in here is what you
		// want
		SquarePos sp;

		public int width = 16;
		public int height = 30;
		public int nb_width = 5;
		public int nb_height = 3;

		// this is the array the has all the squares in it that are not moving
		// two arrays were used so that collision detection would be easier to 
		// implement
		int[,] game_field;//[16][30];//(const)(X_MAX + 1)][(const)(Y_MAX + 1)];

		// array for the current, moving block
		int[,] current_block_field;//[16][30];//(const)(X_MAX + 1)][(const)(Y_MAX + 1)];


		// main array size
		public int X_MAX, Y_MAX;

		// counter to determine when the next level is
		public int next_level;

		// main game array, you need to draw this somehow to display the game
		// draw a different colored square (7 totoal) for each array value
		// [0][0] is top left, [X_MAX][Y_MAX] is bottom right
		// if an element is 0, it should be left blank
		public int[,] game_array;//[16][30];

		// same thing as main array only smaller for the next block display
		public int[,] next_block_array;//[5][3];

		// self explanatory
		public int score, level;
		public int next_block;

		// Is this in ms?
		// block drop timer
		public uint tmr_value;
		public bool quit, new_level, done_down_all;

		Random rnd;

		public TetEngine()
		{
			init();
		}

		void init()
		{
			X_MAX = 15; Y_MAX = 29;

			rnd = new Random();

			game_field = new int[width, height];
			current_block_field = new int[width, height];
			game_array = new int[width, height];
			next_block_array = new int[nb_width, nb_height];

			// zero game fields
			int i = 0, j = 0;
			while(j <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					game_field[i, j] = 0;
					i++;
				}
				i = 0;
				j++;
			}
			i = 0; j = 0;
			while(j <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					if(current_block_field[i, j] != 0)
						current_block_field[i, j] = 0;
					i++;
				}
				i = 0;
				j++;
			}

			// get first and second blocks
			newb.type = rnd.Next(1, 7);
			cb.type = newb.type;
			newb.type = rnd.Next(1, 7);
			cb.x = 7;
			cb.y = 1;
			cb.rot = 1;
			sp = find_square_pos(cb);
			current_block_field[sp.x1, sp.y1] = cb.type;
			current_block_field[sp.x2, sp.y2] = cb.type;
			current_block_field[sp.x3, sp.y3] = cb.type;
			current_block_field[sp.x4, sp.y4] = cb.type;

			update_next_block();

			// init other stuff
			level = 0;
			score = 0;
			tmr_value = 600;
			next_level = 10;
			quit = false;
			new_level = false;
			done_down_all = false;
		}

		public void update_next_block()
		{
			SquarePos sp1;
			BlockData n = new BlockData();
			n.type = newb.type;
			n.x = 2;
			n.y = 1;
			n.rot = 1;
			int i = 0, j = 0;
			sp1 = find_square_pos(n);

			// zero the array
			while(j < 3)
			{
				while(i < 5)
				{
					next_block_array[i, j] = 0;
					++i;
				}
				i = 0;
				++j;
			}
			next_block_array[sp1.x1, sp1.y1] = n.type;
			next_block_array[sp1.x2, sp1.y2] = n.type;
			next_block_array[sp1.x3, sp1.y3] = n.type;
			next_block_array[sp1.x4, sp1.y4] = n.type;
		}

		public void update_game_array()
		{
			int i = 0, j = 0;
			while(j <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					game_array[i, j] = game_field[i, j];
					i++;
				}
				i = 0;
				j++;
			}
			i = 0; j = 0;
			while(j <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					if(current_block_field[i, j] != 0)
						game_array[i, j] = current_block_field[i, j];
					i++;
				}
				i = 0;
				j++;
			}
		}

		// fills random junk in the bottom
		public void fill_rows(int rows, int density)
		{
			int i = 0, j = 0, d, r;

			// limits check
			if(density > 9)
				d = 9;
			else if(density < 1)
				d = 1;
			else
				d = density;

			// limits check
			if(rows < 0)
				r = 0;
			else if(rows > Y_MAX)
				r = Y_MAX;
			else
				r = rows;

			// first move stuff up the number of rows to create space
			while((j + r) <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					game_field[i, j] = game_field[i, j + r];
					i++;
				}
				i = 0;
				j++;
			}

			// fill random squares in
			i = 0; j = Y_MAX - r + 1;
			while(j <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					if(rnd.Next(1, 10) < d)
						game_field[i, j] = rnd.Next(1, 7);
					else
						game_field[i, j] = 0;
					i++;
				}
				i = 0;
				j++;
			}
		}

		bool collision(BlockData nb)
		{
			SquarePos np;
			np = find_square_pos(nb);

			// check left
			if(!((np.x1 >= 0) && (np.x2 >= 0) && (np.x3 >= 0) && (np.x4 >= 0)))
				return true;

			// check right
			if(!((np.x1 <= X_MAX) && (np.x2 <= X_MAX) && (np.x3 <= X_MAX) && (np.x4 <= X_MAX)))
				return true;

			// check top
			if(!((np.y1 >= 0) && (np.y2 >= 0) && (np.y3 >= 0) && (np.y4 >= 0)))
				return true;

			// check bottom
			if(!((np.y1 <= Y_MAX) && (np.y2 <= Y_MAX) && (np.y3 <= Y_MAX) && (np.y4 <= Y_MAX)))
				return true;

			// still ok?, then check game_field
			if(!((game_field[np.x1, np.y1] == 0) &&
				 (game_field[np.x2, np.y2] == 0) &&
				 (game_field[np.x3, np.y3] == 0) &&
				 (game_field[np.x4, np.y4] == 0)))
				return true;
			else
				return false;
		}

		void move_left()
		{
			SquarePos sp, np;
			BlockData nb; // next block position
			nb = cb;
			nb.x = cb.x - 1;

			if(collision(nb))
				return;
			else
			{
				sp = find_square_pos(cb);
				np = find_square_pos(nb);

				current_block_field[sp.x1, sp.y1] = 0;
				current_block_field[sp.x2, sp.y2] = 0;
				current_block_field[sp.x3, sp.y3] = 0;
				current_block_field[sp.x4, sp.y4] = 0;

				current_block_field[np.x1, np.y1] = nb.type;
				current_block_field[np.x2, np.y2] = nb.type;
				current_block_field[np.x3, np.y3] = nb.type;
				current_block_field[np.x4, np.y4] = nb.type;

				cb = nb;
			}
		}
		void move_right()
		{
			SquarePos sp, np;
			BlockData nb; // next block position
			nb = cb;
			nb.x = cb.x + 1;

			if(collision(nb))
				return;
			else
			{
				sp = find_square_pos(cb);
				np = find_square_pos(nb);

				current_block_field[sp.x1, sp.y1] = 0;
				current_block_field[sp.x2, sp.y2] = 0;
				current_block_field[sp.x3, sp.y3] = 0;
				current_block_field[sp.x4, sp.y4] = 0;

				current_block_field[np.x1, np.y1] = nb.type;
				current_block_field[np.x2, np.y2] = nb.type;
				current_block_field[np.x3, np.y3] = nb.type;
				current_block_field[np.x4, np.y4] = nb.type;

				cb = nb;
			}
		}
		void move_up()
		{
			SquarePos sp, np;
			BlockData nb; // next block position
			nb = cb;
			nb.y = cb.y - 1;

			if(collision(nb))
				return;
			else
			{
				sp = find_square_pos(cb);
				np = find_square_pos(nb);

				current_block_field[sp.x1, sp.y1] = 0;
				current_block_field[sp.x2, sp.y2] = 0;
				current_block_field[sp.x3, sp.y3] = 0;
				current_block_field[sp.x4, sp.y4] = 0;

				current_block_field[np.x1, np.y1] = nb.type;
				current_block_field[np.x2, np.y2] = nb.type;
				current_block_field[np.x3, np.y3] = nb.type;
				current_block_field[np.x4, np.y4] = nb.type;

				cb = nb;
			}
		}
		void move_rot()
		{
			//
			SquarePos sp, np;
			BlockData nb; // next block position
			nb = cb;
			if(cb.rot < 4)
				nb.rot = cb.rot + 1;
			else if(cb.rot == 4)
				nb.rot = 1;

			if(collision(nb))
				return;
			else
			{
				sp = find_square_pos(cb);
				np = find_square_pos(nb);

				current_block_field[sp.x1, sp.y1] = 0;
				current_block_field[sp.x2, sp.y2] = 0;
				current_block_field[sp.x3, sp.y3] = 0;
				current_block_field[sp.x4, sp.y4] = 0;

				current_block_field[np.x1, np.y1] = nb.type;
				current_block_field[np.x2, np.y2] = nb.type;
				current_block_field[np.x3, np.y3] = nb.type;
				current_block_field[np.x4, np.y4] = nb.type;

				cb = nb;
			}
		}

		// moves the current block all the way down
		// don't call if done_down_all is true!
		void full_down()
		{
			if(!done_down_all)
				while(!move_down()) ;
			{ }
			done_down_all = true;
		}

		bool move_down()
		{
			//
			SquarePos sp, np;
			BlockData nb; // next block position
			nb = cb;
			nb.y = cb.y + 1;

			if(collision(nb))
			{
				new_block();
				return true;
			}
			else
			{
				sp = find_square_pos(cb);
				np = find_square_pos(nb);

				current_block_field[sp.x1, sp.y1] = 0;
				current_block_field[sp.x2, sp.y2] = 0;
				current_block_field[sp.x3, sp.y3] = 0;
				current_block_field[sp.x4, sp.y4] = 0;

				current_block_field[np.x1, np.y1] = nb.type;
				current_block_field[np.x2, np.y2] = nb.type;
				current_block_field[np.x3, np.y3] = nb.type;
				current_block_field[np.x4, np.y4] = nb.type;

				cb = nb;
				return false;
			}
		}

		void new_block()
		{
			// move the current block to the game_field array
			SquarePos sp;
			sp = find_square_pos(cb);
			game_field[sp.x1, sp.y1] = cb.type;
			game_field[sp.x2, sp.y2] = cb.type;
			game_field[sp.x3, sp.y3] = cb.type;
			game_field[sp.x4, sp.y4] = cb.type;

			current_block_field[sp.x1, sp.y1] = 0;
			current_block_field[sp.x2, sp.y2] = 0;
			current_block_field[sp.x3, sp.y3] = 0;
			current_block_field[sp.x4, sp.y4] = 0;

			// change the current block and next block types
			cb.type = newb.type;
			newb.type = rnd.Next(1, 7);

			// set the start position
			cb.rot = 1;
			cb.x = 7;
			cb.y = 1;

			// if we have a collision, post quit and get out
			if(collision(cb))
			{
				quit = true;
				return;
			}

			// no collision so display the current block
			sp = find_square_pos(cb);
			current_block_field[sp.x1, sp.y1] = cb.type;
			current_block_field[sp.x2, sp.y2] = cb.type;
			current_block_field[sp.x3, sp.y3] = cb.type;
			current_block_field[sp.x4, sp.y4] = cb.type;

			score += 1;
			update_next_block();
			check_full_row();
		}

		void check_full_row()
		{
			int i = 0, j = 0, RowsRemoved = 0;
			while(j <= Y_MAX)
			{
				while(i <= X_MAX)
				{
					// there is a zero in the array
					if(game_field[i, j] == 0)
						break;
					else
						i++;
				}
				if(i > X_MAX)
				{
					remove_row(j);
					RowsRemoved++;
				}
				i = 0;
				j++;
			}

			// done checking, modify some variables
			level = level + RowsRemoved;
			score = score + RowsRemoved * 10;
			if(level >= next_level)
			{
				next_level += 10;
				tmr_value = (uint)(tmr_value * 0.85f);
				new_level = true;
			}
		}

		void remove_row(int row)
		{
			// local variables
			int i = 0, j;

			// assign a value to j
			if((row <= Y_MAX) && (row >= 1))
				j = row;
			else if(row > Y_MAX)
				j = Y_MAX;
			else
				j = 1;

			// move rows down deleting the row at row
			while(true)
			{
				//
				game_field[i, j] = game_field[i, j - 1];
				if((i >= X_MAX) && (j != 1))
				{
					i = 0;
					--j;
				}
				else if(i < X_MAX)
					++i;
				else if(j == 1)
					break;
			}
		}


		public SquarePos find_square_pos(BlockData b)
		{
			// -y is up
			// -x is left
			SquarePos sp = new SquarePos();
			// L block
			if(b.type == 1)
			{
				// * * *
				// *
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x + 2; sp.y4 = b.y;
				}
				// * *
				//   *
				//   *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x; sp.y3 = b.y - 2;
					sp.x4 = b.x - 1; sp.y4 = b.y;
				}
				//	 *
				// * * *
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x - 2; sp.y4 = b.y;
				}
				// *
				// *
				// * *
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x; sp.y4 = b.y + 2;
				}
			}
			// -y is up
			// -x is left
			else if(b.type == 2)
			{
				// * * *
				//	 *
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x - 2; sp.y4 = b.y;
				}
				//   *
				//   *
				// * *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x; sp.y3 = b.y + 2;
					sp.x4 = b.x - 1; sp.y4 = b.y;
				}
				// *
				// * * *
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x + 2; sp.y4 = b.y;
				}
				// * *
				// *
				// *  
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x; sp.y4 = b.y - 2;
				}
			}
			// -y is up
			// -x is left
			else if(b.type == 3)
			{
				// * * * *
				//
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x + 1; sp.y2 = b.y;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x + 2; sp.y4 = b.y;
				}
				//   *
				//   *
				//   *
				//   *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x; sp.y3 = b.y - 2;
					sp.x4 = b.x; sp.y4 = b.y + 1;
				}
				// * * * *
				//
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x + 1; sp.y2 = b.y;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x + 2; sp.y4 = b.y;
				}
				//   * 
				//   *
				//   *
				//   *
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x; sp.y3 = b.y - 2;
					sp.x4 = b.x; sp.y4 = b.y + 1;
				}
			}
			// -y is up
			// -x is left
			else if(b.type == 4)
			{
				// * *
				// * *
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y - 1;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
				// * *
				// * *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y - 1;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
				// * *
				// * *
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y - 1;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
				// * *
				// * *
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y - 1;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
			}
			// -y is up
			// -x is left
			else if(b.type == 5)
			{
				// * * *
				//   *
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
				//   *
				// * *
				//   *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x; sp.y3 = b.y - 1;
					sp.x4 = b.x - 1; sp.y4 = b.y;
				}
				//   *
				// * * *
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x - 1; sp.y4 = b.y;
				}
				// *
				// * *
				// *
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x; sp.y4 = b.y + 1;
				}
			}
			// -y is up
			// -x is left
			else if(b.type == 6)
			{
				// * *
				//   * *
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x + 1; sp.y4 = b.y + 1;
				}
				//   *
				// * *
				// *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x - 1; sp.y2 = b.y;
					sp.x3 = b.x; sp.y3 = b.y - 1;
					sp.x4 = b.x - 1; sp.y4 = b.y + 1;
				}
				// * *
				//   * *
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x - 1; sp.y3 = b.y;
					sp.x4 = b.x + 1; sp.y4 = b.y + 1;
				}
				//   *
				// * *
				// *
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x - 1; sp.y2 = b.y;
					sp.x3 = b.x; sp.y3 = b.y - 1;
					sp.x4 = b.x - 1; sp.y4 = b.y + 1;
				}
			}
			// -y is up
			// -x is left
			else if(b.type == 7)
			{
				//   * *
				// * * 
				if(b.rot == 1)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x - 1; sp.y4 = b.y + 1;
				}
				// *
				// * *
				//   *
				else if(b.rot == 2)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y + 1;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
				//   * *
				// * *
				else if(b.rot == 3)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y + 1;
					sp.x3 = b.x + 1; sp.y3 = b.y;
					sp.x4 = b.x - 1; sp.y4 = b.y + 1;
				}
				// *
				// * *
				//   *
				else if(b.rot == 4)
				{
					sp.x1 = b.x; sp.y1 = b.y;
					sp.x2 = b.x; sp.y2 = b.y - 1;
					sp.x3 = b.x + 1; sp.y3 = b.y + 1;
					sp.x4 = b.x + 1; sp.y4 = b.y;
				}
			}
			return sp;
		}
	}
}


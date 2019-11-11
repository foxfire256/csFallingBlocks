// tet_engine Version 1.0

// Rev. started on 2007.01.02
// Rev. completed on 2007.01.06
// This has been compiled with MS Visual Studio 8 (2005), Dev-Cpp 4.9.9.2, and OpenWatcom 1.6
// This should work with Borland and Cygwin
// This can be modified to compile for a Pocket PC in MSVC8 by changing srand(time(NULL)) to 
// srand(GetTickCount())

namespace csFallingBlocks
{
	class TetEngine
	{
		// private classes for current block (cb) and new block (newb)
		BlockData cb, newb;
		
		// private square position, don't ever assume what is in here is what you
		// want
		SquarePos sp;
		
		// this is the array the has all the squares in it that are not moving
		// two arrays were used so that collision detection would be easier to 
		// implement
		//int game_field[16][30];//(const)(X_MAX + 1)][(const)(Y_MAX + 1)];
		
		// array for the current, moving block
		//int current_block_field[16][30];//(const)(X_MAX + 1)][(const)(Y_MAX + 1)];
		
		
		// main array size
		public int X_MAX, Y_MAX;
		
		// counter to determine when the next level is
		public int next_level;
		
		// main game array, you need to draw this somehow to display the game
		// draw a different colored square (7 totoal) for each array value
		// [0][0] is top left, [X_MAX][Y_MAX] is bottom right
		// if an element is 0, it should be left blank
		//public int game_array[16][30];
		
		// same thing as main array only smaller for the next block display
		//public int next_block_array[5][3];
		
		// self explanatory
		public int score, level;
		public int next_block;
		
		// Is this in ms?
		// block drop timer
		public uint tmr_value;
		public bool quit, new_level, done_down_all;
	}
}
/*
tet_engine::tet_engine()
{
	init();
}

void tet_engine::init()
{
	X_MAX = 15; Y_MAX = 29;
	
	// zero game fields
	int i = 0, j = 0;
	while(j <= Y_MAX)
	{
		while(i <= X_MAX)
		{
			game_field[i][j] = 0;
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
			if(current_block_field[i][j] != 0)
				current_block_field[i][j] = 0;
			i++;
		}
		i = 0;
		j++;
	}
	
	// get first and second blocks
	srand(time(nullptr));
	newb.type = get_rand(7);
	cb.type = newb.type;
	newb.type = get_rand(7);
	cb.x = 7;
	cb.y = 1;
	cb.rot = 1;
	sp = find_square_pos(cb);
	current_block_field[sp.x1][sp.y1] = cb.type;
	current_block_field[sp.x2][sp.y2] = cb.type;
	current_block_field[sp.x3][sp.y3] = cb.type;
	current_block_field[sp.x4][sp.y4] = cb.type;
	
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

int tet_engine::get_rand(int size)
{
	int n;
	n = (rand())/(RAND_MAX / size) + 1;
	// make sure (1 <= n <= size)
	if (n > size)
		n = size;
	else if (n < 1)
		n = 1;
	return n;
}

void tet_engine::update_next_block()
{
	square_pos sp1;
	block_data n;
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
			next_block_array[i][j] = 0;
			++i;
		}
		i = 0;
		++j;
	}
	next_block_array[sp1.x1][sp1.y1] = n.type;
	next_block_array[sp1.x2][sp1.y2] = n.type;
	next_block_array[sp1.x3][sp1.y3] = n.type;
	next_block_array[sp1.x4][sp1.y4] = n.type;
}

void tet_engine::update_game_array()
{
	int i = 0, j = 0;
	while(j <= Y_MAX)
	{
		while(i <= X_MAX)
		{
			game_array[i][j] = game_field[i][j];
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
			if(current_block_field[i][j] != 0)
				game_array[i][j] = current_block_field[i][j];
			i++;
		}
		i = 0;
		j++;
	}
}

// fills random junk in the bottom
void tet_engine::fill_rows(int rows, int density)
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
			game_field[i][j] = game_field[i][j + r];
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
			if(get_rand(10) < d)
				game_field[i][j] = get_rand(7);
			else
				game_field[i][j] = 0;
			i++;
		}
		i = 0;
		j++;
	}
}

bool tet_engine::collision(block_data nb)
{
	square_pos np;
	np = find_square_pos(nb);

	// check left
	if(!((np.x1 >= 0)&&(np.x2 >= 0)&&(np.x3 >= 0)&&(np.x4 >= 0)))
		return true;

	// check right
	if(!((np.x1 <= X_MAX)&&(np.x2 <= X_MAX)&&(np.x3 <= X_MAX)&&(np.x4 <= X_MAX)))
		return true;

	// check top
	if(!((np.y1 >= 0)&&(np.y2 >= 0)&&(np.y3 >= 0)&&(np.y4 >= 0)))
		return true;

	// check bottom
	if(!((np.y1 <= Y_MAX)&&(np.y2 <= Y_MAX)&&(np.y3 <= Y_MAX)&&(np.y4 <= Y_MAX)))
		return true;

	// still ok?, then check game_field
	if(!((game_field[np.x1][np.y1] == 0) &&
		 (game_field[np.x2][np.y2] == 0) &&
		 (game_field[np.x3][np.y3] == 0) &&
		 (game_field[np.x4][np.y4] == 0)))
		return true;
	else
		return false;
}


void tet_engine::move_left()
{
	square_pos sp, np;
	block_data nb; // next block position
	nb = cb;
	nb.x = cb.x - 1;
	
	if(collision(nb))
		return;
	else
	{
		sp = find_square_pos(cb);
		np = find_square_pos(nb);

		current_block_field[sp.x1][sp.y1] = 0;
		current_block_field[sp.x2][sp.y2] = 0;
		current_block_field[sp.x3][sp.y3] = 0;
		current_block_field[sp.x4][sp.y4] = 0;

		current_block_field[np.x1][np.y1] = nb.type;
		current_block_field[np.x2][np.y2] = nb.type;
		current_block_field[np.x3][np.y3] = nb.type;
		current_block_field[np.x4][np.y4] = nb.type;
	 
		cb = nb;
	}
}
void tet_engine::move_right()
{
	square_pos sp, np;
	block_data nb; // next block position
	nb = cb;
	nb.x = cb.x + 1;

	if(collision(nb))
		return;
	else
	{
		sp = find_square_pos(cb);
		np = find_square_pos(nb);

		current_block_field[sp.x1][sp.y1] = 0;
		current_block_field[sp.x2][sp.y2] = 0;
		current_block_field[sp.x3][sp.y3] = 0;
		current_block_field[sp.x4][sp.y4] = 0;
		 
		current_block_field[np.x1][np.y1] = nb.type;
		current_block_field[np.x2][np.y2] = nb.type;
		current_block_field[np.x3][np.y3] = nb.type;
		current_block_field[np.x4][np.y4] = nb.type;
		 
		cb = nb;
	}
}
void tet_engine::move_up()
{
	square_pos sp, np;
	block_data nb; // next block position
	nb = cb;
	nb.y = cb.y - 1;

	if(collision(nb))
		return;
	else
	{
		sp = find_square_pos(cb);
		np = find_square_pos(nb);

		current_block_field[sp.x1][sp.y1] = 0;
		current_block_field[sp.x2][sp.y2] = 0;
		current_block_field[sp.x3][sp.y3] = 0;
		current_block_field[sp.x4][sp.y4] = 0;
		 
		current_block_field[np.x1][np.y1] = nb.type;
		current_block_field[np.x2][np.y2] = nb.type;
		current_block_field[np.x3][np.y3] = nb.type;
		current_block_field[np.x4][np.y4] = nb.type;
		 
		cb = nb;
	}
}
void tet_engine::move_rot()
{
	//
	square_pos sp, np;
	block_data nb; // next block position
	nb = cb;
	if (cb.rot < 4)
		nb.rot = cb.rot + 1;
	else if (cb.rot == 4)
		nb.rot = 1;
	
	if(collision(nb))
		return;
	else
	{
		sp = find_square_pos(cb);
		np = find_square_pos(nb);

		current_block_field[sp.x1][sp.y1] = 0;
		current_block_field[sp.x2][sp.y2] = 0;
		current_block_field[sp.x3][sp.y3] = 0;
		current_block_field[sp.x4][sp.y4] = 0;
		 
		current_block_field[np.x1][np.y1] = nb.type;
		current_block_field[np.x2][np.y2] = nb.type;
		current_block_field[np.x3][np.y3] = nb.type;
		current_block_field[np.x4][np.y4] = nb.type;
		 
		cb = nb;
	}
}

// moves the current block all the way down
// don't call if done_down_all is true!
void tet_engine::full_down(void)
{
	if(!done_down_all)
		while(!move_down()); {}
	done_down_all = true;
}

bool tet_engine::move_down()
{
	//
	square_pos sp, np;
	block_data nb; // next block position
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

		current_block_field[sp.x1][sp.y1] = 0;
		current_block_field[sp.x2][sp.y2] = 0;
		current_block_field[sp.x3][sp.y3] = 0;
		current_block_field[sp.x4][sp.y4] = 0;
		
		current_block_field[np.x1][np.y1] = nb.type;
		current_block_field[np.x2][np.y2] = nb.type;
		current_block_field[np.x3][np.y3] = nb.type;
		current_block_field[np.x4][np.y4] = nb.type;
		
		cb = nb;
		return false;
	}
}

void tet_engine::new_block()
{
	// move the current block to the game_field array
	square_pos sp;
	sp = find_square_pos(cb);
	game_field[sp.x1][sp.y1] = cb.type;
	game_field[sp.x2][sp.y2] = cb.type;
	game_field[sp.x3][sp.y3] = cb.type;
	game_field[sp.x4][sp.y4] = cb.type;
	 
	current_block_field[sp.x1][sp.y1] = 0;
	current_block_field[sp.x2][sp.y2] = 0;
	current_block_field[sp.x3][sp.y3] = 0;
	current_block_field[sp.x4][sp.y4] = 0;
	
	// change the current block and next block types
	cb.type = newb.type;
	newb.type = get_rand(7);
	
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
	current_block_field[sp.x1][sp.y1] = cb.type;
	current_block_field[sp.x2][sp.y2] = cb.type;
	current_block_field[sp.x3][sp.y3] = cb.type;
	current_block_field[sp.x4][sp.y4] = cb.type;

	score += 1;
	update_next_block();
	check_full_row();
}

void tet_engine::check_full_row(void)
{
	int i = 0, j = 0, RowsRemoved = 0;
	while(j <= Y_MAX)
	{
		while(i <= X_MAX)
		{
			// there is a zero in the array
			if(!game_field[i][j])
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
	if (level >= next_level)
	{
		next_level += 10;
		tmr_value = (unsigned int)(tmr_value * 0.85f);
		new_level = true;
	}
}

void tet_engine::remove_row(int row)
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
	while (1)
	{
		//
		game_field[i][j] = game_field[i][j - 1];
		if ((i >= X_MAX)&&(j != 1))
		{
			i = 0;
			--j;
		}
		else if (i < X_MAX)
			++i;
		else if (j == 1)
			break;
	}
}
/*
void tet_engine::check_full_row(void)
{
	//
	int x, y, y1;
	bool RowIsFull, DoneChecking, DoneMoving, DoneRowCheck;
	int rowsRemoved;
	DoneChecking = false;
	y = Y_MAX;
	x = 0;
	int i = 0;
	//long row_result = 1;
	rowsRemoved = 0;
	while (!DoneChecking)
	{
		RowIsFull = false;
		DoneMoving = false;
		DoneRowCheck = false;
		
		// find first full row
		while ((!RowIsFull)&&(!DoneRowCheck)) 
		{
			i = 0;
			while((i <= X_MAX))
			{
				if(game_field[i][y])
					i++;
				else 
				{
					RowIsFull = false;
					break;
				}
				if(i > X_MAX)
					RowIsFull = true;
			}
			if (!RowIsFull)
				--y;
			if (y == 0)
				DoneRowCheck = true;
		} // end while row check
		if (RowIsFull)
		{
			//
			x = 0;
			y1 = y;
			rowsRemoved += 1;
			while (!DoneMoving)
			{
				//
				game_field[x][y1] = game_field[x][y1 - 1];
				if ((x >= X_MAX)&&(y1 != 1))
				{
					x = 0;
					--y1;
				}
				else if (x < X_MAX)
					++x;
				else if (y1 == 1)
					DoneMoving = true;
			}
		}
		else if (!RowIsFull)
			DoneChecking = true;
	} // end done checking
	level = level + rowsRemoved;
	score = score + rowsRemoved * 10;
	if (level >= next_level)
	{
		next_level += 10;
		tmr_value = (unsigned int)(tmr_value * 0.85f);
		new_level = true;
	}
}
*/
//------------------------------------------------------------------------------

/******************
 * Find Squares' 
 * Positions
 ******************/
/*
square_pos tet_engine::find_square_pos(block_data b)
{
	// -y is up
	// -x is left
	square_pos BlockI;
	// L block
	if (b.type == 1)
	{
		// * * *
		// *
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 2; BlockI.y4 = b.y;
		}
		// * *
		//   *
		//   *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y - 2;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y;
		}
		//	 *
		// * * *
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x - 2; BlockI.y4 = b.y;
		}
		// *
		// *
		// * *
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x;	 BlockI.y4 = b.y + 2;
		}
	}
	// -y is up
	// -x is left
	else if (b.type == 2)
	{
		// * * *
		//	 *
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x - 2; BlockI.y4 = b.y;
		}
		//   *
		//   *
		// * *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y + 2;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y;
		}
		// *
		// * * *
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 2; BlockI.y4 = b.y;
		}
		// * *
		// *
		// *  
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x;	 BlockI.y4 = b.y - 2;
		}
	}
	// -y is up
	// -x is left
	else if (b.type == 3)
	{
		// * * * *
		//
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x + 1; BlockI.y2 = b.y;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 2; BlockI.y4 = b.y;
		}
		//   *
		//   *
		//   *
		//   *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y - 2;
			BlockI.x4 = b.x;	 BlockI.y4 = b.y + 1;
		}
		// * * * *
		//
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x + 1; BlockI.y2 = b.y;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 2; BlockI.y4 = b.y;
		}
		//   * 
		//   *
		//   *
		//   *
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y - 2;
			BlockI.x4 = b.x;	 BlockI.y4 = b.y + 1;
		}
	}
	// -y is up
	// -x is left
	else if (b.type == 4)
	{
		// * *
		// * *
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
		// * *
		// * *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
		// * *
		// * *
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
		// * *
		// * *
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
	}
	// -y is up
	// -x is left
	else if (b.type == 5)
	{
		// * * *
		//   *
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
		//   *
		// * *
		//   *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y;
		}
		//   *
		// * * *
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y;
		}
		// *
		// * *
		// *
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x;	 BlockI.y4 = b.y + 1;
		}
	}
	// -y is up
	// -x is left
	else if (b.type == 6)
	{
		// * *
		//   * *
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y + 1;
		}
		//   *
		// * *
		// *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x - 1; BlockI.y2 = b.y;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y + 1;
		}
		// * *
		//   * *
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x - 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y + 1;
		}
		//   *
		// * *
		// *
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x - 1; BlockI.y2 = b.y;
			BlockI.x3 = b.x;	 BlockI.y3 = b.y - 1;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y + 1;
		}
	}
	// -y is up
	// -x is left
	else if (b.type == 7)
	{
		//   * *
		// * * 
		if (b.rot == 1)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y + 1;
		}
		// *
		// * *
		//   *
		else if (b.rot == 2)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y + 1;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
		//   * *
		// * *
		else if (b.rot == 3)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y + 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y;
			BlockI.x4 = b.x - 1; BlockI.y4 = b.y + 1;
		}
		// *
		// * *
		//   *
		else if (b.rot == 4)
		{
			BlockI.x1 = b.x;	 BlockI.y1 = b.y;
			BlockI.x2 = b.x;	 BlockI.y2 = b.y - 1;
			BlockI.x3 = b.x + 1; BlockI.y3 = b.y + 1;
			BlockI.x4 = b.x + 1; BlockI.y4 = b.y;
		}
	}
	return BlockI;
}
*/

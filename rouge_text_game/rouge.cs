using System;
// simple rouge nethack dungeon scrolling example
// no exception handling, no error checking, no input validation CAUSE NIGGA I"M LAZY
namespace dungeon_hack
{// add places been
	class MainClass
	{	//main classes main loop
		public static void Main (string[] args)
		{
			Console.WriteLine("How big of a game do you want to play?");
			int size = Int32.Parse(Console.ReadLine()) / 2 ;// need round function
			MapGenorator game01 = new MapGenorator(size,size);//create new map board (custom size)
			PlayerGenorator players = new PlayerGenorator();//create the characters
			game01.mSetSpace(0,0,players);// tell our game board to move to new square
			string direction = "right";
			int x=0,y=0;
			for ( int turns = 0; turns <= game01.mGetSize(); turns++ )//turned off for debugging change == to <=
			{
				//need to do bounds checking of movement
				Console.WriteLine("What direction do you want to go?");
				direction = Console.ReadLine();
				if ( direction.Equals("left")) y--; 
				if ( direction.Equals("right")) y++; 
				if ( direction.Equals("up")) x++; 
				if ( direction.Equals("down")) x--; 
				game01.mSetSpace(x,y,players);//no bounds checking
				//turn healing/regen player loop
				//mana isn't used
				for (int i=0; i < 4; i++)
				{
					if (players.mGetAttr(i,1) != players.mGetAttr(i,10))
					{
						players.mSetAttr(i,10,1);
					}
					if (players.mGetAttr(i,2) != players.mGetAttr(i,11))
					{
						players.mSetAttr(i,11,2);
					}
				}
				if (players.mGetAlive() <= 0 )
				{
					Console.WriteLine("All players dead, ending game.");
					break;
				}
			}

			Console.WriteLine (" You have " + players.mGetAlive() + " players alive upon completing the game.");//see how many players in group are still kicking
		}// WHOLE GAME LOGIC IS STORED IN ONLY TWO INTEGER ARRAYS, only one of which is 2 demensional
	}
	class MapGenorator
	{
		private static int [,] map;// multidemensional array holding our map/board
		public MapGenorator (int width, int length)//constructor for our map class (custom size one)
		{
			map = new int [width, length];
			Console.WriteLine ("We created your map based on your sizes!");
		}
		public MapGenorator ()// constructor used incase we don't manually set a size of the board
		{
			map = new int [10,10];
			Console.WriteLine ("Default map created.");
		}
		public void mSetSpace(int x, int y, PlayerGenorator oPlays)
		{// this class takes the cordinates of a place to move
			map[x,y] = 1;//then it calls an private class method to see if there is a monster there
			if ( mGetMonster() ) 
			{
				Console.WriteLine("There's a monster here!");
				Random ran = new Random();
				int MonsterHP = ran.Next(5,10),player=0,i=0, iMonsterAtk=0;
				
				while( i <= MonsterHP)
				{

					if ( oPlays.mCombat(MonsterHP) ) break;
					player=ran.Next(0,3);
					iMonsterAtk=oPlays.mMonsterAttack(0,MonsterHP);//dmg hp, returns hp
					
					oPlays.mSetAttr(player,10,(oPlays.mGetAttr(player,10)-iMonsterAtk));
					if (oPlays.mGetAttr(player,10) <= 0 ) Console.WriteLine("Player " + player + " is dead!");
					
					//combat loop
				}
			}
		}
		public int mGetSize()
		{// return board size
			return map.Length;
		}
		private bool mGetMonster ()
		{// create a new random object from the sdk
			Random rnd = new Random();
			int monster = rnd.Next(1, 10);// assign a random 0 to 10 mumber
			if (monster > 5 ) return true; //we find a monster if number is big
			else return false;// or get lucky if number is small
		}

	}
	class PlayerGenorator
	{//only allowed to have 3 players per group
		private static int [,] players = new int [4,12];
		/* Replace arrary with structure or enumerator
			0. class
			1. hit points
			2. mana points
			3. dexterity
			4. strength
			5. widsdom
			6. intelect
			7. consitution
			8. luck
			9. experience 
			10. alive (current hit points )
			11. power (current mana points)
		*/
		public PlayerGenorator ()
		{// custom constructor to create defaults on players
			Random seed = new Random();
			for (int i = 0; i < 4; i++ )
			{// AKA 1 is alive, 0 is dead
				Console.WriteLine("What class for player " + i + " would you like to be Warrior, Wizard, Priest, or Thief [1,2,3,4]?");
				players[i,0] = Int32.Parse(Console.ReadLine());
				players[i,6] = seed.Next(0,5);
				players[i,7] = seed.Next(0,5);
				players[i,3] = seed.Next(0,5);
				players[i,4] = seed.Next(0,5);
				players[i,5] = seed.Next(0,5);
				players[i,1] = seed.Next(0,5)*players[i,7];
				players[i,2] = seed.Next(0,5)*players[i,6];
				players[i,8] = seed.Next(0,3);
				players[i,9] = seed.Next(0,2)*players[i,8];
				players[i,10] = players[i,1];
				players[i,11] = players[i,2];

			}
			Console.WriteLine ("All three players are alive!");
		}
		public int mGetAttr ( int player, int attribute )
		{
			return players[player,attribute];
		}
		public void mSetAttr(int player, int attribute, int num )
		{
			players[player, attribute] = num;
		}
		public void mSetClass(int player, int profession)
		{
			players[player,0] = profession; 
		}
		public int mGetAlive()
		{
			int i=0, alive=0;
			while ( i < 4 )
			{
				if ( players[i,10] >= 1) alive++;
				i++;
			}
			if ( alive > 0 ) alive = alive + 1;
			return alive;
		}
		public bool mHitPlayer ( int player, int dmg )
		{
			Console.WriteLine("Player " + player + " hit for " + dmg + " damage");
			players[player,1] = players[player,1] - dmg;
			if ( players[player,1] < 0 ) return false;
			else return true;
		}
		public int mSpell (int player)
		{
			int dmg = 0;
			Random seed = new Random();
			if ( seed.Next(0,7) > 0 ) 
			{
				if (players[player,0] == 2 )
				{
					Console.WriteLine("Critical cast!");
					if (players[player,0] == 2 ) dmg = players[player,8] * seed.Next(0, players[player,6]);
					else dmg = players[player,8] * seed.Next(0, players[player,5]);
					if (players[player,0] == 2 )Console.WriteLine("Magic missle does " + dmg + " damage!");
					else Console.WriteLine("Heal group  does " + dmg + " hit points!");
				}
				else
				{
					if (players[player,0] == 2 ) dmg = seed.Next(0, players[player,6]);
					else dmg = seed.Next(0, players[player,5]);
					if (players[player,0] == 2 )Console.WriteLine("Magic missle does " + dmg + " damage!");
					else Console.WriteLine("Heal group  does " + dmg + " hit points!");
				}
			}
			return dmg;
		}
		public int mStab (int player)
		{
			int dmg = 0;
			Random seed = new Random();
			if ( seed.Next(0,7) > 0 ) 
			{
				if (players[player,0] == 2 )
				{
					Console.WriteLine("Critical stab!");
					dmg = players[player,8] * seed.Next(0, players[player,3]);
					Console.WriteLine("Back stab does " + dmg + " damage!");
				}
				else
				{
					dmg = seed.Next(0, players[player,3]);
					Console.WriteLine("Back stab does " + dmg + " damage!");
				}
			}
			return dmg;
		}
		public int mAttack (int player)
		{
			int dmg = 0;
			Random seed = new Random();
			int hit = seed.Next(0,7);
			if ( hit == 1 ) 
			{
				if (players[player,0] == 2 )
				{
					Console.WriteLine("Critical hit!");
					dmg = players[player,8] * seed.Next(0, players[player,4]);
					Console.WriteLine("Your attack does " + dmg + " damage!");
				}
				else
				{
					dmg = seed.Next(0, players[player,4]);
					Console.WriteLine("Your attack does " + dmg + " damage!");
				}
			}
			//Console.WriteLine(hit + "DMG " + dmg);
			return dmg;
		}
		public int mMonsterAttack ( int iMonsterDMG, int iMonsterHP )
		{
			Random ran = new Random();
			int HP = iMonsterHP - iMonsterDMG; 
			if ( HP <= 0 ) return -1;
			return ran.Next(0,5);
		}
		private static int iDmg=0;
		public bool mCombat (int iMonsterHP)
		{
			//cycle through players until monster is dead
			int iHeal=0,i=0;
			while ( i < 4 )
			{
				//cycle players
				double iMonsterDead = 0.0;
				double f = Convert.ToSingle(iDmg);
				double f2 = Convert.ToSingle(iMonsterHP);
				iMonsterDead = f2 - f;
				if ( iMonsterDead < 1 )
				{
					Console.WriteLine("in loop");
					//monster dead
					return true;
				}	

			//	Console.WriteLine("You are here." + mGetAttr(i,0) );
				if ( mGetAttr(i,0) == 1 ) 
				{
			//		Console.WriteLine("You are here.");
					iDmg = iDmg + mAttack(i);
				}
				if (mGetAttr(i,0) == 2 )
				{
					Console.WriteLine("Cast magic missle? 0 for no or 1 for yes.");
					int cast = Int32.Parse ( Console.ReadLine());
					if ( cast == 1 ) iDmg = iDmg + mSpell(i);
					else iDmg = iDmg + mAttack(i);
				}//no miscast
				if (mGetAttr(i,0) == 2 )
				{
					Console.WriteLine("Heal a player? 0 for no or 1 for yes.");						
					int cast = Int32.Parse ( Console.ReadLine());
					if ( cast == 1 ) iHeal = mSpell(i);
					else iDmg = iDmg + mAttack(i);
					if ( iHeal > 0 ) 
					{
						Console.WriteLine("Which player to heal? 0-3.");
						int healPlayer = Int32.Parse ( Console.ReadLine());
						mSetAttr ( healPlayer,10,(mGetAttr(healPlayer,10)) + iHeal);
						iHeal=0;
					}
					else Console.WriteLine("Miscast.");
				}
				if (mGetAttr(i,0) == 4 )
				{
					Console.WriteLine("Back stab? 0 for no or 1 for yes.");
					int cast = Int32.Parse ( Console.ReadLine());
					if ( cast == 1 ) iDmg = iDmg + mStab(i);
					else iDmg = iDmg + mAttack(i);
				}//no fail
				Console.WriteLine("Monster HP & damage done " + iMonsterHP + " " + iDmg );
				i++;
			}
			Console.WriteLine("End of combat");
			return false;
		}
	}
}

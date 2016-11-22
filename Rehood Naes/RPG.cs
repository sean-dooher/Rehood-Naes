﻿using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Rehood_Naes.Building;
using Rehood_Naes.Entities;

namespace Rehood_Naes
{
    /// <summary>
    /// Game runner
    /// </summary>
    public class RPG : Game
    {
    	#region Fields
        private Area currentArea;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private static bool loadNewGame;
        #endregion
        
        #region Properties
        /// <summary>
        /// Random number generator for game to use
        /// </summary>
        public static Random RNG;  
        /// <summary>
        /// Player the game uses
        /// </summary>
    	public static Player player; 	
    	/// <summary>
    	/// Content manager to load content
    	/// </summary>
    	public static ContentManager ContentManager;
    	/// <summary>
    	/// Graphics manager to draw game and viewport
    	/// </summary>
    	public static GraphicsDeviceManager GraphicsManager;	
    	/// <summary>
    	/// Width of window
    	/// </summary>
        public const int WINDOW_WIDTH = 800; //width of screen
        /// <summary>
        /// Height of window
        /// </summary>
        public const int WINDOW_HEIGHT = 600; //height of screen
        /// <summary>
        /// Whether the game is in debug mode or not
        /// </summary>
        public static bool DebugMode;
        /// <summary>
        /// Whether the mouse should be visible or not
        /// </summary>
        public static bool MouseVisible;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new game
        /// </summary>
        public RPG()
        {
            GraphicsManager = graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            ContentManager = Content;
        }
        #endregion

        #region Default Methods    
        /// <summary>
        /// Loads starting values
        /// </summary>
        protected override void Initialize()
        {
        	graphics.PreferredBackBufferHeight = WINDOW_HEIGHT; //height of screen
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH; //width of screen
            graphics.ApplyChanges();
            base.Initialize();
            (Form.FromHandle(Window.Handle) as Form).FormClosing += CloseProgram;
        }

        /// <summary>
        /// Loads all content necessary for startup
        /// </summary>
        protected override void LoadContent()
        {
            RNG = new Random();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loadNewGame = true;
        }

        /// <summary>
        /// Unloads specific content at game close
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Updates game; checks for collisions; etc
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
        	if(loadNewGame)
            {
            	player = new Player(null, new Vector2(514, 218), "Sean", "defaultPlayer");
            	currentArea = new Area("house_inside");
            	loadNewGame = false;
            }
        	if(IsActive)
        	{
        		if(MediaPlayer.State == MediaState.Paused) MediaPlayer.Resume();
        		this.IsMouseVisible = MouseVisible;
        		currentArea.Update(gameTime);
            	base.Update(gameTime);
            	this.Window.Title = "Player coordinates: " + currentArea.User.Position;
        	}
        	else
        	{
        		MediaPlayer.Pause();
        	}
        }

        /// <summary>
        /// Draws the game
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
        	GraphicsDevice.Clear(Color.Black);
        	spriteBatch.Begin(); //begin drawing
            currentArea.Draw(spriteBatch);
            spriteBatch.End(); //end drawing
            base.Draw(gameTime);
        }
        #endregion
        /// <summary>
        /// Closes the program
        /// </summary>
        /// <param name="sender">Object command sent from</param>
        /// <param name="e">Arguments for the form closing</param>
        public static void CloseProgram(object sender, FormClosingEventArgs e)
        {

        	if(MessageBox.Show("Are you sure you want to exit to Windows?",
        	                   "Close game", MessageBoxButtons.YesNo) == DialogResult.Yes)
        	{
        		Process.GetProcesses().First(pro => pro.ProcessName == Process.GetCurrentProcess().ProcessName).Kill();
        	}
            else e.Cancel = true;
        }
        
        /// <summary>
        /// Tells game to restart/load a new game
        /// </summary>
        public static void LoadNewGame()
        {
        	loadNewGame = true;
        }
    }
}
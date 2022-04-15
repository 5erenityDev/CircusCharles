///Course:      CSC 316
///Project:     Assignment 3 - 3D Circus Charlie
///Date:        4/1/22
///Author:      Sean Blankenship
///Description: This is my version of a 3D Circus Charlie simply titled Circus Charles
///             You play as a frog named Charles as he hosts his own circus perfomance
///             Dodge the balls coming at you by either dodging around, going under, or jumping over.
///             WASD or Arrow Keys to move left and right 
///             Space, W, or up key to jump

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using System;
using System.Collections.Generic;

namespace CircusCharles
{
    public class Game1 : Game
    {
        ///////////////
        ///VARIABLES///
        ///////////////
        // Essential
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        // Songs
        private Song gameTheme;

        // 3D Assets
        private Model playerModel, groundModel, ballModel, shadowModel;

        // 2D Assets
        private SpriteFont gameFont;
        private Texture2D cursor, title;
        private Texture2D startButton, creditsButton, exitButton, titleButton, againButton, backButton;
        private Texture2D startHover, creditsHover, exitHover, titleHover, againHover, backHover;


        // Player
        private Player player;
        private List<SoundEffect> playerSounds = new List<SoundEffect>();

        // Ball
        private Ball ball;
        private Random rand;
        private List<Ball> ballList;
        private List<SoundEffect> ballSounds = new List<SoundEffect>();
        private int minBallSpeed, maxBallSpeed;
        private int minBallHeight, maxBallHeight;
        private float spin;

        // Ground
        private Vector3 groundPos;


        // Cursor
        private Vector2 cursorPos;
        private float cursorRadius;

        // Mouse
        private MouseState mState;
        private Vector2 mPos;
        private bool mReleasedLeft;


        // Camera
        private Vector3 camTarget;
        private Vector3 camPosition;

        // Matrices
        private Matrix worldPlayer, worldBall, worldGround, worldShadow;
        private Matrix view, proj;


        //Buttons
        private Vector2 startButtonPos, creditsButtonPos, exitButtonPos, titleButtonPos, againButtonPos, backButtonPos;
        private Vector2 mouseStartDist, mouseCreditsDist, mouseExitDist, mouseTitleDist, mouseAgainDist, mouseBackDist;
        const int buttonWidth = 200;
        const int buttonHeight = 100;

        // Game
        private float fov;
        private int currentScreen, currentMenu;
        private bool gameOver, newTopScore;

        // Score
        private int score;
        private int[] topScores;

        // Timer
        private float maxTime; //how much time between obstacle spawns
        private float timer;



        ////////////////////
        ///MAIN FUNCTIONS///
        ////////////////////
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Set camera position and target
            camPosition = new Vector3(75f, 50f, -60f); // For 3D
            //camPosition = new Vector3(0f, 0f, -100f); // For 2D
            camTarget = Vector3.Zero;

            // Set cursor data
            cursorPos = new Vector2(10, 10);
            cursorRadius = 22f;

            // Set mouse data
            mPos = new Vector2(0, 0);
            mReleasedLeft = true;

            // Set game data
            currentScreen = 1;
            currentMenu = 1;
            fov = 45f;
            score = 0;
            topScores = new int[] { 0, 0, 0, 0, 0 };
            maxTime = 2f;
            timer = maxTime;
            rand = new Random();

            // Set ball data
            minBallSpeed = 40;
            maxBallSpeed = 80;
            minBallHeight = 10;
            maxBallHeight = 50;
            spin = 0;

            //Set Button data
            startButtonPos = new Vector2(240, 250);
            creditsButtonPos = new Vector2(130, 370);
            exitButtonPos = new Vector2(350, 370);
            titleButtonPos = new Vector2(110, 420);
            againButtonPos = new Vector2(330, 420);
            backButtonPos = new Vector2(110, 420);

            // Camera matrices
            view = Matrix.CreateLookAt(camPosition,
                camTarget,
                Vector3.Up);

            proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov),
                                                        _graphics.GraphicsDevice.Viewport.AspectRatio,
                                                        1.0f,
                                                        1000f);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load sounds
            gameTheme = Content.Load<Song>("gameTheme");
            MediaPlayer.Play(gameTheme);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;


            playerSounds.Add(Content.Load<SoundEffect>("playerJump"));
            ballSounds.Add(Content.Load<SoundEffect>("ballCollide"));

            // Load fonts
            gameFont = Content.Load<SpriteFont>("gamefont");

            // Load 2D textures
            title = Content.Load<Texture2D>("charlesTitle");

            cursor = Content.Load<Texture2D>("ballCursor");

            startButton = Content.Load<Texture2D>("start");
            startHover = Content.Load<Texture2D>("startHover");
            creditsButton = Content.Load<Texture2D>("credits");
            creditsHover = Content.Load<Texture2D>("creditsHover");
            exitButton = Content.Load<Texture2D>("exit");
            exitHover = Content.Load<Texture2D>("exitHover");
            titleButton = Content.Load<Texture2D>("title");
            titleHover = Content.Load<Texture2D>("titleHover");
            againButton = Content.Load<Texture2D>("again");
            againHover = Content.Load<Texture2D>("againHover");
            backButton = Content.Load<Texture2D>("back");
            backHover = Content.Load<Texture2D>("backHover");

            // Load 3D models
            playerModel = Content.Load<Model>("charles");
            groundModel = Content.Load<Model>("ground");
            ballModel = Content.Load<Model>("ball");
            shadowModel = Content.Load<Model>("shadow");
        }

        protected override void Update(GameTime gameTime)
        {
            // Update mouse state
            mState = Mouse.GetState();
            // Update mouse position
            mPos.X = mState.X;
            mPos.Y = mState.Y;

            // Update cursor position
            cursorPos.X = mPos.X - cursorRadius;
            cursorPos.Y = mPos.Y - cursorRadius;

            switch (currentScreen)
            {
                case 1:
                    StartScreenUpdate();
                    break;
                case 2:
                    MainGameUpdate(gameTime);
                    break;
                case 3:
                    EndScreenUpdate();
                    break;
            }

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();

            // Graphics Device Settings
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.Black);



            switch (currentScreen)
            {
                case 1:
                    StartScreenDraw();
                    break;
                case 2:
                    MainGameDraw();
                    break;
                case 3:
                    EndScreenDraw();
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }



        ////////////////////
        ///GAME FUNCTIONS///
        ////////////////////
        private void MainGameUpdate(GameTime gameTime)
        {
            // Calculates obstacle timer
            if (timer > 0)
            {
                timer = timer - (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // If the timer hits zero...
            if (timer <= 0)
            {
                timer = maxTime;
                ball = new Ball(new Vector3(-100f, 0f, (float)rand.Next(-20, 20)),
                                (float)rand.Next(minBallHeight, maxBallHeight), //ball height
                                (float)rand.Next(minBallSpeed, maxBallSpeed),   //ball speed
                                ballModel.Meshes[0].BoundingSphere.Radius,
                                ballSounds);
                ballList.Add(ball);
                if (maxTime > 0.5f)
                {
                    maxBallSpeed += 4;
                    maxTime -= 0.05f;
                }

            }


            // Update player
            player.Update(gameTime);

            // Update obstacles
            foreach (Ball b in ballList)
            {
                b.Update(gameTime);
                if (b.checkCollision(player))
                {
                    gameOver = true;
                    currentScreen = 3;
                }
                if (b.Pos.X >= 70f)
                {
                    score += 100;
                    ballList.Remove(b);
                    break;
                }

            }
        }
        private void MainGameDraw()
        {


            // Render ground
            worldGround = Matrix.CreateScale(new Vector3(1.1f, 1f, 5f)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(90f)) *
                        Matrix.CreateTranslation(groundPos);
            groundModel.Draw(worldGround, view, proj);


            // Render player
            worldPlayer = Matrix.CreateScale(0.1f) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(90f)) *
                        Matrix.CreateTranslation(player.Pos);
            playerModel.Draw(worldPlayer, view, proj);

            // Render obstacles
            foreach (Ball b in ballList)
            {
                worldBall = Matrix.CreateScale(0.05f) *
                            Matrix.CreateRotationZ(MathHelper.ToRadians(b.Rot)) *
                            Matrix.CreateTranslation(b.Pos);
                ballModel.Draw(worldBall, view, proj);
                worldShadow = Matrix.CreateScale(0.05f) *
                              Matrix.CreateTranslation(new Vector3(b.Pos.X, 0.3f, b.Pos.Z));
                shadowModel.Draw(worldShadow, view, proj);
            }

            // Render score
            _spriteBatch.DrawString(gameFont, "Score: " + score.ToString(),
                        new Vector2(10, 10), Color.White);
        }



        /////////////////////
        ///START FUNCTIONS///
        /////////////////////
        ///UPDATE
        private void StartScreenUpdate()
        {
            switch (currentMenu)
            {
                case 1:
                    MainMenuUpdate();
                    break;
                case 2:
                    CreditsMenuUpdate();
                    break;
            }
        }

        private void MainMenuUpdate()
        {
            // Find mouse distance from buttons
            mouseStartDist = new Vector2(Math.Abs(startButtonPos.X - mState.X), Math.Abs(startButtonPos.Y - mState.Y));
            mouseCreditsDist = new Vector2(Math.Abs(creditsButtonPos.X - mState.X), Math.Abs(creditsButtonPos.Y - mState.Y));
            mouseExitDist = new Vector2(Math.Abs(exitButtonPos.X - mState.X), Math.Abs(exitButtonPos.Y - mState.Y));

            // Check for button press
            if (mState.LeftButton == ButtonState.Pressed
            && mReleasedLeft == true)
            {
                // Start
                if (mouseStartDist.Y < buttonHeight / 2 && mouseStartDist.X < buttonWidth / 2)
                {
                    score = 0;
                    timer = maxTime;
                    player = new Player(playerSounds);
                    ballList = new List<Ball>();
                    groundPos = new Vector3(50f, 0f, 0f);
                    minBallSpeed = 40;
                    maxBallSpeed = 80;
                    maxTime = 2f;
                    currentScreen = 2;
                    mReleasedLeft = false;
                }
                // Credits
                else if (mouseCreditsDist.Y < buttonHeight / 2 && mouseCreditsDist.X < buttonWidth / 2)
                {
                    currentMenu = 2;
                    mReleasedLeft = false;
                }
                // Exit
                else if (mouseExitDist.Y < buttonHeight / 2 && mouseExitDist.X < buttonWidth / 2)
                {
                    if (mState.LeftButton == ButtonState.Pressed
                    && mReleasedLeft == true)
                        Exit();
                    if (mState.LeftButton == ButtonState.Released)
                        mReleasedLeft = true;
                }
            }
            if (mState.LeftButton == ButtonState.Released)
                mReleasedLeft = true;
        }
        private void CreditsMenuUpdate()
        {
            mouseBackDist = new Vector2(Math.Abs(backButtonPos.X - mState.X), Math.Abs(backButtonPos.Y - mState.Y));

            // Check for button press
            if (mState.LeftButton == ButtonState.Pressed
            && mReleasedLeft == true)
            {
                // Back
                if (mouseBackDist.Y < buttonHeight / 2 && mouseBackDist.X < buttonWidth / 2)
                {
                    currentMenu = 1;
                    mReleasedLeft = false;
                }
            }
            if (mState.LeftButton == ButtonState.Released)
                mReleasedLeft = true;
        }


        ///DRAW
        private void StartScreenDraw()
        {
            switch (currentMenu)
            {
                case 1:
                    MainMenuDraw();
                    break;
                case 2:
                    CreditsMenuDraw();
                    break;
            }
            // Draw cursor
            _spriteBatch.Draw(cursor, new Vector2(mState.X - cursorRadius, mState.Y - cursorRadius), Color.White);
        }

        private void MainMenuDraw()
        {
            worldPlayer = Matrix.CreateScale(0.6f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(spin++)) *
                Matrix.CreateRotationX(MathHelper.ToRadians(30)) *
                Matrix.CreateTranslation(new Vector3(-10f, 5f, -40f));

            playerModel.Draw(worldPlayer, view, proj);
            _spriteBatch.Draw(title, new Vector2(40, 10), Color.White);
            // Button Hover Check
            // Start
            if (mouseStartDist.Y < buttonHeight / 2 && mouseStartDist.X < buttonWidth / 2)
                _spriteBatch.Draw(startHover, new Vector2(startButtonPos.X - buttonWidth / 2, startButtonPos.Y - buttonHeight / 2), Color.White);
            else
                _spriteBatch.Draw(startButton, new Vector2(startButtonPos.X - buttonWidth / 2, startButtonPos.Y - buttonHeight / 2), Color.White);

            // Credits
            if (mouseCreditsDist.Y < buttonHeight / 2 && mouseCreditsDist.X < buttonWidth / 2)
                _spriteBatch.Draw(creditsHover, new Vector2(creditsButtonPos.X - buttonWidth / 2, creditsButtonPos.Y - buttonHeight / 2), Color.White);
            else
                _spriteBatch.Draw(creditsButton, new Vector2(creditsButtonPos.X - buttonWidth / 2, creditsButtonPos.Y - buttonHeight / 2), Color.White);

            // Exit
            if (mouseExitDist.Y < buttonHeight / 2 && mouseExitDist.X < buttonWidth / 2)
                _spriteBatch.Draw(exitHover, new Vector2(exitButtonPos.X - buttonWidth / 2, exitButtonPos.Y - buttonHeight / 2), Color.White);
            else
                _spriteBatch.Draw(exitButton, new Vector2(exitButtonPos.X - buttonWidth / 2, exitButtonPos.Y - buttonHeight / 2), Color.White);
        }
        private void CreditsMenuDraw()
        {
            //Draw scores
            _spriteBatch.DrawString(gameFont, "Created by: Sean Blankenship",
                                    new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(gameFont, "Music Used:\n" +
                                    "Clowning Around by Shane Ivers\n     https://www.silvermansound.com",
                                    new Vector2(10, 100), Color.White);
            _spriteBatch.DrawString(gameFont, "Based On Konami's Circus Charlie (1986)",
                        new Vector2(10, 290), Color.White);
            // Button Hover Check
            // Back
            if (mouseBackDist.Y < buttonHeight / 2 && mouseBackDist.X < buttonWidth / 2)
                _spriteBatch.Draw(backHover, new Vector2(backButtonPos.X - buttonWidth / 2, backButtonPos.Y - buttonHeight / 2), Color.White);
            else
                _spriteBatch.Draw(backButton, new Vector2(backButtonPos.X - buttonWidth / 2, backButtonPos.Y - buttonHeight / 2), Color.White);
        }

        ///////////////////
        ///END FUNCTIONS///
        ///////////////////
        private void EndScreenUpdate()
        {
            if (gameOver)
            {
                // Determine high score
                if (score > topScores[0])
                {
                    topScores[4] = topScores[3];
                    topScores[3] = topScores[2];
                    topScores[2] = topScores[1];
                    topScores[1] = topScores[0];
                    topScores[0] = score;
                    newTopScore = true;
                }
                else if (score > topScores[1])
                {
                    topScores[4] = topScores[3];
                    topScores[3] = topScores[2];
                    topScores[2] = topScores[1];
                    topScores[1] = score;
                    newTopScore = true;
                }
                else if (score > topScores[2])
                {
                    topScores[4] = topScores[3];
                    topScores[3] = topScores[2];
                    topScores[2] = score;
                    newTopScore = true;
                }
                else if (score > topScores[3])
                {
                    topScores[4] = topScores[3];
                    topScores[3] = score;
                    newTopScore = true;
                }
                else if (score > topScores[4])
                {
                    topScores[4] = score;
                    newTopScore = true;
                }
                gameOver = false;
            }


            // Find mouse distance from buttons
            mouseTitleDist = new Vector2(Math.Abs(titleButtonPos.X - mState.X), Math.Abs(titleButtonPos.Y - mState.Y));
            mouseAgainDist = new Vector2(Math.Abs(againButtonPos.X - mState.X), Math.Abs(againButtonPos.Y - mState.Y));

            // Check for button press
            if (mState.LeftButton == ButtonState.Pressed
            && mReleasedLeft == true)
            {
                // Title
                if (mouseTitleDist.Y < buttonHeight / 2 && mouseTitleDist.X < buttonWidth / 2)
                {
                    newTopScore = false;

                    currentScreen = 1;
                    mReleasedLeft = false;
                }
                // Again
                else if (mouseAgainDist.Y < buttonHeight / 2 && mouseAgainDist.X < buttonWidth / 2)
                {
                    if (mState.LeftButton == ButtonState.Pressed
                    && mReleasedLeft == true)
                    {
                        score = 0;
                        timer = maxTime;
                        player = new Player(playerSounds);
                        ballList = new List<Ball>();
                        groundPos = new Vector3(50f, 0f, 0f);
                        minBallSpeed = 40;
                        maxBallSpeed = 80;
                        maxTime = 2f;
                        newTopScore = false;

                        currentScreen = 2;
                        mReleasedLeft = false;

                    }
                    if (mState.LeftButton == ButtonState.Released)
                        mReleasedLeft = true;
                }
            }
            if (mState.LeftButton == ButtonState.Released)
                mReleasedLeft = true;
        }

        private void EndScreenDraw()
        {
            // Draw spinning balls
            worldBall = Matrix.CreateScale(0.1f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(spin++)) *
                Matrix.CreateTranslation(new Vector3(-10f, -20f, -40f));
            ballModel.Draw(worldBall, view, proj);
            worldBall = Matrix.CreateScale(0.1f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(spin)) *
                Matrix.CreateTranslation(new Vector3(-10f, 10f, -40f));
            ballModel.Draw(worldBall, view, proj);
            worldBall = Matrix.CreateScale(0.1f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(spin)) *
                Matrix.CreateTranslation(new Vector3(-10f, 40f, -40f));
            ballModel.Draw(worldBall, view, proj);

            // Draw end text
            _spriteBatch.DrawString(gameFont, "GAME OVER", new Vector2(100, 10), Color.White);

            //Draw scores
            _spriteBatch.DrawString(gameFont, "Score: " + score.ToString(),
                                    new Vector2(10, 60), Color.White);
            _spriteBatch.DrawString(gameFont, "Top 5 Scores:\n" +
                                    "1: " + topScores[0].ToString() + "\n" +
                                    "2: " + topScores[1].ToString() + "\n" +
                                    "3: " + topScores[2].ToString() + "\n" +
                                    "4: " + topScores[3].ToString() + "\n" +
                                    "5: " + topScores[4].ToString(),
                                    new Vector2(10, 120), Color.White);
            if (newTopScore)
                _spriteBatch.DrawString(gameFont, "New Top Score!",
                    new Vector2(300, 60), Color.White);
            _spriteBatch.DrawString(gameFont, "Created by:\nSean Blankenship",
                        new Vector2(500, 380), Color.White);

            // Button Hover Check
            // Back to Title
            if (mouseTitleDist.Y < buttonHeight / 2 && mouseTitleDist.X < buttonWidth / 2)
                _spriteBatch.Draw(titleHover, new Vector2(titleButtonPos.X - buttonWidth / 2, titleButtonPos.Y - buttonHeight / 2), Color.White);
            else
                _spriteBatch.Draw(titleButton, new Vector2(titleButtonPos.X - buttonWidth / 2, titleButtonPos.Y - buttonHeight / 2), Color.White);
            // Play Again
            if (mouseAgainDist.Y < buttonHeight / 2 && mouseAgainDist.X < buttonWidth / 2)
                _spriteBatch.Draw(againHover, new Vector2(againButtonPos.X - buttonWidth / 2, againButtonPos.Y - buttonHeight / 2), Color.White);
            else
                _spriteBatch.Draw(againButton, new Vector2(againButtonPos.X - buttonWidth / 2, againButtonPos.Y - buttonHeight / 2), Color.White);

            // Draw cursor
            _spriteBatch.Draw(cursor, new Vector2(mState.X - cursorRadius, mState.Y - cursorRadius), Color.White);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System.Collections.Generic;

namespace CircusCharles
{
    //////////////////
    ///PLAYER CLASS///
    //////////////////
    class Player
    {
        ///////////////
        ///VARIABLES///
        ///////////////
        //moving
        private Vector3 pos;

        //jumping
        private bool jumping = false;
        private bool falling = false;
        private float jumpSpeed = 1f;
        private float maxHeight = 36f;
        private float minHeight = 5f;

        //sound
        private List<SoundEffect> soundEffects;
        private SoundEffectInstance jumpInstance;



        ////////////////////
        ///MAIN FUNCTIONS///
        ////////////////////
        public Player(List<SoundEffect> s)
        {
            pos = new Vector3(50f, minHeight, 0f);
            soundEffects = s;
            jumpInstance = soundEffects[0].CreateInstance();
        }
        public void Update(GameTime gameTime)
        {
            Move(gameTime);
        }



        ////////////////////////
        ///MOVEMENT FUNCTIONS///
        ////////////////////////
        private void Move(GameTime gameTime)
        {
            //move left
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                if (pos.Z <= 20)
                    pos.Z++;
            //move right
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                if (pos.Z >= -20)
                    pos.Z--;
            //initiate jump
            if (!jumping && Keyboard.GetState().IsKeyDown(Keys.Space) || !jumping && Keyboard.GetState().IsKeyDown(Keys.Up) || !jumping && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                jumpInstance.Play();
                jumping = true;
            }


            //calculate jump
            if (jumping)
                Jump(gameTime);
        }

        private void Jump(GameTime gameTime)
        {
            //determine if player is still going up, if so, increase jump height
            if (Pos.Y <= maxHeight && !falling)
            {
                pos.Y += jumpSpeed;
            }
            //determine if player is now going down
            else if (Pos.Y > maxHeight && !falling)
            {
                falling = true;
            }
            //decrease jump height
            else if (falling && pos.Y > minHeight)
                pos.Y -= jumpSpeed;
            //once on the ground, stop jumping
            else if (pos.Y <= minHeight)
            {
                pos.Y = minHeight;
                falling = false;
                jumping = false;
            }
        }



        //////////////////////
        ///HELPER FUNCTIONS///
        //////////////////////
        public Vector3 Pos
        {
            get { return pos; }
            set { pos = value; }
        }
    }
}

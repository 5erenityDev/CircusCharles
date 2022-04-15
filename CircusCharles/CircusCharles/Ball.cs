using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CircusCharles
{
    ////////////////
    ///BALL CLASS///
    ////////////////
    class Ball
    {
        ///////////////
        ///VARIABLES///
        ///////////////
        //movement
        private Vector3 pos;
        private float speed, acc, radius, rot = 0f;
        private bool rising = false;

        //sound
        private List<SoundEffect> soundEffects;
        private SoundEffectInstance collideInstance;



        ////////////////////
        ///MAIN FUNCTIONS///
        ////////////////////
        public Ball(Vector3 p, float mh, float s, float r, List<SoundEffect> bs)
        {
            pos = new Vector3(p.X, mh, p.Z);
            speed = s;
            acc = -1.6f;
            radius = r;
            soundEffects = bs;
            collideInstance = soundEffects[0].CreateInstance();
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
            //ball movement
            float gravity = 1.1f;

            //determine if the ball is rising or falling.
            //it is falling(-) when its velocity hits zero (or somewhere close to).
            //it is rising(+) when it bounces off the ground at Y = 0.

            if (Math.Abs(acc) <= 1.5f)
            {
                if (rising)
                {
                    acc *= -1;
                    rising = false;
                }

            }
            if (pos.Y <= radius + 5f)
            {
                if (!rising)
                {
                    pos.Y = radius + 5f;
                    acc *= -1;
                    rising = true;
                }
            }

            //rising: gravity works against the ball
            //falling: gravity works with the ball
            if (rising)
                acc /= gravity;
            if (!rising)
                acc *= gravity;

            //in reality, the ball would lose speed and height as it bounces,
            //but for the sake of good gameplay, I will not emulate that behavior.
            //so the acceleration has a very slight effect on the overall X velocity.
            pos = new Vector3(
                pos.X += (speed + (Math.Abs(acc) * 0.06f)) * (float)gameTime.ElapsedGameTime.TotalSeconds,
                pos.Y += acc * (float)gameTime.ElapsedGameTime.TotalSeconds,
                pos.Z);
            //ball rotate
            rot -= (speed * 3f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }



        /////////////////////////
        ///COLLISION FUNCTIONS///
        /////////////////////////
        public bool checkCollision(Player other)
        {

            if (this.boundingSphere.Intersects(new BoundingBox(
                new Vector3(other.Pos.X - 5, other.Pos.Y - 5, other.Pos.Z - 5),
                new Vector3(other.Pos.X + 5, other.Pos.Y + 5, other.Pos.Z + 5))))
            {
                collideInstance.Play();
                return true;
            }
            return false;
        }



        //////////////////////
        ///HELPER FUNCTIONS///
        //////////////////////
        public Vector3 Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public float Acc
        {
            get { return acc; }
            set { acc = value; }
        }

        public float Rot
        {
            get { return rot; }
            set { rot = value; }
        }

        private BoundingSphere boundingSphere
        {
            get { return new BoundingSphere(pos, radius); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public abstract class Abstract_Model : IUpdatable {
        protected double _x = 0;
        protected double _y = 0;
        protected double _z = 0;
        protected double _rX = 0;
        protected double _rY = 0;
        protected double _rZ = 0;

        public string type { get; set; }
        public Guid guid { get; set; }
        public double x { get { return _x; } }
        public double y { get { return _y; } }
        public double z { get { return _z; } }
        public double rotationX { get { return _rX; } }
        public double rotationY { get { return _rY; } }
        public double rotationZ { get { return _rZ; } }

        public bool needsUpdate = true;



        public virtual void Move(double x, double y, double z) {
            this._x = x;
            this._y = y;
            this._z = z;

            needsUpdate = true;
        }
        public virtual void MoveTo(double xd,double yd, double zd)
        {
            //Check if you need to move on an axis
            // Check if you need to move less than a 'tick'
            // if true, move the  last remaining bit
            // if false, move the tick valye
            // Repeat until the move is done
        
            double xdif = xd - x;
            double ydif = yd - y;
            double zdif = zd - z;
            bool destinationreached = false;
            // 3 times, for each axis
            while (!destinationreached)
            {

            
                if (!needsUpdate)
                {



                    // If not 0, i need to move on the X axis
                    if (xdif != 0)
                    {
                        // If less than 5 , 
                        if (xdif < 5)
                        {
                            this.Move(xdif, 0, 0);
                            destinationreached = true;

                        }
                        else
                        {
                            this.Move(5, 0, 0);
                            xdif = xdif - 5;
                        }
                    }

                    if (xdif == 0)
                    {
                        destinationreached = true;
                    }


                }
            }
        }

        public virtual void Rotate(double rotationX, double rotationY, double rotationZ) {
            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;

            needsUpdate = true;
        }
       
        public virtual bool Update(int tick)
        {
            if(needsUpdate) {
                needsUpdate = false;
                return true;
            }
            return false;
        }
    }
}
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
        // Using nodes instead of doubles, delete these later
        protected double target_x = 0;
        protected double target_y = 0;
        protected double target_z = 0;

        protected List<Node> route;
        protected Node TargetNode;

        protected List<Node> PathList;
        public bool destinationreached = true;
        public bool isMoving = false;
        public double speed = 0.11;

        public virtual void Move(double x, double y, double z) {
            this._x = x;
            this._y = y;
            this._z = z;

            needsUpdate = true;
        }
        public virtual void SetDestination(double x,double y ,double z)
        {
            target_x = x;
            target_y = y;
            target_z =  z;
            destinationreached = false;
        }
        /// <summary>
        /// Moves the model to the given coordinates
        /// </summary>
        /// <param name="xd"></param>
        /// <param name="yd"></param>
        /// <param name="zd"></param>
        public virtual void MoveTo(double xd, double yd, double zd)
        {
         if (!isMoving)
            {
                // If  this is the first time moving, set the target
                target_x = xd;
                target_y = yd;
                target_z = zd;
                isMoving = true;
                needsUpdate = true;
            }
            if (isMoving)
            {
                if (xd == this.x && yd == this.y && zd ==z)
                {
                    isMoving = false;
                    return;
                }
                // If already moving, move the model on the axis by the value of speed. ALso checks for positve and negative values
                if (x != target_x )
                {
                    if (target_x >x)
                    {
                        _x = x + speed;

                    }
                    else
                    {
                        _x = x - speed;
                    }
                    if (Math.Abs(target_x - x) < speed)
                    {
                        _x = target_x;
                        Console.WriteLine("Less than " + speed + " x diff");

                    }
                    //  _x = x +speed;
                    // needsUpdate = true;

                }
                if (y != target_y)
                {
                    if (target_y > y)
                    {
                        _y = y + speed;
                    }
                    else
                    {
                        _y = y - speed;
                    }
     
                    if (Math.Abs(target_y - y) < speed)
                    {

                        _y = target_y;
                        Console.WriteLine("Less than "+speed+" y diff");
                    }
                    // _y = y + speed;
                    //  needsUpdate = true;

                }
                if (z != target_z)
                {
                    if (target_z > x)
                    {
                        _z = z + speed;
                    }
                    else
                    {
                        _z = z - speed;
                    }
                    //  _z = z + speed;
                    if (Math.Abs(target_z - z) < speed)
                    {

                        _z = target_z;
                        Console.WriteLine("Less than " + speed + " z diff");
                    }

                }
                needsUpdate = true;
                return;
            }
            double x_dif = xd - x;
            double y_dif = yd - y;
            double z_dif = zd - z;

            // Move one step, check if i need to move again

            //Check if you need to move on an axis
            // Check if you need to move less than a 'tick'
            // if true, move the  last remaining bit
            // if false, move the tick valye
            // Repeat until the move is done

            if (destinationreached)
            {
                // No need to move this update
                return;
            }


            // 3 times, for each axis

        }
        // Calls Moveto using a node instead of coordinates
        public virtual void MoveTo(Node node)
        {
            MoveTo(node.x, node.y, node.z);
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
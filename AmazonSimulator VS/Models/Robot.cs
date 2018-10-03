using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : Abstract_Model
    {
        bool atPickupPoint = true;
        Rek carriedRek;
        private World w;
        
        public void PickupRek()
        {
           
            if (atPickupPoint)
            {
                foreach (var item in w.worldObjects)
                {
                   
                    if (item is Rek)
                    {
                         Rek q = (Rek)item;
                        
                        if (q.readyforpickup == true)
                        {
                            carriedRek = q;
                            carriedRek.Move(this.x,this.y,this.z);
                        }
                    }
                }
            }
        }

       
    
    public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ,World w)
        {
            this.w = w;
            this.type = "robot";
            this.guid = Guid.NewGuid();

            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;
        }
        public override bool Update(int tick)
        {
            this.MoveTo(30, 0, 30);
            if(carriedRek != null)
            {
                carriedRek.Move(this.x, this.y, this.z);
            }
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;
            }
            return false;
        }
    }
}
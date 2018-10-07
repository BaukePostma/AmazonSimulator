using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace Models
{
    public enum TreiState
    {
        TRAIN_INCOMMING,
        AT_LOADING_DOCK,
        TRAIN_DEPARTING
    }
    public class Trein : Abstract_Model
    {
        World _world;
        TreiState _state;

        public Rek CarriedRek;
        public Trein(double x, double y, double z, double rotationX, double rotationY, double rotationZ, World w)
        {
            this.type = "trein";
            this.guid = Guid.NewGuid();
            this._world = w;
            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;

            this.CarriedRek = w.CreateRek(_x,_y,_z);

            _state = TreiState.TRAIN_INCOMMING;

        }



        void AtLoadingDock()
        {
            
            this._state = TreiState.AT_LOADING_DOCK;
            

            this._world.TrainArrived(this);
            CarriedRek = null;

        }


        // Word aangeroepen door een robot
        void Load(Rek cargo)
        {
            this.CarriedRek = cargo;
        }

        void Depart()
        {
            this._state = TreiState.TRAIN_DEPARTING;
            this.isMoving = false;
        }

        public override bool Update(int tick)
        {
            switch (this._state)
            {
                case TreiState.TRAIN_INCOMMING:
                    this.MoveTo(15, 0, -5);
                    if (this._x == 15 && this._z == -5)
                    {
                        AtLoadingDock();
                    }
                    break;
                case TreiState.AT_LOADING_DOCK:
                    if (CarriedRek != null) // Als we een rek hebben gekregen sinds de laatste Update()
                    {
                        Depart();
                    }
                    break;
                case TreiState.TRAIN_DEPARTING:
                    this.MoveTo(40, 0, -5);
                    break;
            }

            if (CarriedRek != null)
            {
                CarriedRek.Move(this.x, this.y, this.z);
            }
            // CreateRek(15,0,-5);
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;

            }
            return false;
        }
    }
}
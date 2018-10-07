 using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace Models
{
    public enum TreinState
    {
        TRAIN_INCOMMING,
        AT_LOADING_DOCK,
        TRAIN_DEPARTING
    }
    public class Trein : Abstract_Model
    {
        World _world;
        TreinState _state;

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

            this.CarriedRek = this._world.CreateRek(this.x, this.y, this.z);
            this.CarriedRek.readyforpickup = false;
            _world.worldObjects.Add(this.CarriedRek);
            _state = TreinState.TRAIN_INCOMMING;

        }



        int AtLoadingDock()
        {

            Rek cargo = this.Unload();
            this.CarriedRek = null;
            this._world.TrainArrived(this, cargo);

            this._state = TreinState.AT_LOADING_DOCK;

            return 0;
        }

        int Departed()
        {
            // Als trein helemaal weg gereden is
            this._world.TrainDeparted(this);
            return 0;
        }

        Rek Unload()
        {
            _world.worldObjects.Add(this.CarriedRek);
            this.CarriedRek.readyforpickup = true;
            _world.CommandPickup(this.CarriedRek,true);
            return this.CarriedRek;
        }

        // Word aangeroepen door een robot
        public void Load(Rek cargo)
        {
            this.CarriedRek = cargo;
        }

        void Depart()
        {
            this._state = TreinState.TRAIN_DEPARTING;
        }

        public override bool Update(int tick)
        {

            switch (this._state)
            {
                case TreinState.TRAIN_INCOMMING:
                    this.MoveTo(15, 0, -5, this.AtLoadingDock);
                    break;
                case TreinState.AT_LOADING_DOCK:
                    if (CarriedRek != null) // Als we een rek hebben gekregen sinds de laatste Update()
                    {
                        Depart();
                    }
                    break;
                case TreinState.TRAIN_DEPARTING:
                    this.MoveTo(40, 0, -5, this.Departed);
                    break;
            }

            if (CarriedRek != null)
            {
                CarriedRek.Move(this.x-2, this.y+2, this.z);
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
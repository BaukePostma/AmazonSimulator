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
        /// <summary>
        /// Amount of barrels this train brings, to store in areas
        /// </summary>
        public int Barrels_to_Store;
        /// <summary>
        /// AMount of barrels this train needs to load in order to  depart
        /// </summary>
        public int Barrels_to_Load;

       // public Rek CarriedRek;
        public List<Rek> Cargo = new List<Rek>();

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


            Randomizeload();
            
            _state = TreiState.TRAIN_INCOMMING;

        }
        public void FillCargo(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Rek q = _world.CreateRek(_x + i, _y, _z);
                Cargo.Add(q);
            }
        }
        /// <summary>
        /// Randomizes the amount of barrels this train will deliver and ask
        /// </summary>
        public void Randomizeload()
        {
            Random r = new Random();

            Barrels_to_Load = r.Next(1, 4);
            Barrels_to_Store = r.Next(1, 4);
            FillCargo(Barrels_to_Store);

        }
        /// <summary>
        /// Gets called when the train reaches the loading dock
        /// </summary>
        void AtLoadingDock()
        {
            isMoving = false;
            this._state = TreiState.AT_LOADING_DOCK;       
            this._world.TrainArrived(this);
            //Cargo = null;
            Cargo.Clear();

        }
        void resetTrain()
        {
            Move(-20, 0, -5);
            this._state = TreiState.TRAIN_INCOMMING;
            Cargo.Clear();
            Randomizeload();
            Console.WriteLine();
        }

        /// <summary>
        /// word aangeropen door een robot
        /// </summary>
        /// <param name="cargo"></param>
        void Load(Rek cargo)
        {
            // this.CarriedRek = cargo;
            Cargo.Add(cargo);
        }
        /// <summary>
        /// Word aangeropen wanneer de trein moet vertrekken
        /// </summary>
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
                    if (this._x >= 15 && this._z >= -5)
                    {
                        AtLoadingDock();
                    }
                    break;
                case TreiState.AT_LOADING_DOCK:
                    if (Cargo.Count() == Barrels_to_Load) // Als we een rek hebben gekregen sinds de laatste Update()
                    {
                        Depart();
                    }
                    break;
                case TreiState.TRAIN_DEPARTING:
                    this.MoveTo(60, 0, -5);
                    if (this._x == 60 && this._z == -5)
                    {
                        resetTrain();

                    }
                    break;
            }

            if (isMoving == true)
            {
                foreach (var item in Cargo)
                {
                    item.Move(this.x - 3, this.y + 2, this.z);
                }
              
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
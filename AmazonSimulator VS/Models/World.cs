using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;

namespace Models
{
    public class World : IObservable<Command>, IUpdatable
    {
        public List<Abstract_Model> worldObjects = new List<Abstract_Model>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        Robot r;
        Truck t;
        Dijkstra d;
        public List<Node> NodeList = new List<Node>();
        public List<Storage> StorageSpots = new List<Storage>();
      
        public void addNode(char node, double x, double y, double z)
        {
            Node n = new Node(node, x, y, z);
            NodeList.Add(n);
        }
        public World()
        {
           
            // Create the graph, and create the nodes the robot can move to
            d = new Dijkstra();

            addNode('A', 0, 0, 0);
            addNode('B', 15, 0, 0);
            addNode('C', 30, 0, 0);
            addNode('D', 0, 0, 30);
            addNode('E', 30, 0, 30);

            addNode('F', 15, 0, 5);
            addNode('G', 12, 0, 5);
            addNode('H', 15, 0, 15);
            addNode('I', 12, 0, 15);
            addNode('J', 21, 0, 15);
            addNode('K', 21, 0, 12);
            addNode('L', 21, 0, 25);
            addNode('M', 12, 0, 25);
            addNode('N', 10, 0, 30);
            addNode('O', 30, 0, 15);

            Storage storage1 = new Storage(NodeList[6], 25, 5, 5, 0, 5, this);
            Storage storage2 = new Storage(NodeList[8], 25, 5, 5, 0, 12.5, this);
            Storage storage3 = new Storage(NodeList[10], 15, 5, 20, 0, 5, this);
            Storage storage4 = new Storage(NodeList[12], 25, 5, 5, 0, 22.5, this);
            StorageSpots.Add(storage1);
            StorageSpots.Add(storage2);
            StorageSpots.Add(storage3);
            StorageSpots.Add(storage4);

            // g.shortest_path('A', 'H').ForEach(x => Console.WriteLine(x));
            t = SpawnTruck(-20,0,0);
           // List<char> paths = d.shortest_path('A','F');
            Rek q = CreateRek(-100,0,0);
            r = CreateRobot(15, 0, 0);

            CommandPickup();
           // MoveModel(r, 50, 0, 0);
        }
        //
        /// <summary>
        /// Tell a robot to pick up an item
        /// </summary>
            public void CommandPickup()
        {
            char start = 'B';
            char stop = ' ';

            for (int i = 0; i < StorageSpots.Count; i++)
            {
                if (!StorageSpots[i].IsFull())
                {
                    stop = StorageSpots[i].DropoffNode.name;
                }
            }
            // Tell a robot to come pick up an item
            r.idle = false;
            r.SetRoute(GenerateRoute(start, stop), stop);
            r.PickupRek();
        }
        /// <summary>
        /// Returns a route to a positon , and back again
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns> A list to and from the target</returns>
        public List<char> GenerateRoute(char start , char end)
        {
            List<char> Route = d.shortest_path(start,end);
            List<char> Terugweg = d.shortest_path(end,start);
            Route.Reverse();
            Terugweg.Reverse();
            Route.AddRange(Terugweg);
            return Route;
        }
        private Truck SpawnTruck(double x, double y, double z)
        {
            Truck t = new Truck(x, y, z, 0, 0, 0);
            worldObjects.Add(t);
            return t;
        }
        private Rek CreateRek(double x, double y, double z)
        {
            Rek rek = new Rek (x, y, z, 0, 0, 0);
            worldObjects.Add(rek);
            return rek;
        }
        private Robot CreateRobot(double x, double y, double z)
        {
            Robot constructorrobot = new Robot(x, y, z, 0, 0, 0,this);
            worldObjects.Add(constructorrobot);
            return constructorrobot;
        }
        //public void MoveModel(Abstract_Model model,double x, double y , double z)
        //{
        //    //Check if you need to move on an axis
        //    // Check if you need to move less than a 'tick'
        //    // if true, move the  last remaining bit
        //    // if false, move the tick valye
        //    // Repeat until the move is done

        //    double xdif = x - model.x;
        //    double ydif = y - model.y;
        //    double zdif = z - model.z;
        //    bool destinationreached = false;
        //    // 3 times, for each axis
        //    while (!destinationreached)
        //    {


        //        if (model.needsUpdate)
        //        {



        //            // If not 0, i need to move on the X axis
        //            if (xdif != 0)
        //            {
        //                // If less than 5 , 
        //                if (xdif < 5)
        //                {
        //                    model.Move(xdif, 0, 0);
        //                    destinationreached = true;

        //                }
        //                else
        //                {
        //                    model.Move(5, 0, 0);
        //                    xdif = xdif - 5;
        //                }
        //            }

        //            if (xdif == 0)
        //            {
        //                destinationreached = true;
        //            }


        //        }
        //    }
        //}

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            foreach (Abstract_Model m3d in worldObjects)
            {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for (int i = 0; i < worldObjects.Count; i++)
            {
                Abstract_Model u = worldObjects[i];

                if (u is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)u).Update(tick);
                    if (needsCommand)
                    {
                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }
            return true;
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
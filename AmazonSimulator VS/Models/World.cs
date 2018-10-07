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
        private List<Robot> robotlist = new List<Robot>();

        Robot r;
        Robot walle;
        Robot irongiant;

    
        public Trein t;
        public Dijkstra d;
        public List<Node> NodeList = new List<Node>();
        public List<Storage> StorageSpots = new List<Storage>();
        public List<Rek> Perron_Cargo = new List<Rek>();
           
       
        public World()
        {
           
            // Create the graph, and create the nodes the robot can move to
            d = new Dijkstra();
            InitialNodes();
           
            // Create the four storage area's 
            Storage storage1 = new Storage(NodeList[6], 10, 5, 3, 0, 5, this);
            Storage storage2 = new Storage(NodeList[8], 10, 5, 3, 0, 12.5, this);
            Storage storage3 = new Storage(NodeList[10], 35, 5, 20, 0, 5, this);
            Storage storage4 = new Storage(NodeList[12], 10, 5, 3, 0, 22.5, this);
            StorageSpots.Add(storage1);
            StorageSpots.Add(storage2);
            StorageSpots.Add(storage3);
            StorageSpots.Add(storage4);

            //Create the train
            t = SpawnTrein(-20,0,0);
            t.Rotate(0, 89.55, 0);
            t.speed = 0.6;

            // Initialze 3 robots. 
            r = CreateRobot(12, 0, 0);
            walle = CreateRobot(15, 0, 0);
            irongiant = CreateRobot(18, 0, 0);
            r.speed = 0.4;
            walle.speed = 0.3;
            irongiant.speed = 0.2;
            robotlist.Add(r);
            robotlist.Add(walle);
            robotlist.Add(irongiant);
            //for (int i = 0; i < 5; i++)
            //{
            //    Robot q = CreateRobot(15,0,0);
            //    robotlist.Add(q);
            //}
        }
        /// <summary>
        /// Add a 3D node to the Nodelist
        /// </summary>
        /// <param name="node"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void addNode(char node, double x, double y, double z)
        {
            Node n = new Node(node, x, y, z);
            NodeList.Add(n);
        }
        //
        /// <summary>
        /// Tell a single, nearby  robot to pick up an item
        /// </summary>
        public void CommandPickup()
        {
            char start = 'B';
            char stop = ' ';
            // Check which storagespot still has space
            for (int i = 0; i < StorageSpots.Count; i++)
            {
                if (!StorageSpots[i].IsFull())
                {
                    stop = StorageSpots[i].DropoffNode.name;
                }
            }
            // Tell a  nearby robot to  pick up an item
            for (int i = 0; i < robotlist.Count; i++)
            {
                if (robotlist[i].idle)
                {
                  
                    if (robotlist[i].PickupRek())
                    {
                        robotlist[i].idle = false;
                        robotlist[i].SetRoute(GenerateRoute(start, stop), stop);
                    }
                    else
                    {
                        Console.WriteLine("No Nearby barrels to pick up for robot "+ robotlist[i]);
                    }
                  
                    return;
                }
                
            }
        }
        /// <summary>
        /// Tell a single, nearby robot to fetch an item
        /// </summary>
        public void CommandDeliver()
        {
            char start = 'B';
            char stop = ' ';
            // Check which storagespot is full
            for (int i = 0; i < StorageSpots.Count; i++)
            {
                if (!StorageSpots[i].IsEmpty())
                {
                    stop = StorageSpots[i].DropoffNode.name;
                }
            }
            // Tell a  nearby robot to fetch an item
            for (int i = 0; i < robotlist.Count; i++)
            {
                if (robotlist[i].idle)
                {
                        robotlist[i].idle = false;
                    robotlist[i].isFetching = true;
                    robotlist[i].SetRoute(GenerateRoute(start, stop), stop);
                    


                    return;
                }

            }
        }
        /// <summary>
        /// Set the initial 3D nodes for the nodelist
        /// </summary>
        public void InitialNodes()
        {
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
        }

        /// <summary>
        /// Returns a route to a positon , and back 
        /// </summary>
        /// <param name="start">start char</param>
        /// <param name="end">end char</param>
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
        /// <summary>
        /// Generate a route to a target
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onewayticket"></param>
        /// <returns></returns>
        public List<char> GenerateRoute(char start, char end,bool onewayticket)
        {
            List<char> Route = d.shortest_path(start, end);
            Route.Reverse();
            return Route;
        }
        private Trein SpawnTrein(double x, double y, double z)
        {
            Trein t = new Trein(x, y, z, 0, 0, 0,this);
            worldObjects.Add(t);
            return t;
        }
        public Rek CreateRek(double x, double y, double z)
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
        /// <summary>
        /// Gets called by the train when it arrives. 
        /// </summary>
        /// <param name="_t"></param>
        public void TrainArrived(Trein _t)
        {
            // Dump de barrels op het perron. Word aangeroepen wanneer een trein (_t) bij het loading dock is
            foreach (var item in _t.Cargo)
            {
                
                Perron_Cargo.Add(item);

                item.readyforpickup = true;
            }
            // Make sure the barrels are positioned properly
            for (int i = 0; i < Perron_Cargo.Count(); i++)
            {
                Perron_Cargo[i].Move(10 + (i * 1.5), 0, 0);
            }
            //Loop door robots en laat een idle robot de cargo ophalen
            for (int i = 0; i < _t.Barrels_to_Store; i++)
            {
                CommandPickup();
            }
            for (int i = 0; i < _t.Barrels_to_Load; i++)
            {
                CommandDeliver();
               
            }

        }


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
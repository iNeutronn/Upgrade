using System.Drawing;

namespace UpgradeServer
{
    public class Player
    {
        public string Name;
        public int ID;
        public int pos;
        public Point Location;
        static int nextID = 1;
        public Player(string n)
        {
            Name = n;
            ID = nextID;
            nextID++;
            pos = 0;
        }
    }
}

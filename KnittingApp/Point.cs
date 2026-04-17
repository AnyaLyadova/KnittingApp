using System.Text.Json.Serialization;
using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class Point
    {
        double x;
        double y;

        [JsonPropertyName("x")]
        public double X { get { return x; } set { x = value; } }

        [JsonPropertyName("y")]
        public double Y { get { return y; } set { y = value; } }

        string part;  //какой части принадлежит точка

        [JsonPropertyName("part")]
        public string Part { get { return part; } }

        // List<Point> connections=new List<Point>();
        Point connection;  //точка, с которой есть соединение
        

        public Point(double x, double y, string part)
        {
            this.x = x;
            this.y = y;
            this.part = part;
        }
        public void AddConnection(Point p)
        {
            //connections.Add(p);
            connection = p;
        }
        public void RemoveConnection(Point p)
        {
            //return connections.Remove(p);
            connection = null;
        }

        public Point GetConnection() 
        {
            //return connections; 
            return connection;
        }

        public Draft CreateDraft(List<Draft> parts)
        {
            throw new Exception();
        }

        
    }


    /*public class GridPoint
    {
        public int Row { get; set; }        // номер ряда (Y)
        public int Stitch { get; set; }     // номер петли (X)
        List<GridPoint> connections = new List<GridPoint>();

        public void AddConnection(GridPoint p)
        {
            connections.Add(p);
        }
        public bool RemoveConnection(GridPoint p)
        {
            return connections.Remove(p);
        }

        public List<GridPoint> GetConnections()
        {
            return connections;
        }

    }*/

}

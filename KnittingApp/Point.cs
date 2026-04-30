using System.Text.Json.Serialization;
using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class Point
    {
        double x;
        double y;

        public Point() { }

        [JsonPropertyName("visible")]
        public bool visible { get; set; }

        [JsonPropertyName("x")]
        public double X { get { return x; } set { x = value; } }

        [JsonPropertyName("y")]
        public double Y { get { return y; } set { y = value; } }

        string part;  //какой части принадлежит точка

        [JsonPropertyName("part")]
        public string Part { get { return part; } set { part = value; } }

        // List<Point> connections=new List<Point>();
      //  Point connection;  //точка, с которой есть соединение
        

        public Point(double x, double y, string part)
        {
            this.x = x;
            this.y = y;
            this.part = part;
            this.visible = false ;
        }


        public override bool Equals(object obj)  //переопределяем метод для сравнения и получения точек из списка
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Point other = (Point)obj;
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        /* public void AddConnection(Point p)
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
         }*/


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

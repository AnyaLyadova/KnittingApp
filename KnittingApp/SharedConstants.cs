namespace KnittingApp
{
    public static class SharedConstants
    {


        public enum LoopType  //тип петель
        {
            loop,
            increase,
            decrease,
            none
        }

        public enum LoopSide  //лицевые и изнаночные петли
        {
            front,
            back
        }

        public enum NeckType  //тип горловины
        {
         //   noNeck,
            vNeck,
            oNeck
        }

        //названия частей
        public const string BodyName = "body";
        public const string SleeveName = "sleeve";
        public const string ArmholeName = "armhole";
        public const string ONeckName = "oneck";
        public const string VNeckName = "vneck";
        public const string QuadName = "quad";
        public const string SleeveRollName = "sleeveRoll";
        public const string ShoulderName = "shoulder";

        //приоритет частей для порядка построения

        public enum PartPriority
        {
            Body = 10,
            Armhole = 20,
            Shoulder = 30,
            Neck = 40,
            Sleeve=50,
            SleeveRoll = 60,
        }
        //мерки частей

        public static Dictionary<string, double> NeckMeasures = new Dictionary<string, double>
        {
            {"neckWidth",20 },
            {"neckHeight",5 },
          //  {"IsFront",1 }
        };

        public static Dictionary<string, double> ArmholeMeasures = new Dictionary<string, double>
        {
            {"armholeWidth",8 },
            {"armholeHeight",20 },
        };

        public static Dictionary<string, double> ShoulderMeasures = new Dictionary<string, double>
        {
            {"shoulderWidth",15 },
            {"shoulderHeight",5 },
        };

        public static Dictionary<string, double> BodyMeasures = new Dictionary<string, double>
        {
            {"bodyHeight",60 },
            {"topWidth",80 },
            {"bottomWidth",100 }
        };

        public static Dictionary<string, double> QuadMeasures = new Dictionary<string, double>
        {
            {"quadHeight",60 },
            {"topWidth",50 },
            {"bottomWidth",50 }
        };

        public static Dictionary<string, double> SleeveMeasures = new Dictionary<string, double>
        {
            {"sleeveWidth",36},
            {"sleeveHeight",45 },
        };

        public static Dictionary<string, double> SleeveRollMeasures = new Dictionary<string, double>
        {
            {"sleeveRollWidth",24 },
            {"sleeveRollHeight",18 },
        };

        //списки частей для выбора
        public static Dictionary<string, string> NeckParts = new Dictionary<string, string>
        {
           // {NeckType.noNeck.ToString(), "Без горловины"},
           {ONeckName, "Круглая горловина"},
           { VNeckName,"V-образная горловина"}
        };

        public static Dictionary<string, string> SleeveRollParts = new Dictionary<string, string>
        {
            {SleeveRollName, "Скос рукава"},
            {"noSleeveRoll", "Без скоса рукава"}
        };

        public static Dictionary<string, string> ArmholeParts = new Dictionary<string, string>
        {
            { ArmholeName, "Выемка под рукав" },
            { "noArmhole", "Без выемки под рукав" }
        };


        //прибавки в ширину и в высоту для перемещения точки
        public static int AddingM = 30;
        public static int AddingN = 30;


 /*       public static List<Model> BaseModels= new List<Model>
        {
           
        }*/
    }
}

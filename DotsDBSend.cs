using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using MongoDB.Bson;
using MongoDB.Driver;

using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace DotsDBProj
{
    public class DotsDBSend : GH_Component
    {
        public DotsDBSend()
          : base("DotsDBProj", "send-DB",
              "Send Information to Database",
              "DOTS", "Database")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // 0. curves
            pManager.AddCurveParameter("Input Curves", "inp-crvs", "", GH_ParamAccess.list);
            // 1. name of db collection
            pManager.AddTextParameter("Name of the Model in remote db", "name_model", "names of the model in the remote database - mLab", GH_ParamAccess.item);
            // 2. names of the curves
            pManager.AddTextParameter("Key(name)", "keys", "names of the curves", GH_ParamAccess.list);
            // 3. write to db
            pManager.AddBooleanParameter("write to db (T/F)", "write-DB ?", "write to databse true or false", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("debug-str", "debug-str", "debug-str", GH_ParamAccess.list);
            pManager.AddTextParameter("send info to DB", "send-DB", "send string to database", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            var dbList = dbClient.ListDatabases().ToList();
            List<string> dbNames = new List<string>();
            foreach(var item in dbList)
            {
                dbNames.Add(item.ToString());
            }
            DA.SetDataList(0, dbNames);


            List<Curve> crvs = new List<Curve>();
            string modelName = "testCollection";
            List<string> keyLi = new List<string>();
            bool writeDB = false;

            if (!DA.GetDataList(0, crvs)) return;
            if (!DA.GetData(1, ref modelName)) return;
            if (!DA.GetDataList(2, keyLi)) return;
            if (!DA.GetData(3, ref writeDB)) return;

            List<string> strLi = new List<string>();
            if (writeDB == false)
            {
                strLi.Add("NOT WRITING TO DB");
                DA.SetDataList(1, strLi);
                return;
            }


            MongoCrud db = new MongoCrud("dots");
            string ptStr = ""; // connect all the points (coor by "," & pt by ";") of the polygon separated by "-"
            for (int i = 0; i < crvs.Count; i++)
            {
                ptStr += GetCrvPts(crvs[i]);
                string rkey = "x";
                if (keyLi.Count == crvs.Count)
                {
                    rkey = keyLi[i];
                }
                
            }
            string username = Environment.UserName;
            ConfigModel configmodel = new ConfigModel
            {
                UserName = username,
                ModelName = modelName,
                PolyArrInStr = ptStr
            };

            db.InsertRecord("DotsGH", configmodel);
            DA.SetDataList(1, strLi);
        }

        public string GetCrvPts(Curve crv)
        {
            List<Point3d> ptLi = new List<Point3d>();

            Polyline poly = new Polyline();
            var t = crv.TryGetPolyline(out poly);
            IEnumerator<Point3d> pts = poly.GetEnumerator();
            while (pts.MoveNext())
            {
                ptLi.Add(pts.Current);
            }

            Point3d[] ptArr = new Point3d[ptLi.Count];
            ptArr = ptLi.ToArray();

            string s = "";
            for(int i=0; i<ptArr.Length; i++)
            {
                Point3d p = ptArr[i];
                s += p.X + "," + p.Y + "," + p.Z+";";
            }
            s += "/";
            return s;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6aac04fd-f222-479d-8bca-dc721fc553ee"); }
        }
    }

    public class ConfigModel
    {
        public string UserName { get; set; }
        public string ModelName { get; set; }
        public String PolyArrInStr { get; set; }
    }

    public class MongoCrud
    {
        IMongoDatabase db;
        public MongoCrud(string database)
        {
            string localUri = "mongodb://127.0.0.1:27017";
            string uri = "mongodb://dotsUser:dotsUser07@ds215388.mlab.com:15388/dots?retryWrites=false";

            var localClient = new MongoClient(localUri);
            var remoteClient = new MongoClient(uri);
            
            db = remoteClient.GetDatabase(database);
        }
        public void InsertRecord<T> (string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }
    }

}

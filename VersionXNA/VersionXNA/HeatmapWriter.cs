using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using LoveSeat;
using LoveSeat.Interfaces;
using Newtonsoft.Json;

namespace GameDesign_2
{
    public class HeatmapWriter
    {

        public HeatmapWriter()
        {
        }

        public void WriteData(Vector2 position, int level, int diedInPhase, int totalPhases, bool died = false)
        {
            //Create the object.
            JsonObject obj = new JsonObject();
            obj.X = (int)position.X;
            obj.Y = (int)position.Y;
            obj.Level = level;
            obj.DiedInPhase = diedInPhase;
            obj.TotalPhases = totalPhases;
            obj.Died = died;

            //Use string json = JsonConvert.SerializeObject(object); to get the object in json format.
            string json = JsonConvert.SerializeObject(obj);
            
            //Connect with the database.
            var client = new CouchClient("admin", "admin");
            var db = client.GetDatabase("heatmap");

            //Now add the object to the database.
            db.CreateDocument(json);

            
        }

        public bool Isinitialised()
        {
            var client = new CouchClient("admin", "admin");
            return client.HasDatabase("heatmap");
        }
    }

    public struct JsonObject
    {
        //(X, Y) position using integers.
        public int X;
        public int Y;

        //Which level did we play?
        public int Level;

        //In which phase are we?
        public int DiedInPhase;
        public int TotalPhases;

        //Are we dead?
        public bool Died;
    }
}

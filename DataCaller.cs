using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Discord;
using System.Reflection.Metadata;

namespace LoadoutRandomizer
{
    class DataCaller
    {
        private string _connectionString = "mongodb://localhost:27017";
        private string _dataBankName = "TF2_Weapons_Catalog";
        public string callPrimary(string parameter)
        {
            string primary;
            string collectionName = parameter;

            if (collectionName == "invalid")
            {
                return ("invalid Request");
            }
           
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            
            var filter = Builders<BsonDocument>.Filter.Eq("slot", "primary");
            var projection = Builders<BsonDocument>.Projection.Include("name").Exclude("_id");

            var result = collection.Find(filter).Project(projection).ToList();
            var namesArray = result.Select(document => document.GetValue("name").AsString).ToArray();

            
            var r = new Random();
            int randomIndex = r.Next(0, namesArray.Length);
            primary = namesArray[randomIndex];
            
            return primary;
        }

        public string callSecondary(string parameter)
        {
            string response;
            string collectionName = parameter;
            if (collectionName == "invalid")
            {
                return ("invalid Request");
            }

            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var filter = Builders<BsonDocument>.Filter.Eq("slot", "secondary");
            var projection = Builders<BsonDocument>.Projection.Include("name").Exclude("_id");

            var result = collection.Find(filter).Project(projection).ToList();
            var namesArray = result.Select(document => document.GetValue("name").AsString).ToArray();


            var r = new Random();
            int randomIndex = r.Next(0, namesArray.Length);
            response = namesArray[randomIndex];

            return response;
        }

        public string callMelee(string parameter)
        {
            string response;
            string collectionName = parameter;

            if (collectionName == "invalid")
            {
                return ("invalid Request");
            }

            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var filter = Builders<BsonDocument>.Filter.Eq("slot", "melee");
            var projection = Builders<BsonDocument>.Projection.Include("name").Exclude("_id");

            var result = collection.Find(filter).Project(projection).ToList();
            var namesArray = result.Select(document => document.GetValue("name").AsString).ToArray();


            var r = new Random();
            int randomIndex = r.Next(0, namesArray.Length);
            response = namesArray[randomIndex];

            return response;
        }

        // callSapper and callWatch will be called when the parameter is spy.
        public string callSapper()
        {
            string sapper;
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>("spy");

            var filter = Builders<BsonDocument>.Filter.Eq("slot", "sapper");
            var projection = Builders<BsonDocument>.Projection.Include("name").Exclude("_id");
            var result = collection.Find(filter).Project(projection).ToList();
            var namesArray = result.Select(document => document.GetValue("name").AsString).ToArray();


            var r = new Random();
            int randomIndex = r.Next(0, namesArray.Length);
            sapper = namesArray[randomIndex];
            return sapper;
        }

        public string callWatch()
        {
            string sapper;
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>("spy");

            var filter = Builders<BsonDocument>.Filter.Eq("slot", "watch");
            var projection = Builders<BsonDocument>.Projection.Include("name").Exclude("_id");
            var result = collection.Find(filter).Project(projection).ToList();
            var namesArray = result.Select(document => document.GetValue("name").AsString).ToArray();


            var r = new Random();
            int randomIndex = r.Next(0, namesArray.Length);
            sapper = namesArray[randomIndex];
            return sapper;
        }

        public string[] callReskin(string parameter)
        {
            string reskin;
            string replace;
            string tfclass;

            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>("reskins");
            if (parameter == "invalid")
            {
                string[] error = { "invalid Request", "parameter not found" };
                return error;
            }

            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("tf_class", parameter),
                Builders<BsonDocument>.Filter.ElemMatch("tf_class", Builders<BsonDocument>.Filter.Eq("tf_class", parameter)));

            var projection = Builders<BsonDocument>.Projection
                .Include("name")
                .Include("replace")
                .Include("tf_class")
                .Exclude("_id");

            var result = collection.Find(filter).Project(projection).ToList();

            var namesArray = result.Select(document => document.GetValue("name").AsString).ToArray();
            var replaceArray = result.Select(document => document.GetValue("replace").AsString).ToArray();
            var classArray = result.Select(document => document.GetValue("tf_class").AsString);

            var r = new Random();
            int randomIndex = r.Next(0, namesArray.Length);
            reskin = namesArray[randomIndex];
            replace = replaceArray[randomIndex];
            tfclass = parameter;

            string[] response = { reskin, replace };
            return response;
        }

        //Gets Images of the called Weapons
        public string callImages(string name, string tfClass, bool reskin)
        {
            string collectionName = tfClass;
            string image = "nothing";
            var filter = Builders<BsonDocument>.Filter.Eq("name", name);

            if (reskin)
            {
                collectionName = "reskins";
                filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("tf_class", tfClass),
                Builders<BsonDocument>.Filter.ElemMatch("tf_class", Builders<BsonDocument>.Filter.Eq("tf_class", tfClass))       
                    ),Builders<BsonDocument>.Filter.Eq("name", name));
            }

            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBankName);
            var collection = database.GetCollection<BsonDocument>(collectionName);


            var projection = Builders<BsonDocument>.Projection
                .Include("image")
                .Exclude("_id");
            

            var resultDocument = collection.Find(filter).Project(projection).FirstOrDefault();
            if (resultDocument != null)
            {
                image = resultDocument["image"].AsString; // Retrieve the URL string
            }

            return image;
        }

        //In case of different classes, the programm will select which class will be used.
        public string collectionRandomizer(string parameter)
        {
            
            string[] classes = {"scout", "soldier", "pyro", "demoman", "heavy", "engineer", "medic", "sniper", "spy"};
            Random r = new Random();

            string response;
            if (parameter == "random")
            {
                int index = r.Next(0, classes.Length);
                response = classes[index];
                Console.WriteLine($"class found: {response} ");
            }
            else
            {
                response = parameter.ToLower();
            }
            
            //checks if the parameter is not one of the classes or random.
            //Will cancel the command and send an error embed in Discord.
            if (!classes.Contains(parameter.ToLower()) && parameter != "random")
            {
                return "invalid";
            }

            Console.WriteLine($"selected {response}");
            return response;
        }
    }
}

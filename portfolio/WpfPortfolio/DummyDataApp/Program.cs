using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyDataApp
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            while(true)
            {
                var Rooms = new[] { "DINNING", "LIVING", "BATH", "BED" }; //부엌,거실,욕실,침실
                var sensorDummy = new Faker<SensorInfo>()
                    .RuleFor(r => r.DevId, f => f.PickRandom(Rooms))
                    .RuleFor(r => r.CurrTime, f => f.Date.Past(0))  //Date.Past(0) == 현재
                    .RuleFor(r => r.Temp, f => f.Random.Float(19.0f, 30.9f)) //19도~30.9도
                    .RuleFor(r => r.Humid, f => f.Random.Float(40.0f, 63.9f)); //40씨~63.9씨

                var currValue = sensorDummy.Generate();

                Console.WriteLine(JsonConvert.SerializeObject(currValue,Formatting.Indented));

                Thread.Sleep(1000);
            }
        }

    }
}

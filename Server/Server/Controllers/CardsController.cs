using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.Helpers;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        #region Serialization

        private static List<Card> DeserializeFromJson()
        {
            List<Card> list = JsonConvert.DeserializeObject<List<Card>>(JsonHelper.Read("cards.json", "Data"));
            return list;
        }

        private static void SerializeToJson(List<Card> num)
        {
            string jsonString = JsonConvert.SerializeObject(num);
            JsonHelper.Write("cards.json", "Data", jsonString);
        }

        #endregion

        [HttpGet]
        public IEnumerable<Card> Get()
        {
            var list = JsonConvert.DeserializeObject<List<Card>>(JsonHelper.Read("cards.json", "Data"));
            return list;
        }

        [HttpPost]
        public IActionResult Post(Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var list = DeserializeFromJson();

            list.Add(card);

            SerializeToJson(list);

            return Ok();
        }

        [HttpPut]
        public IActionResult Put(Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var list = JsonConvert.DeserializeObject<List<Card>>(JsonHelper.Read("cards.json", "Data"));
            for (var i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i).Id == card.Id)
                {
                    list.ElementAt(i).Name = card.Name;
                    list.ElementAt(i).Map = card.Map;
                    SerializeToJson(list);
                    break;
                }

                if (i == list.Count - 1)
                    return BadRequest("Element not found");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var list = DeserializeFromJson();
            try
            {
                list.RemoveAt(list.FindIndex(card => card.Id == id));
            }
            catch
            {
                return BadRequest();
            }

            SerializeToJson(list);
            return Ok();
        }
    }
}
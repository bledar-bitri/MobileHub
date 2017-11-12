using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileHubWebApiCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<Value> Get()
        {
            return new Value[] { new Value{Id = 1, Text = "value 1"}, new Value { Id = 2, Text = "value 2" } };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Value Get(int id)
        {
            return new Value { Id = 1, Text = "value 1" };
        }

        // POST api/values
        [HttpPost]
        [Produces(typeof(Value))]
        public IActionResult Post([FromBody]Value value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return CreatedAtAction("Get", new {id = value.Id}, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Value value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Value
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
    }
}

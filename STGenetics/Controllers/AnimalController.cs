using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using STGenetics.DTOs;
using STGenetics.Models;
using STGenetics.Repository;
using System.IO;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace STGenetics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalRepository animalRepository;
        private readonly IOrderRepository orderRepository;
        public AnimalController(IAnimalRepository animalRepos, IOrderRepository orderRepos)
        {
            animalRepository = animalRepos;
            orderRepository = orderRepos;
        }
        // GET: api/<AnimalController>

        [HttpGet]
        [Route("Get")]
        [Authorize]
        public async Task<ActionResult<List<Animal>>> Get([FromBody] Object filterInfo)
        {
            if (filterInfo == null)
            {
                return BadRequest("Invalid filter parameter");
            }
            string filter = filterInfo.ToString() ?? string.Empty;
            var data = JsonConvert.DeserializeObject<dynamic>(filter);
            string filterBy = string.Empty;
            string value = string.Empty;
            if (data != null)
            {
                filterBy = data.filterBy.ToString();
                value = data.filterBy.ToString();
            }
            
            if (!(filterBy == "AnimalId" || filterBy == "Name" || filterBy == "Sex" || filterBy == "Status"))
            {
                return BadRequest("Invalid filter parameter");
            }
            var listAnimal = await animalRepository.GetByParameterAsync(filterBy, value);
            return listAnimal.ToList();
        }


        // POST api/<AnimalController>
        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] AnimalCreationDTO newAnimal)
        {
            Animal animal = new Animal
            {
                Name = newAnimal.Name,
                Breed = newAnimal.Breed,
                BirthDate = newAnimal.BirthDate,
                Sex = newAnimal.Sex,
                Price = newAnimal.Price,
                Status = newAnimal.Status
            };
            var listAnimal = await animalRepository.GetByParameterAsync(animal);
            if (listAnimal != null)
            {
                return new JsonResult(new { StatusCode = 400, Message = "Animal already inserted" });
            }
           
            var result = await animalRepository.AddAsync(animal);
            if (result)
            {
                return new JsonResult(new { StatusCode = 201, Message = "Animal created successfully" });
            }
            else
            {
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });
            }
        }


        // PUT api/<AnimalController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, [FromBody] AnimalCreationDTO animalDTO)
        {
            Animal animal = new Animal
            {
                AnimalId = id,
                Name = animalDTO.Name,
                Breed = animalDTO.Breed,
                BirthDate = animalDTO.BirthDate,
                Sex = animalDTO.Sex,
                Price = animalDTO.Price,
                Status = animalDTO.Status
            };
            var listAnimal = await animalRepository.GetByIdAsync(id);
            if (listAnimal == null)
            {
                return NotFound();
            }
           
            var result = await animalRepository.UpdateAsync(animal);
            if (result)
            {
                return new JsonResult(new { StatusCode = 200, Message = "Animal edited successfully" });
            }
            else
            {
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });
            }
        }

        // DELETE api/<AnimalController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await animalRepository.DeleteAsync(id);
            if (result)
            {
                return new JsonResult(new { StatusCode = 200, Message = "Animal deleted successfully" });
            }
            else
            {
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });
            }
        }

        // POST api/<AnimalController>
        [HttpPost]
        [Route("Order")]
        [Authorize]
        public async Task<ActionResult> CreateOrder([FromBody] OrderDTO order)
        {
            List<Animal> animals = new List<Animal>();
            decimal price = 0;
            if (order == null)
            {
                return BadRequest("Invalid filter parameter");
            }
            var orders = order.Animals ?? new List<AnimalOrderDTO>();
            foreach (var item in orders)
            {
                var animal = await animalRepository.GetByIdAsync(item.AnimalId);
                if (animal == null)
                {
                    return NotFound("Invalid animal ID: " + item.AnimalId);
                }
                if (animals.Any(x => x.AnimalId == item.AnimalId))
                {
                    return BadRequest("Duplicate animal ID: " + item.AnimalId);
                }
                animals.Add(animal);
                price += animal.Price;
            }

            //Applying diccounts
            if (animals.Count > 50)
            {
                decimal disccount = price * 0.05m;
                price -= disccount;
            }
            if (animals.Count > 200)
            {
                decimal disccount = price * 0.03m;
                price -= disccount;
            }
            if (!(animals.Count > 300))
            {
                price += 1000;
            }
            Order newOrder = new Order
            {
                Total = price
            };

            var result = await orderRepository.AddAsync(newOrder);
            if (result != 0)
            {
                return new JsonResult(new { StatusCode = 200, Message = $"Order made successfully  OrderID:{result} Total Amount:{price}" });
            }
            else
            {
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });
            }
        }
    }
}

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
            if (!FilterInfoCheck(filterInfo, out string filterBy, out string value))
                return BadRequest("Invalid filter parameter");

            var listAnimal = await animalRepository.GetByParameterAsync(filterBy, value);
            return listAnimal.ToList();

        }

        // POST api/<AnimalController>
        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] AnimalCreationDTO newAnimal)
        {
            Animal animal = AnimalSet(newAnimal, id: 0);
            IEnumerable<Animal> listAnimal = await animalRepository.GetByParameterAsync(animal);
            if (listAnimal != null)
                return new JsonResult(new { StatusCode = 400, Message = "Animal already inserted" });

            bool result = await animalRepository.AddAsync(animal);
            if (result)
                return new JsonResult(new { StatusCode = 201, Message = "Animal created successfully" });

            else
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });

        }


        // PUT api/<AnimalController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, [FromBody] AnimalCreationDTO animalDTO)
        {
            Animal animal = AnimalSet(animalDTO, id);

            Animal? listAnimal = await animalRepository.GetByIdAsync(id);
            if (listAnimal == null)
                return NotFound();

            var result = await animalRepository.UpdateAsync(animal);
            if (result)
                return new JsonResult(new { StatusCode = 200, Message = "Animal edited successfully" });

            else
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });

        }

        // DELETE api/<AnimalController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await animalRepository.DeleteAsync(id);
            if (result)
                return new JsonResult(new { StatusCode = 200, Message = "Animal deleted successfully" });

            else
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });

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
                return BadRequest("Invalid filter parameter");

            List<AnimalOrderDTO> orders = order.Animals ?? new List<AnimalOrderDTO>();
            foreach (AnimalOrderDTO item in orders)
            {
                Animal? animal = await animalRepository.GetByIdAsync(item.AnimalId);
                if (animal == null)
                    return NotFound("Invalid animal ID: " + item.AnimalId);

                if (animals.Any(x => x.AnimalId == item.AnimalId))
                    return BadRequest("Duplicate animal ID: " + item.AnimalId);

                animals.Add(animal);
                price += animal.Price;

            }

            Order newOrder = OrderGenerate(animals, price);

            int result = await orderRepository.AddAsync(newOrder);
            if (result != 0)
                return new JsonResult(new { StatusCode = 200, Message = $"Order made successfully  OrderID:{result} Total Amount:{price}" });

            else
                return new JsonResult(new { StatusCode = 400, Message = "Bad request" });


        }

        #region Misc.

        /// <summary>
        /// Validate Filter info before Retrieve data.
        /// </summary>
        /// <param name="filterInfo"></param>
        /// <param name="filterBy"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool FilterInfoCheck(Object filterInfo, out string filterBy, out string value)
        {
            filterBy = string.Empty;
            value = string.Empty;
            if (filterInfo == null)
                return false;

            string filter = filterInfo.ToString() ?? string.Empty;
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(filter);

                if (data == null)
                    return false;

                filterBy = data.filterBy.ToString();
                value = data.value.ToString();

                return filterBy == "AnimalId" || filterBy == "Name" || filterBy == "Sex" || filterBy == "Status";
            }
            catch (Exception)
            {
                return false;
            }

        }



        /// <summary>
        /// Set Animal DTO to Animal 
        /// </summary>
        /// <param name="newAnimal"></param>
        /// <returns></returns>
        private Animal AnimalSet(AnimalCreationDTO newAnimal, int id)
        {

            Animal animal = new Animal();
            if (newAnimal != null)
            {
                if (id != 0)
                    animal.AnimalId = id;

                animal.Name = newAnimal.Name;
                animal.Breed = newAnimal.Breed;
                animal.BirthDate = newAnimal.BirthDate;
                animal.Sex = newAnimal.Sex;
                animal.Price = newAnimal.Price;
                animal.Status = newAnimal.Status;

            }

            return animal;

        }


        /// <summary>
        /// Apply discounts to generate Order.
        /// </summary>
        /// <param name="animals"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        private Order OrderGenerate(List<Animal> animals, decimal price)
        {
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
                price += 1000;

            Order newOrder = new Order
            {
                Total = price
            };

            return newOrder;

        }

        #endregion Misc.

    }
}

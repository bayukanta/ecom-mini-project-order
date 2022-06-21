using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.DTO;
//using API.Security;
using BLL;
//using BLL.Redis;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private IMapper _mapper;
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger, IUnitOfWork uow, IMapper mapper) //, IGameMessageSenderFactory msgSernderFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _orderService ??= new OrderService(uow); //msgSernderFactory);
        }

        /// <summary>
        /// Get all order by user that is not pending
        /// </summary>
        /// <param id="userId"> user id.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpGet]
        [Route("{userId}")]
        [ProducesResponseType(typeof(List<OrderDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetByUserId([FromRoute] Guid userId)
        {
            List<DAL.Models.Order> result = await _orderService.GetAllOrderByUserIdAsync(userId);
            if (result != null)
            {
                List<OrderDTO> mappedResult = _mapper.Map<List<OrderDTO>>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Get pending order by user to use as cart
        /// </summary>
        /// <param id="userId"> user id.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpGet]
        [Route("cart/{userId}")]
        [ProducesResponseType(typeof(List<OrderDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetPendingByUserId([FromRoute] Guid userId)
        {
            DAL.Models.Order result = await _orderService.GetOrderByUserIdAsync(userId);
            if (result != null)
            {
                OrderDTO mappedResult = _mapper.Map<OrderDTO>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Create cart/order if there are no order that is pending 
        /// </summary>
        /// <param order="order">order data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> CreateAsync([FromBody] OrderDTO orderDTO)
        {
            try
            {
                DAL.Models.Order order = await _orderService.GetOrderByUserIdAsync(orderDTO.UserId);
                if (order == null)
                {
                    order = _mapper.Map<DAL.Models.Order>(orderDTO);
                    await _orderService.CreateOrderAsync(order);

                }
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Update order 
        /// </summary>
        /// <param order="order">order data.</param>
        /// <response code="200">Request ok.</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(OrderDTO), 200)]
        public async Task<ActionResult> Update([FromBody] OrderDTO orderDTO)
        {
            DAL.Models.Order order = _mapper.Map<DAL.Models.Order>(orderDTO);
            await _orderService.UpdateOrderAsync(order);
            return new OkResult();
        }
    }
}

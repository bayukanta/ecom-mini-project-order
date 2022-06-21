using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.DTO;
//using API.Security;
using BLL;
using BLL.Redis;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BLL.Eventhub;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly OrderDetailService _orderDetailService;
        private IMapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public OrderDetailController(ILogger<ProductController> logger, IUnitOfWork uow, IMapper mapper, IRedisService redis, IConfiguration config, IOrderEventSenderFactory msgSernderFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _orderDetailService??= new OrderDetailService(uow, redis, config, msgSernderFactory);
        }

        /// <summary>
        /// Get all orderDetail by orderId
        /// </summary>
        /// <param id="userId"> user id.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(List<OrderDetailDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetByOrderId([FromRoute] Guid id)
        {
            List<DAL.Models.OrderDetail> result = await _orderDetailService.GetAllOrderDetailByOrderIdAsync(id);
            if (result != null)
            {
                List<OrderDetailWithDataDTO> mappedResult = _mapper.Map<List<OrderDetailWithDataDTO>>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Create orderDetail 
        /// </summary>
        /// <param orderDetail="orderDetail">orderDetail data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> CreateAsync([FromBody] OrderDetailDTO orderDetailDTO)
        {
            try
            {
                DAL.Models.OrderDetail od = _mapper.Map<DAL.Models.OrderDetail>(orderDetailDTO);
                await _orderDetailService.CreateOrderDetailAsync(od);
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new BadRequestResult();
            }
        }

    }
}

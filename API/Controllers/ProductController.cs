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


namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController
    {
        private readonly ProductService _productService;
        private IMapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger, IUnitOfWork uow, IMapper mapper, IRedisService redis) //, IGameMessageSenderFactory msgSernderFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _productService ??= new ProductService(uow, redis); //msgSernderFactory);
        }

        /// <summary>
        /// Get all list product
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<ProductDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> GetAllAsync()
        {

            List<DAL.Models.Product> result = await _productService.GetAllProductAsync();
            List<ProductDTO> mappedResult = _mapper.Map<List<ProductDTO>>(result);
            return new OkObjectResult(mappedResult);
        }

        
       

        /// <summary>
        /// Get all by filter
        /// </summary>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(List<ProductDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> FilterPost([FromBody] ProductSearchDTO productSearchDTO)
        {
            DAL.Models.Product product = _mapper.Map<DAL.Models.Product>(productSearchDTO);
            List<DAL.Models.Product> result = await _productService.FilterPost(product);
            if (result != null)
            {
                List<ProductDTO> mappedResult = _mapper.Map<List<ProductDTO>>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }

        

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param id="id">product Id</param>
        /// <response code="200">Request ok.</response>
        /// <response code="405">Request not found.</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> GetById([FromRoute] Guid id)
        {
            DAL.Models.Product result = await _productService.GetProductByIdAsync(id);
            if (result != null)
            {
                ProductDTO mappedResult = _mapper.Map<ProductDTO>(result);
                return new OkObjectResult(mappedResult);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Create product 
        /// </summary>
        /// <param product="product">product data.</param>
        /// <response code="200">Request ok.</response>
        /// <response code="400">Request failed because of an exception.</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        //[Authorize]
        public async Task<ActionResult> CreateAsync([FromBody] ProductDTO productDTO)
        {
            try
            {
                DAL.Models.Product product = _mapper.Map<DAL.Models.Product>(productDTO);
                await _productService.CreateProductAsync(product);
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Update product 
        /// </summary>
        /// <param product="product">product data.</param>
        /// <response code="200">Request ok.</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        public async Task<ActionResult> Update([FromBody] ProductDTO productDTO)
        {
            DAL.Models.Product product = _mapper.Map<DAL.Models.Product>(productDTO);
            await _productService.UpdateProductAsync(product);
            return new OkResult();
        }

        /// <summary>
        /// Delete product 
        /// </summary>
        /// <response code="200">Request ok.</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return new OkResult();
        }
    }
}

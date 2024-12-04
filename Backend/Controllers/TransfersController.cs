using System.Collections.Generic;
using Backend.Features.Transfers;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Transfers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpGet]
        public IEnumerable<Transfer> GetAllTransfers()
        {
            return _transferService.GetAllTransfers();
        }

        [HttpPost]
        public void AddTransfer([FromBody] Transfer transfer)
        {
            _transferService.AddTransfer(transfer);
        }

        [HttpGet("{id}")]
        public Transfer? GetTransferById(int id)
        {
            return _transferService.GetTransferById(id);
        }

        [HttpPut("{id}")]
        public void UpdateTransfer([FromBody] Transfer transfer)
        {
            _transferService.UpdateTransfer(transfer);
        }
    }
}

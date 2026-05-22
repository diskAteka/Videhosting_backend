using MainServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.GlobalInterfaces;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("wpf")]
    public class WPFController : ControllerBase
    {
        private readonly IAddObjectService _addObjectService;
        private readonly IDeleteObjectService _deleteObjectService;
        private readonly IUpdateObjectService _updateObjectService;
        private readonly IGetObjectService _getObjectService;
        private readonly IVideoService _videoService;

        public WPFController(IAddObjectService addObjectService,
                             IDeleteObjectService deleteObjectService,
                             IUpdateObjectService updateObjectService,
                             IGetObjectService getObjectService,
                             IVideoService videoService)
        {
            _addObjectService = addObjectService;
            _deleteObjectService = deleteObjectService;
            _updateObjectService = updateObjectService;
            _getObjectService = getObjectService;
            _videoService = videoService;
        }

        [Route("delete/comment{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            try
            {

            }
        }
    }
}

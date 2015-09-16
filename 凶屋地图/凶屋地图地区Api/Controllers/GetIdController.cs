using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace 凶屋地图地区Api.Controllers
{
    public class GetIdController : ApiController
    {

        [HttpGet]
        [ActionName("test")]
        public IHttpActionResult GetDetail(int id)
        {
            MysqlHelper mysqlhelper = new MysqlHelper();
            return Ok(mysqlhelper.GetDetail(id.ToString()));
        }
        public string GetAllProducts()
        {
            MysqlHelper mysqlhelper = new MysqlHelper();
            return mysqlhelper.GetDetail("1");
        }
    }
}

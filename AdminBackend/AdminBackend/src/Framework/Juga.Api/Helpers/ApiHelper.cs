//using Juga.Abstractions.Application.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace Juga.Api.Helpers;

//public class ApiHelper
//{
//    private IActionResult ActionResultMaker<T>(Result<T> kimlikResult)
//    {
//        if (kimlikResult.ResultType == ResultType.Ok)

//            return Ok(kimlikResult);

//        if (kimlikResult.ResultType == ResultType.NotFound)

//            return NotFound();

//        if (kimlikResult.ResultType == ResultType.Invalid)
//            return BadRequest();
//        if (kimlikResult.ResultType == ResultType.Unauthorized)
//            return Unauthorized();
//        return Ok(kimlikResult);
//    }
//}
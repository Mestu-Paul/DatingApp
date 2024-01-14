﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AppUser>> GetUsers(){
        var users = _context.Users.ToList();
        return users;
    }

    [HttpGet("{id}")] // {base}/api/users/2
    public ActionResult<AppUser>GetUser(int id){
        return _context.Users.Find(id);
    }
}

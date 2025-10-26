﻿using StudHub.SharedDTO;

namespace Client.Services;

public interface IInventoryClientService
{
    Task<List<SetDTO>> GetUserSetsAsync(string email);
}
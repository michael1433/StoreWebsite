﻿using Microsoft.EntityFrameworkCore;
using StoreWebsite.Data;
using StoreWebsite.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWebsite.Services.Users
{
    public class UserService: IUserService
    {
        readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Address> GetAddressAsync(Guid addressId)
        {
            return _context.Addresses
                .FirstAsync(a => a.Id == addressId);
        }

        public async Task<bool> AddAddressAsync(Address address)
        {
            await _context.AddAsync(address);

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> UpdateUserAddressAsync(ApplicationUser user, Address address)
        {
            int removals = 0;
            var modifiedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            if (modifiedUser.AddressId.HasValue)
            {
                _context.Addresses.Remove(await GetAddressAsync(modifiedUser.AddressId.Value));
                removals++;
            }

            address.Id = Guid.NewGuid();
            modifiedUser.Address = address;

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 2 + removals;
        }
    }
}

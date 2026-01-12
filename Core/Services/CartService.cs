using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Cart;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
	public class CartService(AppDbTransferContext appDbTransferContext, IAuthService authService, IMapper mapper) : ICartService
	{
		public async Task AddUpdateAsync(CartAddUpdateModel model)
		{
			var userId = await authService.GetUserIdAsync();
			var cartItem = appDbTransferContext.Carts
				.SingleOrDefault(c => c.UserId == userId &&
					c.TransportationId == model.TransportationId);
			if (cartItem == null)
			{
				cartItem = new CartEntity
				{
					UserId = userId,
					TransportationId = model.TransportationId,
					CountTickets = model.Quantity
				};
				await appDbTransferContext.Carts.AddAsync(cartItem);
			}
			else {
				cartItem.CountTickets = model.Quantity;
			}
			await appDbTransferContext.SaveChangesAsync();
		}
		public async Task<List<CartItemModel>> GetListAsync()
		{
			var userId = await authService.GetUserIdAsync();
			var result = await appDbTransferContext.Carts
				.Where(c => c.UserId == userId)
				.ProjectTo<CartItemModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			return result;
		}
	}
}

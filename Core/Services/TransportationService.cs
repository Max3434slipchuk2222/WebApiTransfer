using AutoMapper;
using Core.Interfaces;
using Core.Models.Transportation;
using Domain;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace Core.Services;

public class TransportationService(AppDbTransferContext appDbTransferContext, IMapper mapper) : ITransportationService
{
	public async Task<List<TransportationItemModel>> GetListAsync()
	{
		var result = await appDbTransferContext.Transportations
			.ProjectTo<TransportationItemModel>(mapper.ConfigurationProvider)
			.ToListAsync();
		return result;
	}
}

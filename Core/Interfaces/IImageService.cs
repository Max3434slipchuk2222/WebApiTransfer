using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IImageService
{
	public Task<string> UploadImageAsync(IFormFile form);
}

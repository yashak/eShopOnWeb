using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;

/// <summary>
/// Updates a Catalog Item
/// </summary>
public class UpdateCatalogItemEndpoint : IEndpoint<IResult, UpdateCatalogItemRequest>
{
    private IRepository<CatalogItem> _itemRepository;
    private readonly IUriComposer _uriComposer;
    private IFileSystem _webFileSystem;

    public UpdateCatalogItemEndpoint(IUriComposer uriComposer)
    {
        _uriComposer = uriComposer;        
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("api/catalog-items",
            [Authorize(Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async
            (UpdateCatalogItemRequest request, IRepository<CatalogItem> itemRepository, IFileSystem webFileSystem) =>
            {
                _itemRepository = itemRepository;
                _webFileSystem = webFileSystem;
                return await HandleAsync(request);
            })
            .Produces<UpdateCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(UpdateCatalogItemRequest request)
    {
        var response = new UpdateCatalogItemResponse(request.CorrelationId());

        var existingItem = await _itemRepository.GetByIdAsync(request.Id);

        existingItem.UpdateDetails(request.Name, request.Description, request.Price);
        existingItem.UpdateBrand(request.CatalogBrandId);

        if (string.IsNullOrEmpty(request.PictureBase64) && string.IsNullOrEmpty(request.PictureUri))
        {
            existingItem.UpdatePictureUri("eCatalog-item-default.png");            
        }
        else
        {
            // TODO: fix this - save into storage account for temporary upload
            var cancellationToken = new System.Threading.CancellationToken();
            var picName = $"{existingItem.Id}{Path.GetExtension(request.PictureName)}";
            if (await _webFileSystem.SavePicture($"{picName}", request.PictureBase64, cancellationToken))
            {
                existingItem.UpdatePictureUri(picName);
            }
        }

        existingItem.UpdateType(request.CatalogTypeId);

        await _itemRepository.UpdateAsync(existingItem);

        var dto = new CatalogItemDto
        {
            Id = existingItem.Id,
            CatalogBrandId = existingItem.CatalogBrandId,
            CatalogTypeId = existingItem.CatalogTypeId,
            Description = existingItem.Description,
            Name = existingItem.Name,
            PictureUri = _uriComposer.ComposePicUri(existingItem.PictureUri),
            Price = existingItem.Price
        };
        response.CatalogItem = dto;
        return Results.Ok(response);
    }
}

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints;

/// <summary>
/// Creates a new Catalog Item
/// </summary>
public class CreateCatalogItemEndpoint : IEndpoint<IResult, CreateCatalogItemRequest>
{
    private IRepository<CatalogItem> _itemRepository;
    private readonly IUriComposer _uriComposer;
    private IFileSystem _webFileSystem;

    public CreateCatalogItemEndpoint(IUriComposer uriComposer)
    {
        _uriComposer = uriComposer;        
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/catalog-items",
            [Authorize(Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async
            (CreateCatalogItemRequest request, IRepository<CatalogItem> itemRepository, IFileSystem webFileSystem) =>
            {
                _itemRepository = itemRepository;
                _webFileSystem = webFileSystem;
                return await HandleAsync(request);
            })
            .Produces<CreateCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints");
    }

    public async Task<IResult> HandleAsync(CreateCatalogItemRequest request)
    {
        var response = new CreateCatalogItemResponse(request.CorrelationId());

        var catalogItemNameSpecification = new CatalogItemNameSpecification(request.Name);
        var existingCataloogItem = await _itemRepository.CountAsync(catalogItemNameSpecification);
        if (existingCataloogItem > 0)
        {
            throw new DuplicateException($"A catalogItem with name {request.Name} already exists");
        }        

        var newItem = new CatalogItem(request.CatalogTypeId, request.CatalogBrandId, request.Description, request.Name, request.Price, request.PictureUri);
        newItem = await _itemRepository.AddAsync(newItem);

        if (newItem.Id != 0)
        {
            // TODO: fix this - save into storage account for temporary upload
            var cancellationToken = new System.Threading.CancellationToken();
            var picName = $"{newItem.Id}{Path.GetExtension(request.PictureName)}";
            if (string.IsNullOrEmpty(request.PictureBase64) && await _webFileSystem.SavePicture(picName, request.PictureBase64, cancellationToken))
            {
                newItem.UpdatePictureUri(picName);
                await _itemRepository.UpdateAsync(newItem, cancellationToken);                
            }
            else {
                newItem.UpdatePictureUri("eCatalog-item-default.png");
                //We disabled the upload functionality and added a default/placeholder image to this sample due to a potential security risk 
                //  pointed out by the community. More info in this issue: https://github.com/dotnet-architecture/eShopOnWeb/issues/537 
                //  In production, we recommend uploading to a blob storage and deliver the image via CDN after a verification process.            
                await _itemRepository.UpdateAsync(newItem);
            }            
        }

        var dto = new CatalogItemDto
        {
            Id = newItem.Id,
            CatalogBrandId = newItem.CatalogBrandId,
            CatalogTypeId = newItem.CatalogTypeId,
            Description = newItem.Description,
            Name = newItem.Name,
            PictureUri = _uriComposer.ComposePicUri(newItem.PictureUri),
            Price = newItem.Price
        };
        response.CatalogItem = dto;
        return Results.Created($"api/catalog-items/{dto.Id}", response);       
    }
}

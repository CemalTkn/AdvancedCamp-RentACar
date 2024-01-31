using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Brands.Dtos;
using Application.Features.Brands.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommand :IRequest<CreatedBrandDto>
    {
        public string Name { get; set; }

        public class CreatedBrandCommandHandler:IRequestHandler<CreateBrandCommand, CreatedBrandDto>
        {
            private readonly IBrandRepository _brandRepository;
            private readonly IMapper _mapper;
            private readonly BrandBusinessRules _brandBusinessRules;

            public CreatedBrandCommandHandler(IBrandRepository brandRepository, IMapper mapper, BrandBusinessRules brandBusinessRules)
            {
               _brandRepository = brandRepository;
               _mapper = mapper;
               _brandBusinessRules = brandBusinessRules;
            }

            public async Task<CreatedBrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
            {
               await _brandBusinessRules.BrandNameCanNotBeDuplicatedWhenInserted(request.Name);//İş kuralı

               Brand mappedBrand = _mapper.Map<Brand>(request);//Kullanıcının oluşturacağı Brand ile verdiği Brand özelliğini mapliyoruz.
               Brand createdBrand = await _brandRepository.AddAsync(mappedBrand);//Kullanıcının verdiği Brand instancesini veritabanına ekliyoruz.
               CreatedBrandDto createdBrandDto = _mapper.Map<CreatedBrandDto>(createdBrand);//Oluşturlan Brand'in sadece kullanıcının görmesini istediğimiz özelliğini mapleyerek gönderiyoruz.
              
               return createdBrandDto;//Ve Kullanıcıya çıktı olarak gösteriyoruz.
            }
        }

    }
}

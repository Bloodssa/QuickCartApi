using QuickCartApi.Common;
using QuickCartApi.DTOs.Address;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Services
{
    public class AddressService(IAddressRepository addressRepository) : IAddressService
    {
        public async Task<List<AddressResponse>> GetAllAsync(int userId)
        {
            var addresses = await addressRepository.GetByUserIdAsync(userId);
            return addresses.Select(ToResponse).ToList();
        }

        public async Task<AddressResponse?> GetByIdAsync(int userId, int addressId)
        {
            var address = await addressRepository.GetByIdForUserAsync(addressId, userId);
            return address is null ? null : ToResponse(address);
        }

        public async Task<ServiceResult<AddressResponse>> CreateAsync(int userId, AddressRequest request)
        {
            // check if the user dont have any addess if the first create address then make it as the default address
            var makeDefault = request.IsDefault || await addressRepository.CountByUserAsync(userId) == 0;

            if (makeDefault)
                await addressRepository.ClearDefaultAsync(userId);

            var address = new Address
            {
                UserId = userId,
                IsDefault = makeDefault
            };

            Apply(address, request);
            await addressRepository.AddAsync(address);

            return Result.Ok(ToResponse(address));
        }

        public async Task<ServiceResult<AddressResponse>> UpdateAsync(int userId, int id, AddressRequest request)
        {
            var address = await addressRepository.GetByIdForUserAsync(id, userId);

            if (address is null)
                return Result.Fail<AddressResponse>(ErrorKind.NotFound, "Address not found.");

            //a default address cannot be switched off directly.
            if (request.IsDefault && !address.IsDefault)
            {
                await addressRepository.ClearDefaultAsync(userId);
                address.IsDefault = true;
            }

            Apply(address, request);
            await addressRepository.UpdateAsync(address);

            return Result.Ok(ToResponse(address));
        }

        public async Task<ServiceResult> DeleteAsync(int userId, int id)
        {
            var address = await addressRepository.GetByIdForUserAsync(id, userId);

            if (address is null)
                return Result.Fail(ErrorKind.NotFound, "Address not found.");

            // prevent deletion of the address that have an existing order
            if (await addressRepository.HasOrderAsync(id))
                return Result.Fail(ErrorKind.Conflict, "This address is used by existing orders and cannot be deleted.");

            bool wasDefault = address.IsDefault;

            await addressRepository.DeleteAsync(address);

            // if the address being deleted is default then make other address as default
            if (wasDefault)
            {
                var next = (await addressRepository.GetByUserIdAsync(userId)).FirstOrDefault();

                if (next is not null)
                {
                    next.IsDefault = true;
                    await addressRepository.UpdateAsync(next);
                }
            }

            return Result.Ok();
        }

        public async Task<ServiceResult> SetDefaultAsync(int userId, int id)
        {
            var address = await addressRepository.GetByIdForUserAsync(id, userId);

            if (address is null)
                return Result.Fail(ErrorKind.NotFound, "Address not found.");

            if (address.IsDefault)
                return Result.Ok();

            await addressRepository.ClearDefaultAsync(userId);

            address.IsDefault = true;
            await addressRepository.UpdateAsync(address);

            return Result.Ok();
        }

        private AddressResponse ToResponse(Address a)
            => new AddressResponse
            {
                Id = a.Id,
                RecipientName = a.RecipientName,
                ContactNumber = a.ContactNumber,
                AddressLine = a.AddressLine,
                Barangay = a.Barangay,
                City = a.City,
                Province = a.Province,
                PostalCode = a.PostalCode,
                IsDefault = a.IsDefault
            };
    
        private static void Apply(Address address, AddressRequest request)
        {
            address.RecipientName = request.RecipientName;
            address.ContactNumber = request.ContactNumber;
            address.AddressLine = request.AddressLine;
            address.Barangay = request.Barangay;
            address.City = request.City;
            address.Province = request.Province;
            address.PostalCode = request.PostalCode;
        }
    }
}

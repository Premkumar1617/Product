using SJBRAssociatesWeb.Models;

namespace SJBRAssociatesWeb.Services;

public interface IJsonDataService
{
    Task<List<TeamMember>> GetTeamAsync();
    Task<List<ServiceItem>> GetServicesAsync();
    Task<FirmContact> GetContactInfoAsync();
    Task SaveEnquiryAsync(EnquiryRecord enquiry);
    Task<List<EnquiryRecord>> GetEnquiriesAsync();
    Task<AdminUser?> GetAdminUserAsync(string username);
    Task UpdateAdminUserAsync(AdminUser user);
}

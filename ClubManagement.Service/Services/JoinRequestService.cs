using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.Services.Interfaces;

public class JoinRequestService : IJoinRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMembershipRepository _membershipRepo;
    public JoinRequestService(IUnitOfWork unitOfWork, IMembershipRepository membershipRepo)
    {
        _unitOfWork = unitOfWork;
        _membershipRepo = membershipRepo;
    }

    public async Task<IEnumerable<JoinRequest>> GetAllAsync()
    {
        return await _unitOfWork.JoinRequestRepository.GetAllAsync();
    }

    public async Task<JoinRequest?> GetByIdAsync(int id)
    {
        return await _unitOfWork.JoinRequestRepository.GetByIdAsync(id);
    }

    // ----------------------------------------------------
    // 1) Submit Join Request
    // ----------------------------------------------------
    public async Task<JoinRequest> SubmitAsync(int userId, int clubId, string? note)
    {
        // Check user exists
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User không tồn tại.");

        // Check club exists
        var club = await _unitOfWork.ClubRepository.GetByIdAsync(clubId);
        if (club == null)
            throw new Exception("Câu lạc bộ không tồn tại.");

        // Check active membership
        var isActive = await _unitOfWork.MembershipRepository
                                        .IsActiveMemberAsync(userId, clubId);
        if (isActive)
            throw new Exception("Bạn đã là thành viên của câu lạc bộ này.");

        // Check pending request
        var pending = await _unitOfWork.JoinRequestRepository
                                       .GetPendingRequestAsync(userId, clubId);

        if (pending != null)
            throw new Exception("Bạn đã có yêu cầu tham gia đang chờ duyệt.");

        var request = new JoinRequest
        {
            UserId = userId,
            ClubId = clubId,
            RequestDate = DateTime.Now,
            Status = "Pending",
            Note = note
        };

        await _unitOfWork.JoinRequestRepository.CreateAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return request;
    }

    // ----------------------------------------------------
    // 2) Approve Join Request
    // ----------------------------------------------------
    public async Task<JoinRequest> ApproveAsync(int requestId, int leaderId)
    {
        var request = await _unitOfWork.JoinRequestRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new Exception("Không tìm thấy đơn yêu cầu.");

        if (request.Status != "Pending")
            throw new Exception("Đơn đã được xử lý trước đó.");

        request.Status = "Approved";
        _unitOfWork.JoinRequestRepository.Update(request);

        // Check membership
        var existingMembership = await _membershipRepo.GetByUserAndClubAsync(request.UserId, request.ClubId);

        if (existingMembership != null)
        {
            existingMembership.Status = "Active";
            existingMembership.JoinedAt = DateTime.Now;

            _membershipRepo.Update(existingMembership);
        }
        else
        {
            var membership = new Membership
            {
                UserId = request.UserId,
                ClubId = request.ClubId,
                JoinedAt = DateTime.Now,
                Status = "Active",
                Role = "Member"
            };

            await _membershipRepo.CreateAsync(membership);
        }

        await _unitOfWork.SaveChangesAsync();
        return request;
    }

    // ----------------------------------------------------
    // 3) Reject Join Request
    // ----------------------------------------------------
    public async Task<JoinRequest> RejectAsync(int requestId, int leaderId, string? reason)
    {
        var request = await _unitOfWork.JoinRequestRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new Exception("Không tìm thấy đơn yêu cầu.");

        if (request.Status != "Pending")
            throw new Exception("Đơn đã được xử lý trước đó.");

        request.Status = "Rejected";

        if (!string.IsNullOrEmpty(reason))
        {
            request.Note =
                string.IsNullOrEmpty(request.Note)
                ? $"Lý do từ chối: {reason}"
                : $"{request.Note}\nLý do từ chối: {reason}";
        }

        _unitOfWork.JoinRequestRepository.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return request;
    }
}

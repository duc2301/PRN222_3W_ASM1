using AutoMapper;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _paymentRepo = unitOfWork.PaymentRepository;
        _mapper = mapper;
    }

    public async Task<Payment> CreatePaymentAsync(int userId, int feeId, decimal amount)
    {
        var payment = new Payment
        {
            UserId = userId,
            FeeId = feeId,
            Amount = amount,
            PaymentDate = DateTime.Now,
            Status = "Pending"
        };

        await _paymentRepo.CreateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment> GetByIdAsync(int id)
    {
        return await _paymentRepo.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _paymentRepo.GetAllWithDetailsAsync();
    }

    public async Task<IEnumerable<Payment>> GetByUserAsync(int userId)
    {
        return await _paymentRepo.GetByUserAsync(userId);
    }

    public async Task<bool> HasPaidAsync(int userId, int feeId)
    {
        return await _paymentRepo.HasPaidAsync(userId, feeId);
    }

    public async Task MarkAsPaidAsync(int paymentId)
    {
        var payment = await _paymentRepo.GetByIdAsync(paymentId);

        if (payment == null)
            throw new Exception("Payment not found");

        payment.Status = "Paid";
        payment.PaymentDate = DateTime.Now;

        _paymentRepo.Update(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RequestPaymentAsync(int paymentId)
    {
        var payment = await _paymentRepo.GetByIdAsync(paymentId);

        if (payment == null)
            throw new Exception("Payment not found");

        if (payment.Status is "Paid" or "Expired" or "Pending")
            throw new Exception("Invalid payment state");

        payment.Status = "Pending";
        payment.PaymentDate = DateTime.Now;

        _paymentRepo.Update(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CheckAndUpdateExpiredPaymentsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var payments = (await _paymentRepo.GetAllWithDetailsAsync())
            .Where(p => (p.Status == "Unpaid" || p.Status == "Pending")
                        && p.Fee.DueDate < today)
            .ToList();

        foreach (var payment in payments)
        {
            payment.Status = "Expired";
            _paymentRepo.Update(payment);
        }

        if (payments.Any())
            await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<PaymentResponseDTO>> GetPaymentsByUsernameAsync(string userName)
    {
        var data = await _paymentRepo.GetPaymentsByUsernameAsync(userName);
        return _mapper.Map<List<PaymentResponseDTO>>(data);
    }
}

using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Errors;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using Gatherly.Domain.ValueObjects;

namespace Gatherly.Application.Members.UpdateMember;

internal sealed class UpdateMemberCommandHandler(
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateMemberCommand>
{
    public async Task<Result> Handle(
        UpdateMemberCommand request,
        CancellationToken cancellationToken)
    {
        Member? member = await memberRepository.GetByIdAsync(
            request.MemberId,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure(
                DomainErrors.Member.NotFound(request.MemberId));
        }

        Result<FirstName> firstNameResult = FirstName.Create(request.FirstName);
        Result<LastName> lastNameResult = LastName.Create(request.LastName);

        member.ChangeName(
            firstNameResult.Value,
            lastNameResult.Value);

        memberRepository.Update(member);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

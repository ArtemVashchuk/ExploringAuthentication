using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;

namespace Gatherly.Application.Members.GetMemberById;

internal sealed class GetMemberByIdQueryHandler(IMemberRepository memberRepository)
    : IQueryHandler<GetMemberByIdQuery, MemberResponse>
{
    public async Task<Result<MemberResponse>> Handle(
        GetMemberByIdQuery request,
        CancellationToken cancellationToken)
    {
        Member? member = await memberRepository.GetByIdAsync(
            request.MemberId,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure<MemberResponse>(new Error(
                "Member.NotFound",
                $"The member with Id {request.MemberId} was not found"));
        }

        var response = new MemberResponse(member.Id, member.Email.Value);

        return response;
    }
}
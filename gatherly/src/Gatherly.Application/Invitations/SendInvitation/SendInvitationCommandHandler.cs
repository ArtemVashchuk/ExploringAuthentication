using Gatherly.Application.Abstractions;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using MediatR;

namespace Gatherly.Application.Invitations.SendInvitation;

internal sealed class SendInvitationCommandHandler(
    IMemberRepository memberRepository,
    IGatheringRepository gatheringRepository,
    IInvitationRepository invitationRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService)
    : IRequestHandler<SendInvitationCommand>
{
    public async Task<Unit> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        Member? member = await memberRepository
            .GetByIdAsync(request.MemberId, cancellationToken);

        Gathering? gathering = await gatheringRepository
            .GetByIdWithCreatorAsync(request.GatheringId, cancellationToken);

        if (member is null || gathering is null)
        {
            return Unit.Value;
        }

        Result<Invitation> invitationResult = gathering.SendInvitation(member);

        if (invitationResult.IsFailure)
        {
            // Log error
            return Unit.Value;
        }

        invitationRepository.Add(invitationResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Send email
        await emailService.SendInvitationSentEmailAsync(
            member,
            gathering,
            cancellationToken);

        return Unit.Value;
    }
}
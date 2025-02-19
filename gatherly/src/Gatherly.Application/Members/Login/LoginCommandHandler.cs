﻿using Gatherly.Application.Abstractions;
using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Errors;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using Gatherly.Domain.ValueObjects;

namespace Gatherly.Application.Members.Login;

internal sealed class LoginCommandHandler(
    IMemberRepository memberRepository,
    IJwtProvider jwtProvider)
    : ICommandHandler<LoginCommand, string>
{
    public async Task<Result<string>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        Result<Email> email = Email.Create(request.Email);

        Member? member = await memberRepository.GetByEmailAsync(
            email.Value,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure<string>(
                DomainErrors.Member.InvalidCredentials);
        }

        string token = jwtProvider.Generate(member);

        return token;
    }
}

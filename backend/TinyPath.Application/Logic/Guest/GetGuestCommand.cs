using MediatR;
using TinyPath.Application.Exceptions;
using TinyPath.Application.Interfaces;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Application.Services.Guest;

namespace TinyPath.Application.Logic.Guest;

public abstract class GetGuestCommand
{
    public class Request : IRequest<Response>
    {
    }
    
    public class Response
    {
        public required int Links { get; set; }
        public required bool Blocked { get; set; }
        public required DateTimeOffset? BlockedUntil { get; set; }
    }

    public class Handler : BaseCommandHandler, IRequestHandler<Request, Response>
    {
        private readonly IGuestManager _guestManager;
        
        public Handler(IApplicationDbContext dbContext, IGuestManager guestManager) : base(dbContext)
        {
            _guestManager = guestManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var guest = await _guestManager.GetGuestUser();

            if (guest is null)
            {
                throw new ErrorException("GuestUserNotFound");
            }
            
            return new Response()
            {
                Links = guest.Links,
                Blocked = guest.Blocked,
                BlockedUntil = guest.BlockedUntil
            };
        }
    }
}
using MediatR;

namespace DotNetCore6_Domain.MediatR.Requests;

//  ReadonlyRecordStructHandler
public sealed record ReadonlyRecordStructRequest(string input) : IRequest<MyResponse>;
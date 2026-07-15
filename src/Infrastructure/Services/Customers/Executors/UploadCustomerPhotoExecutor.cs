using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class UploadCustomerPhotoExecutor : CustomerExecutorBase, IUploadCustomerPhotoExecutor
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private const long MaxFileBytes = 5 * 1024 * 1024;

    public UploadCustomerPhotoExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ProfileResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        UploadCustomerPhotoRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(customerId, cancellationToken);
        await EnsureCanViewOrEditCustomerAsync(callerId, callerRole, customer, cancellationToken);

        if (request.Length <= 0)
        {
            throw new AppException("Photo file is required.", AppErrorType.Validation);
        }

        if (request.Length > MaxFileBytes)
        {
            throw new AppException("Photo must be 5 MB or smaller.", AppErrorType.Validation);
        }

        if (!AllowedContentTypes.Contains(request.ContentType))
        {
            throw new AppException("Photo must be a JPEG, PNG, or WebP image.", AppErrorType.Validation);
        }

        var extension = request.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => Path.GetExtension(request.FileName)
        };

        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new AppException("Could not determine photo file extension.", AppErrorType.Validation);
        }

        var profile = await GetOrCreateProfileAsync(customerId, cancellationToken);

        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var relativeDir = Path.Combine("uploads", "profiles", customerId.ToString("N"));
        var absoluteDir = Path.Combine(webRoot, relativeDir);
        Directory.CreateDirectory(absoluteDir);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(absoluteDir, fileName);
        var relativeUrl = $"/{relativeDir.Replace('\\', '/')}/{fileName}";

        await using (var fileStream = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await request.Content.CopyToAsync(fileStream, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(profile.PhotoPath))
        {
            var previousAbsolute = Path.Combine(webRoot, profile.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(previousAbsolute))
            {
                File.Delete(previousAbsolute);
            }
        }

        profile.PhotoPath = relativeUrl;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToProfileResponse(profile);
    }
}

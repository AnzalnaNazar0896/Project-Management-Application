using Microsoft.AspNetCore.Http;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Services
{
    public class AttachmentService
    {
        private readonly IAttachmentRepository _repository;

        public AttachmentService(IAttachmentRepository repository)
        {
            _repository = repository;
        }

        public List<Attachment> GetAll() => _repository.GetAll();

        public Attachment? GetById(int id) => _repository.GetById(id);

        public async Task<Attachment> SaveFileAsync(
            IFormFile file,
            string uploadRoot,
            string uploadedBy,
            int? taskId,
            int? projectId)
        {
            Directory.CreateDirectory(uploadRoot);
            var safeName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
            var physicalPath = Path.Combine(uploadRoot, safeName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{safeName}",
                FileType = file.ContentType,
                UploadedBy = uploadedBy,
                TaskItemId = taskId,
                ProjectId = projectId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(attachment);
            return attachment;
        }

        public int Count() => _repository.Count();
    }
}

using LibraryDomain.Model;
using LibraryInfrastructure;
namespace LibraryInfrastructure.Services
{
    public class PublisherDataPortServiceFactory
        : IDataPortServiceFactory<Publisher>
    {
        private readonly LibraryContext _context;
        public PublisherDataPortServiceFactory(LibraryContext context)
        {
            _context = context;
        }
        public IImportService<Publisher> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new PublisherImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for movies with content type {contentType}");
        }
        public IExportService<Publisher> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new PublisherExportService(_context);
            }
            if (contentType is "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                return new PublisherDocxExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for movies with content type {contentType}");
        }
    }
}